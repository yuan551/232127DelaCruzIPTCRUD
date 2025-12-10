using Microsoft.Data.SqlClient;
using Dapper;
using _232127DelaCruzIPTCRUD.Models;

namespace _232127DelaCruzIPTCRUD.Services
{
    public class _232127DelaCruzCourseService
    {
        private readonly string _connectionString;

        public _232127DelaCruzCourseService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string not found");
        }

        public async Task<IEnumerable<_232127DelaCruzCourseModel>> GetAllCoursesAsync()
        {
            using var connection = new SqlConnection(_connectionString);
            return await connection.QueryAsync<_232127DelaCruzCourseModel>(
                "SELECT * FROM [232127DelaCruzCOURSE] ORDER BY COURSEID");
        }

        public async Task<_232127DelaCruzCourseModel?> GetCourseByIdAsync(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            return await connection.QueryFirstOrDefaultAsync<_232127DelaCruzCourseModel>(
                "SELECT * FROM [232127DelaCruzCOURSE] WHERE COURSEID = @Id",
                new { Id = id });
        }

        public async Task<int> CreateCourseAsync(string description)
        {
            using var connection = new SqlConnection(_connectionString);
            return await connection.ExecuteScalarAsync<int>(
                "INSERT INTO [232127DelaCruzCOURSE] (DESCRIPTION) VALUES (@Description); SELECT CAST(SCOPE_IDENTITY() as int)",
                new { Description = description });
        }

        public async Task<bool> UpdateCourseAsync(int id, string description)
        {
            using var connection = new SqlConnection(_connectionString);
            var result = await connection.ExecuteAsync(
                "UPDATE [232127DelaCruzCOURSE] SET DESCRIPTION = @Description WHERE COURSEID = @Id",
                new { Id = id, Description = description });
            return result > 0;
        }

        public async Task<bool> DeleteCourseAsync(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            var result = await connection.ExecuteAsync(
                "DELETE FROM [232127DelaCruzCOURSE] WHERE COURSEID = @Id",
                new { Id = id });
            return result > 0;
        }

        public async Task<IEnumerable<_232127DelaCruzCourseModel>> GetCoursesWithStudentCountAsync()
        {
            using var connection = new SqlConnection(_connectionString);
            return await connection.QueryAsync<_232127DelaCruzCourseModel>(@"
                SELECT 
                    c.COURSEID,
                    c.DESCRIPTION,
                    COUNT(s.STUDENTID) as StudentCount
                FROM [232127DelaCruzCOURSE] c
                LEFT JOIN [232127DelaCruzSTUDENT] s ON c.COURSEID = s.COURSEID
                GROUP BY c.COURSEID, c.DESCRIPTION
                ORDER BY c.COURSEID");
        }
    }
}
