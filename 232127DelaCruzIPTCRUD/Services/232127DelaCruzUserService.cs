using Microsoft.Data.SqlClient;
using Dapper;
using _232127DelaCruzIPTCRUD.Models;

namespace _232127DelaCruzIPTCRUD.Services
{
    public class _232127DelaCruzUserService
    {
        private readonly string _connectionString;

        public _232127DelaCruzUserService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string not found");
        }

        public async Task<_232127DelaCruzUserModel?> AuthenticateAsync(int userid, string password)
        {
            using var connection = new SqlConnection(_connectionString);
            var user = await connection.QueryFirstOrDefaultAsync<_232127DelaCruzUserModel>(
                "SELECT * FROM [232127DelaCruzUSER] WHERE USERID = @UserId",
                new { UserId = userid });

            if (user != null && !string.IsNullOrEmpty(user.PASSWORD))
            {
                bool isValid = false;
                
                // Check if password is BCrypt hash or plain text
                if (user.PASSWORD.StartsWith("$2a$") || user.PASSWORD.StartsWith("$2b$") || user.PASSWORD.StartsWith("$2y$"))
                {
                    // BCrypt hash - verify with BCrypt
                    try
                    {
                        isValid = BCrypt.Net.BCrypt.Verify(password, user.PASSWORD);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"BCrypt verification failed: {ex.Message}");
                        isValid = false;
                    }
                }
                else
                {
                    // Plain text password - direct comparison (for testing only)
                    isValid = user.PASSWORD == password;
                }
                
                if (isValid)
                {
                    user.PASSWORD = null; // Don't return password
                    return user;
                }
            }

            return null;
        }

        public async Task<IEnumerable<_232127DelaCruzUserModel>> GetAllUsersAsync()
        {
            using var connection = new SqlConnection(_connectionString);
            var users = await connection.QueryAsync<_232127DelaCruzUserModel>(
                "SELECT USERID, FNAME, LNAME, MNAME FROM [232127DelaCruzUSER] ORDER BY USERID");
            return users;
        }

        public async Task<int> CreateUserAsync(_232127DelaCruzUserModel user)
        {
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(user.PASSWORD);
            using var connection = new SqlConnection(_connectionString);
            return await connection.ExecuteScalarAsync<int>(@"
                INSERT INTO [232127DelaCruzUSER] (FNAME, LNAME, MNAME, PASSWORD) 
                VALUES (@FNAME, @LNAME, @MNAME, @HashedPassword);
                SELECT CAST(SCOPE_IDENTITY() as int)",
                new { user.FNAME, user.LNAME, user.MNAME, HashedPassword = hashedPassword });
        }
    }
}
