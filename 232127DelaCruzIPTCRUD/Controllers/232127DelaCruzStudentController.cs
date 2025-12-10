using Microsoft.AspNetCore.Mvc;
using _232127DelaCruzIPTCRUD.Models;
using _232127DelaCruzIPTCRUD.Services;

namespace _232127DelaCruzIPTCRUD.Controllers
{
    public class _232127DelaCruzStudentController : Controller
    {
        private readonly _232127DelaCruzStudentService _studentService;
        private readonly _232127DelaCruzCourseService _courseService;

        public _232127DelaCruzStudentController(
            _232127DelaCruzStudentService studentService,
            _232127DelaCruzCourseService courseService)
        {
            _studentService = studentService;
            _courseService = courseService;
        }

        public async Task<IActionResult> Index(string? search, int? courseId)
        {
            // Check if logged in
            if (HttpContext.Session.GetInt32("UserId") == null)
            {
                return RedirectToAction("Login", "_232127DelaCruzUser");
            }

            IEnumerable<_232127DelaCruzStudentModel> students;

            // Get all students first
            students = await _studentService.GetAllStudentsAsync();

            // Apply search filter if provided
            if (!string.IsNullOrEmpty(search))
            {
                students = students.Where(s => 
                    s.FNAME.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                    s.LNAME.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                    (!string.IsNullOrEmpty(s.MNAME) && s.MNAME.Contains(search, StringComparison.OrdinalIgnoreCase))
                ).ToList();
            }

            // Apply course filter if provided
            if (courseId.HasValue && courseId.Value > 0)
            {
                students = students.Where(s => s.COURSEID == courseId.Value).ToList();
            }

            ViewBag.Courses = await _courseService.GetAllCoursesAsync();
            ViewBag.UserName = HttpContext.Session.GetString("UserName");
            ViewBag.Search = search;
            ViewBag.CourseId = courseId;
            return View(students);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            // Check if logged in
            if (HttpContext.Session.GetInt32("UserId") == null)
            {
                return RedirectToAction("Login", "_232127DelaCruzUser");
            }

            ViewBag.Courses = await _courseService.GetAllCoursesAsync();
            ViewBag.UserName = HttpContext.Session.GetString("UserName");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(_232127DelaCruzStudentModel student)
        {
            if (ModelState.IsValid)
            {
                await _studentService.CreateStudentAsync(student);
                TempData["Success"] = "Student created successfully";
                return RedirectToAction("Index");
            }

            ViewBag.Courses = await _courseService.GetAllCoursesAsync();
            ViewBag.UserName = HttpContext.Session.GetString("UserName");
            return View(student);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            // Check if logged in
            if (HttpContext.Session.GetInt32("UserId") == null)
            {
                return RedirectToAction("Login", "_232127DelaCruzUser");
            }

            var student = await _studentService.GetStudentByIdAsync(id);
            if (student == null)
            {
                return NotFound();
            }

            ViewBag.Courses = await _courseService.GetAllCoursesAsync();
            ViewBag.UserName = HttpContext.Session.GetString("UserName");
            return View(student);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(_232127DelaCruzStudentModel student)
        {
            if (ModelState.IsValid)
            {
                await _studentService.UpdateStudentAsync(student);
                TempData["Success"] = "Student updated successfully";
                return RedirectToAction("Index");
            }

            ViewBag.Courses = await _courseService.GetAllCoursesAsync();
            ViewBag.UserName = HttpContext.Session.GetString("UserName");
            return View(student);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            await _studentService.DeleteStudentAsync(id);
            TempData["Success"] = "Student deleted successfully";
            return RedirectToAction("Index");
        }
    }
}
