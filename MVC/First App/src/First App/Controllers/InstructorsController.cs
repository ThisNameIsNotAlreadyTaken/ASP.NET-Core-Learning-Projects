using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using First_App.Data;
using First_App.Models;
using First_App.Models.SchoolViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace First_App.Controllers
{
    public class InstructorsController : Controller
    {
        private readonly SchoolDbContext _context;

        public InstructorsController(SchoolDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(int? id, int? courseId)
        {
            var viewModel = new InstructorIndexData
            {
                Instructors = await _context.Instructors
                    .Include(i => i.OfficeAssignment)
                    .Include(i => i.Courses)
                    .ThenInclude(i => i.Course)
                    .ThenInclude(i => i.Enrollments)
                    .ThenInclude(i => i.Student)
                    .Include(i => i.Courses)
                    .ThenInclude(i => i.Course)
                    .ThenInclude(i => i.Department)
                    .AsNoTracking()
                    .OrderBy(i => i.LastName)
                    .ToListAsync()
            };

            if (id != null)
            {
                ViewData["InstructorId"] = id.Value;
                var instructor = viewModel.Instructors.Single(i => i.Id == id.Value);
                viewModel.Courses = instructor.Courses.Select(s => s.Course);
            }

            if (courseId == null) return View(viewModel);

            ViewData["CourseId"] = courseId.Value;
            viewModel.Enrollments = viewModel.Courses.Single(x => x.CourseId == courseId).Enrollments;

            return View(viewModel);
        }

        public IActionResult Create()
        {
            var instructor = new Instructor {Courses = new List<CourseAssignment>()};
            PopulateAssignedCourseData(instructor);
            return View();
        }

        // POST: Instructors/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("FirstMidName,HireDate,LastName,OfficeAssignment")] Instructor instructor, string[] selectedCourses)
        {
            if (selectedCourses != null)
            {
                instructor.Courses = new List<CourseAssignment>();
                foreach (var course in selectedCourses)
                {
                    var courseToAdd = new CourseAssignment { InstructorId = instructor.Id, CourseId = int.Parse(course) };
                    instructor.Courses.Add(courseToAdd);
                }
            }
            if (!ModelState.IsValid) return View(instructor);
            _context.Add(instructor);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var instructor = await _context.Instructors
                .Include(i => i.OfficeAssignment)
                .Include(i => i.Courses).ThenInclude(i => i.Course)
                .AsNoTracking().SingleOrDefaultAsync(m => m.Id == id);

            if (instructor == null)
            {
                return NotFound();
            }

            PopulateAssignedCourseData(instructor);
            return View(instructor);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var instructor = await _context.Instructors.SingleOrDefaultAsync(m => m.Id == id);
            if (instructor == null)
            {
                return NotFound();
            }

            return View(instructor);
        }

        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int? id, string[] selectedCourses)
        {
            if (id == null)
            {
                return NotFound();
            }

            var instructorToUpdate = await _context.Instructors
                .Include(i => i.OfficeAssignment)
                .Include(i => i.Courses)
                .ThenInclude(i => i.Course)
                .SingleOrDefaultAsync(s => s.Id == id);

            if (!await TryUpdateModelAsync(instructorToUpdate, "", i => i.FirstMidName, i => i.LastName, i => i.HireDate, i => i.OfficeAssignment)) return View(instructorToUpdate);

            if (string.IsNullOrWhiteSpace(instructorToUpdate.OfficeAssignment?.Location))
            {
                instructorToUpdate.OfficeAssignment = null;
            }

            UpdateInstructorCourses(selectedCourses, instructorToUpdate);

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

        private void PopulateAssignedCourseData(Instructor instructor)
        {
            var allCourses = _context.Courses;
            var instructorCourses = new HashSet<int>(instructor.Courses.Select(c => c.Course.CourseId));
            var viewModel = new List<AssignedCourseData>();
            foreach (var course in allCourses)
            {
                viewModel.Add(new AssignedCourseData
                {
                    CourseId = course.CourseId,
                    Title = course.Title,
                    Assigned = instructorCourses.Contains(course.CourseId)
                });
            }

            ViewData["Courses"] = viewModel;
        }

        private void UpdateInstructorCourses(string[] selectedCourses, Instructor instructorToUpdate)
        {
            if (selectedCourses == null)
            {
                instructorToUpdate.Courses = new List<CourseAssignment>();
                return;
            }

            var selectedCoursesHs = new HashSet<string>(selectedCourses);
            var instructorCourses = new HashSet<int>(instructorToUpdate.Courses.Select(c => c.Course.CourseId));

            foreach (var course in _context.Courses)
            {
                if (selectedCoursesHs.Contains(course.CourseId.ToString()))
                {
                    if (!instructorCourses.Contains(course.CourseId))
                    {
                        instructorToUpdate.Courses.Add(new CourseAssignment { InstructorId = instructorToUpdate.Id, CourseId = course.CourseId });
                    }
                }
                else
                {
                    if (!instructorCourses.Contains(course.CourseId)) continue;
                    var courseToRemove = instructorToUpdate.Courses.SingleOrDefault(i => i.CourseId == course.CourseId);
                    _context.Remove(courseToRemove);
                }
            }
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var instructor = await _context.Instructors.SingleOrDefaultAsync(m => m.Id == id);
            if (instructor == null)
            {
                return NotFound();
            }

            return View(instructor);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var instructor = await _context.Instructors.Include(i => i.Courses).SingleAsync(i => i.Id == id);

            var departments = await _context.Departments.Where(d => d.InstructorId == id).ToListAsync();

            departments.ForEach(d => d.InstructorId = null);

            _context.Instructors.Remove(instructor);

            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }
    }
}
