using Microsoft.AspNetCore.Mvc;
using _232127DelaCruzIPTCRUD.Models;
using _232127DelaCruzIPTCRUD.Services;

namespace _232127DelaCruzIPTCRUD.Controllers
{
    public class _232127DelaCruzReportController : Controller
    {
        private readonly _232127DelaCruzStudentService _studentService;
        private readonly _232127DelaCruzCourseService _courseService;

        public _232127DelaCruzReportController(
            _232127DelaCruzStudentService studentService,
            _232127DelaCruzCourseService courseService)
        {
            _studentService = studentService;
            _courseService = courseService;
        }

        public async Task<IActionResult> Index(string reportType = "allstudents", int? month = null, int? courseId = null)
        {
            // Check if logged in
            if (HttpContext.Session.GetInt32("UserId") == null)
            {
                return RedirectToAction("Login", "_232127DelaCruzUser");
            }

            ViewBag.UserName = HttpContext.Session.GetString("UserName");
            ViewBag.ReportType = reportType;
            ViewBag.Month = month;
            ViewBag.CourseId = courseId;
            ViewBag.Courses = await _courseService.GetAllCoursesAsync();

            switch (reportType)
            {
                case "allstudents":
                    ViewBag.Students = await _studentService.GetAllStudentsAsync();
                    break;

                case "percourse":
                    if (courseId.HasValue)
                    {
                        ViewBag.Students = await _studentService.GetStudentsByCourseAsync(courseId.Value);
                    }
                    break;

                case "birthday":
                    if (month.HasValue)
                    {
                        ViewBag.Students = await _studentService.GetBirthdayCelebrantsByMonthAsync(month.Value);
                    }
                    break;

                case "coursechart":
                    ViewBag.CourseDistribution = await _courseService.GetCoursesWithStudentCountAsync();
                    break;

                case "genderchart":
                    ViewBag.GenderDistribution = await _studentService.GetGenderStatisticsAsync();
                    break;
            }

            return View();
        }
    }
}
