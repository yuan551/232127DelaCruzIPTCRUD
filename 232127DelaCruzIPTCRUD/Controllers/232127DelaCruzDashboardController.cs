using Microsoft.AspNetCore.Mvc;
using _232127DelaCruzIPTCRUD.Models;
using _232127DelaCruzIPTCRUD.Services;
using Microsoft.Data.SqlClient;
using Dapper;

namespace _232127DelaCruzIPTCRUD.Controllers
{
    public class _232127DelaCruzDashboardController : Controller
    {
        private readonly _232127DelaCruzStudentService _studentService;
        private readonly _232127DelaCruzCourseService _courseService;
        private readonly string _connectionString;

        public _232127DelaCruzDashboardController(
            _232127DelaCruzStudentService studentService,
            _232127DelaCruzCourseService courseService,
            IConfiguration configuration)
        {
            _studentService = studentService;
            _courseService = courseService;
            _connectionString = configuration.GetConnectionString("DefaultConnection") ?? "";
        }

        public async Task<IActionResult> Dashboard()
        {
            // Check if logged in
            if (HttpContext.Session.GetInt32("UserId") == null)
            {
                return RedirectToAction("Login", "_232127DelaCruzUser");
            }

            var model = new _232127DelaCruzDashboardModel();

            using (var connection = new SqlConnection(_connectionString))
            {
                model.TotalStudents = await connection.ExecuteScalarAsync<int>(
                    "SELECT COUNT(*) FROM [232127DelaCruzSTUDENT]");
                model.TotalCourses = await connection.ExecuteScalarAsync<int>(
                    "SELECT COUNT(*) FROM [232127DelaCruzCOURSE]");
                model.TotalUsers = await connection.ExecuteScalarAsync<int>(
                    "SELECT COUNT(*) FROM [232127DelaCruzUSER]");

                model.RecentStudents = (await connection.QueryAsync<_232127DelaCruzStudentModel>(@"
                    SELECT TOP 5 
                        s.STUDENTID,
                        s.FNAME,
                        s.LNAME,
                        s.MNAME,
                        c.DESCRIPTION as CourseName
                    FROM [232127DelaCruzSTUDENT] s
                    LEFT JOIN [232127DelaCruzCOURSE] c ON s.COURSEID = c.COURSEID
                    ORDER BY s.STUDENTID DESC")).ToList();

                model.UpcomingBirthdays = (await connection.QueryAsync<_232127DelaCruzStudentModel>(@"
                    SELECT 
                        s.STUDENTID,
                        s.FNAME,
                        s.LNAME,
                        s.BDAY,
                        c.DESCRIPTION as CourseName
                    FROM [232127DelaCruzSTUDENT] s
                    LEFT JOIN [232127DelaCruzCOURSE] c ON s.COURSEID = c.COURSEID
                    WHERE MONTH(s.BDAY) = MONTH(GETDATE()) 
                       OR (MONTH(s.BDAY) = MONTH(DATEADD(MONTH, 1, GETDATE())) AND DAY(s.BDAY) <= DAY(GETDATE()))
                    ORDER BY MONTH(s.BDAY), DAY(s.BDAY)")).ToList();
            }

            model.GenderDistribution = (await _studentService.GetGenderStatisticsAsync()).ToList();
            model.CourseDistribution = (await _courseService.GetCoursesWithStudentCountAsync()).ToList();

            ViewBag.UserName = HttpContext.Session.GetString("UserName");
            return View(model);
        }
    }
}
