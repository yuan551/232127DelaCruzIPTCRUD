using Microsoft.AspNetCore.Mvc;
using _232127DelaCruzIPTCRUD.Models;
using _232127DelaCruzIPTCRUD.Services;

namespace _232127DelaCruzIPTCRUD.Controllers
{
    public class _232127DelaCruzCourseController : Controller
    {
        private readonly _232127DelaCruzCourseService _courseService;

        public _232127DelaCruzCourseController(_232127DelaCruzCourseService courseService)
        {
            _courseService = courseService;
        }

        public async Task<IActionResult> Index()
        {
            // Check if logged in
            if (HttpContext.Session.GetInt32("UserId") == null)
            {
                return RedirectToAction("Login", "_232127DelaCruzUser");
            }

            var courses = await _courseService.GetCoursesWithStudentCountAsync();
            ViewBag.UserName = HttpContext.Session.GetString("UserName");
            return View(courses);
        }

        [HttpGet]
        public IActionResult Create()
        {
            // Check if logged in
            if (HttpContext.Session.GetInt32("UserId") == null)
            {
                return RedirectToAction("Login", "_232127DelaCruzUser");
            }

            ViewBag.UserName = HttpContext.Session.GetString("UserName");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(_232127DelaCruzCourseModel course)
        {
            if (ModelState.IsValid)
            {
                await _courseService.CreateCourseAsync(course.DESCRIPTION);
                TempData["Success"] = "Course created successfully";
                return RedirectToAction("Index");
            }

            ViewBag.UserName = HttpContext.Session.GetString("UserName");
            return View(course);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            // Check if logged in
            if (HttpContext.Session.GetInt32("UserId") == null)
            {
                return RedirectToAction("Login", "_232127DelaCruzUser");
            }

            var course = await _courseService.GetCourseByIdAsync(id);
            if (course == null)
            {
                return NotFound();
            }

            ViewBag.UserName = HttpContext.Session.GetString("UserName");
            return View(course);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(_232127DelaCruzCourseModel course)
        {
            if (ModelState.IsValid)
            {
                await _courseService.UpdateCourseAsync(course.COURSEID, course.DESCRIPTION);
                TempData["Success"] = "Course updated successfully";
                return RedirectToAction("Index");
            }

            ViewBag.UserName = HttpContext.Session.GetString("UserName");
            return View(course);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            await _courseService.DeleteCourseAsync(id);
            TempData["Success"] = "Course deleted successfully";
            return RedirectToAction("Index");
        }
    }
}
