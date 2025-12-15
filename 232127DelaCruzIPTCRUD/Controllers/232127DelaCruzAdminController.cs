using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using _232127DelaCruzIPTCRUD.Services;

namespace _232127DelaCruzIPTCRUD.Controllers
{
    public class _232127DelaCruzAdminController : Controller
    {
        private readonly string _connectionString;
        private readonly _232127DelaCruzUserService _userService;

        public _232127DelaCruzAdminController(IConfiguration configuration, _232127DelaCruzUserService userService)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string not found");
            _userService = userService;
        }

        [HttpGet]
        public IActionResult CreateTables()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser(string fname, string lname, string password)
        {
            if (string.IsNullOrWhiteSpace(fname) || string.IsNullOrWhiteSpace(lname) || string.IsNullOrWhiteSpace(password))
            {
                ViewBag.UserError = "All fields are required.";
                return View("CreateTables");
            }

            var user = new Models._232127DelaCruzUserModel
            {
                FNAME = fname,
                LNAME = lname,
                MNAME = string.Empty,
                PASSWORD = password
            };

            try
            {
                var newId = await _userService.CreateUserAsync(user);
                ViewBag.UserSuccess = $"User created with ID {newId}. You can now log in using that user ID and the password you provided.";
            }
            catch (Exception ex)
            {
                ViewBag.UserError = ex.Message;
            }

            return View("CreateTables");
        }

        [HttpPost]
        public IActionResult CreateTables(string studentNumber, string lastName, bool createUser = true, bool createStudent = true, bool createCourse = true)
        {
            // Basic validation: studentNumber digits only, lastName letters only
            if (string.IsNullOrWhiteSpace(studentNumber) || string.IsNullOrWhiteSpace(lastName))
            {
                ViewBag.Error = "Student number and last name are required.";
                return View();
            }

            if (!System.Text.RegularExpressions.Regex.IsMatch(studentNumber, "^[0-9]+$") || !System.Text.RegularExpressions.Regex.IsMatch(lastName, "^[A-Za-z]+$"))
            {
                ViewBag.Error = "Student number must be digits only and last name must contain letters only.";
                return View();
            }

            var baseName = studentNumber + lastName;
            var log = new List<string>();

            using var connection = new SqlConnection(_connectionString);
            connection.Open();

            try
            {
                if (createCourse)
                {
                    var tableName = $"[{baseName}COURSE]";
                    var sql = $@"IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'{tableName}') AND type in (N'U'))
BEGIN
    CREATE TABLE {tableName} (
        COURSEID INT IDENTITY(1,1) PRIMARY KEY,
        DESCRIPTION NVARCHAR(250) NOT NULL
    );
END";
                    using var cmd = new SqlCommand(sql, connection);
                    cmd.ExecuteNonQuery();
                    log.Add($"Created or found table {tableName}");
                }

                if (createStudent)
                {
                    var tableName = $"[{baseName}STUDENT]";
                    var sql = $@"IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'{tableName}') AND type in (N'U'))
BEGIN
    CREATE TABLE {tableName} (
        STUDENTID INT IDENTITY(1,1) PRIMARY KEY,
        FNAME NVARCHAR(100) NOT NULL,
        LNAME NVARCHAR(100) NOT NULL,
        MNAME NVARCHAR(100) NULL,
        BDAY DATE NOT NULL,
        GENDER NVARCHAR(20) NOT NULL,
        COURSEID INT NULL
    );
END";
                    using var cmd = new SqlCommand(sql, connection);
                    cmd.ExecuteNonQuery();
                    log.Add($"Created or found table {tableName}");
                }

                if (createUser)
                {
                    var tableName = $"[{baseName}USER]";
                    var sql = $@"IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'{tableName}') AND type in (N'U'))
BEGIN
    CREATE TABLE {tableName} (
        USERID INT IDENTITY(1,1) PRIMARY KEY,
        FNAME NVARCHAR(100) NOT NULL,
        LNAME NVARCHAR(100) NOT NULL,
        MNAME NVARCHAR(100) NULL,
        PASSWORD NVARCHAR(500) NULL
    );
END";
                    using var cmd = new SqlCommand(sql, connection);
                    cmd.ExecuteNonQuery();
                    log.Add($"Created or found table {tableName}");
                }

                ViewBag.Success = string.Join("\n", log);
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
            }
            finally
            {
                connection.Close();
            }

            return View();
        }
    }
}
