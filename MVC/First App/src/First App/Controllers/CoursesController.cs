using System.Linq;
using System.Threading.Tasks;
using First_App.Data;
using First_App.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace First_App.Controllers
{
    public class CoursesController : Controller
    {
        private readonly SchoolDbContext _context;

        public CoursesController(SchoolDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var courses = _context.Courses.Include(c => c.Department).AsNoTracking();

            return View(await courses.ToListAsync());
        }

        public IActionResult Create()
        {
            PopulateDepartmentsDropDownList();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CourseID,Credits,DepartmentID,Title")] Course course)
        {
            if (ModelState.IsValid)
            {
                _context.Add(course);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            PopulateDepartmentsDropDownList(course.DepartmentId);
            return View(course);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var course = await _context.Courses.AsNoTracking().SingleOrDefaultAsync(m => m.CourseId == id);

            if (course == null)
            {
                return NotFound();
            }
            PopulateDepartmentsDropDownList(course.DepartmentId);
            return View(course);
        }

        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPost(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var courseToUpdate = await _context.Courses.SingleOrDefaultAsync(c => c.CourseId == id);

            if (await TryUpdateModelAsync(courseToUpdate, "", c => c.Credits, c => c.DepartmentId, c => c.Title))
            {
                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateException /* ex */)
                {
                    //Log the error (uncomment ex variable name and write a log.)
                    ModelState.AddModelError("", "Unable to save changes. " +
                        "Try again, and if the problem persists, " +
                        "see your system administrator.");
                }
                return RedirectToAction("Index");
            }
            PopulateDepartmentsDropDownList(courseToUpdate.DepartmentId);
            return View(courseToUpdate);
        }

        private void PopulateDepartmentsDropDownList(object selectedDepartment = null)
        {
            var departmentsQuery = _context.Departments.OrderBy(d => d.Name).Select(d => d);
                
            ViewBag.DepartmentId = new SelectList(departmentsQuery.AsNoTracking(), "DepartmentId", "Name", selectedDepartment);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var course = await _context.Courses.Include(c => c.Department).AsNoTracking().SingleOrDefaultAsync(m => m.CourseId == id);

            if (course == null)
            {
                return NotFound();
            }

            return View(course);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var course = await _context.Courses.Include(c => c.Department).AsNoTracking().SingleOrDefaultAsync(m => m.CourseId == id);

            if (course == null)
            {
                return NotFound();
            }

            return View(course);
        }

        public IActionResult UpdateCourseCredits()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> UpdateCourseCredits(int? multiplier)
        {
            if (multiplier != null)
            {
                ViewData["RowsAffected"] = await _context.Database.ExecuteSqlCommandAsync("UPDATE Course SET Credits = Credits * {0}", parameters: multiplier);
            }
            return View();
        }
    }
}
