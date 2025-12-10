using Microsoft.Data.SqlClient;
using Dapper;
using _232127DelaCruzIPTCRUD.Models;

namespace _232127DelaCruzIPTCRUD.Services
{
    public class _232127DelaCruzStudentService
    {
        private readonly string _connectionString;

        public _232127DelaCruzStudentService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string not found");
        }

        public async Task<IEnumerable<_232127DelaCruzStudentModel>> GetAllStudentsAsync()
        {
            using var connection = new SqlConnection(_connectionString);
            return await connection.QueryAsync<_232127DelaCruzStudentModel>(@"
                SELECT 
                    s.STUDENTID,
                    s.FNAME,
                    s.LNAME,
                    s.MNAME,
                    s.BDAY,
                    s.GENDER,
                    s.COURSEID,
                    c.DESCRIPTION as CourseName
                FROM [232127DelaCruzSTUDENT] s
                LEFT JOIN [232127DelaCruzCOURSE] c ON s.COURSEID = c.COURSEID
                ORDER BY s.STUDENTID");
        }

        public async Task<_232127DelaCruzStudentModel?> GetStudentByIdAsync(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            return await connection.QueryFirstOrDefaultAsync<_232127DelaCruzStudentModel>(@"
                SELECT 
                    s.STUDENTID,
                    s.FNAME,
                    s.LNAME,
                    s.MNAME,
                    s.BDAY,
                    s.GENDER,
                    s.COURSEID,
                    c.DESCRIPTION as CourseName
                FROM [232127DelaCruzSTUDENT] s
                LEFT JOIN [232127DelaCruzCOURSE] c ON s.COURSEID = c.COURSEID
                WHERE s.STUDENTID = @Id",
                new { Id = id });
        }

        public async Task<int> CreateStudentAsync(_232127DelaCruzStudentModel student)
        {
            using var connection = new SqlConnection(_connectionString);
            return await connection.ExecuteScalarAsync<int>(@"
                INSERT INTO [232127DelaCruzSTUDENT] (FNAME, LNAME, MNAME, BDAY, GENDER, COURSEID) 
                VALUES (@FNAME, @LNAME, @MNAME, @BDAY, @GENDER, @COURSEID);
                SELECT CAST(SCOPE_IDENTITY() as int)",
                student);
        }

        public async Task<bool> UpdateStudentAsync(_232127DelaCruzStudentModel student)
        {
            using var connection = new SqlConnection(_connectionString);
            var result = await connection.ExecuteAsync(@"
                UPDATE [232127DelaCruzSTUDENT] 
                SET FNAME = @FNAME, LNAME = @LNAME, MNAME = @MNAME, 
                    BDAY = @BDAY, GENDER = @GENDER, COURSEID = @COURSEID
                WHERE STUDENTID = @STUDENTID",
                student);
            return result > 0;
        }

        public async Task<bool> DeleteStudentAsync(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            var result = await connection.ExecuteAsync(
                "DELETE FROM [232127DelaCruzSTUDENT] WHERE STUDENTID = @Id",
                new { Id = id });
            return result > 0;
        }

        public async Task<IEnumerable<_232127DelaCruzStudentModel>> SearchStudentsAsync(string searchTerm)
        {
            using var connection = new SqlConnection(_connectionString);
            return await connection.QueryAsync<_232127DelaCruzStudentModel>(@"
                SELECT 
                    s.STUDENTID,
                    s.FNAME,
                    s.LNAME,
                    s.MNAME,
                    s.BDAY,
                    s.GENDER,
                    s.COURSEID,
                    c.DESCRIPTION as CourseName
                FROM [232127DelaCruzSTUDENT] s
                LEFT JOIN [232127DelaCruzCOURSE] c ON s.COURSEID = c.COURSEID
                WHERE s.FNAME LIKE @SearchTerm 
                   OR s.LNAME LIKE @SearchTerm 
                   OR s.MNAME LIKE @SearchTerm
                ORDER BY s.LNAME, s.FNAME",
                new { SearchTerm = $"%{searchTerm}%" });
        }

        public async Task<IEnumerable<_232127DelaCruzStudentModel>> GetStudentsByCourseAsync(int courseId)
        {
            using var connection = new SqlConnection(_connectionString);
            return await connection.QueryAsync<_232127DelaCruzStudentModel>(@"
                SELECT 
                    s.STUDENTID,
                    s.FNAME,
                    s.LNAME,
                    s.MNAME,
                    s.BDAY,
                    s.GENDER,
                    s.COURSEID,
                    c.DESCRIPTION as CourseName
                FROM [232127DelaCruzSTUDENT] s
                LEFT JOIN [232127DelaCruzCOURSE] c ON s.COURSEID = c.COURSEID
                WHERE s.COURSEID = @CourseId
                ORDER BY s.LNAME, s.FNAME",
                new { CourseId = courseId });
        }

        public async Task<IEnumerable<_232127DelaCruzStudentModel>> GetBirthdayCelebrantsByMonthAsync(int month)
        {
            using var connection = new SqlConnection(_connectionString);
            return await connection.QueryAsync<_232127DelaCruzStudentModel>(@"
                SELECT 
                    s.STUDENTID,
                    s.FNAME,
                    s.LNAME,
                    s.MNAME,
                    s.BDAY,
                    s.GENDER,
                    s.COURSEID,
                    c.DESCRIPTION as CourseName
                FROM [232127DelaCruzSTUDENT] s
                LEFT JOIN [232127DelaCruzCOURSE] c ON s.COURSEID = c.COURSEID
                WHERE MONTH(s.BDAY) = @Month
                ORDER BY DAY(s.BDAY), s.LNAME, s.FNAME",
                new { Month = month });
        }

        public async Task<IEnumerable<GenderStatistic>> GetGenderStatisticsAsync()
        {
            using var connection = new SqlConnection(_connectionString);
            return await connection.QueryAsync<GenderStatistic>(@"
                SELECT 
                    GENDER as Gender,
                    COUNT(*) as Count,
                    CAST(COUNT(*) * 100.0 / (SELECT COUNT(*) FROM [232127DelaCruzSTUDENT]) AS DECIMAL(5,2)) as Percentage
                FROM [232127DelaCruzSTUDENT]
                GROUP BY GENDER");
        }
    }
}
