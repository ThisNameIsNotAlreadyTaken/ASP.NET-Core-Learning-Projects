using System;
using System.Linq;
using System.Threading.Tasks;
using First_App.Data;
using First_App.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace First_App.Controllers
{
    public class DepartmentsController : Controller
    {
        private readonly SchoolDbContext _context;

        public DepartmentsController(SchoolDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var schoolContext = _context.Departments.Include(d => d.Administrator);
            return View(await schoolContext.ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            const string query = "SELECT * FROM Department WHERE DepartmentId = {0}";

            var department = await _context.Departments
                .FromSql(query, id)
                .Include(d => d.Administrator)
                .AsNoTracking()
                .SingleOrDefaultAsync();

            if (department == null)
            {
                return NotFound();
            }

            return View(department);
        }

        public IActionResult Create()
        {
            ViewData["InstructorId"] = new SelectList(_context.Instructors, "Id", "FullName");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("DepartmentId,Budget,InstructorId,Name,RowVersion,StartDate")] Department department)
        {
            if (ModelState.IsValid)
            {
                _context.Add(department);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewData["InstructorId"] = new SelectList(_context.Instructors, "Id", "FullName", department.InstructorId);
            return View(department);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var department = await _context.Departments
                .Include(i => i.Administrator)
                .AsNoTracking()
                .SingleOrDefaultAsync(m => m.DepartmentId == id);

            if (department == null)
            {
                return NotFound();
            }

            ViewData["InstructorId"] = new SelectList(_context.Instructors, "Id", "FullName", department.InstructorId);

            return View(department);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int? id, byte[] rowVersion)
        {
            if (id == null)
            {
                return NotFound();
            }

            var departmentToUpdate = await _context.Departments.Include(i => i.Administrator).SingleOrDefaultAsync(m => m.DepartmentId == id);

            if (departmentToUpdate == null)
            {
                var deletedDepartment = new Department();
                await TryUpdateModelAsync(deletedDepartment);
                ModelState.AddModelError(string.Empty, "Unable to save changes. The department was deleted by another user.");
                ViewData["InstructorId"] = new SelectList(_context.Instructors, "Id", "FullName", deletedDepartment.InstructorId);
                return View(deletedDepartment);
            }

            _context.Entry(departmentToUpdate).Property("RowVersion").OriginalValue = rowVersion;

            if (await TryUpdateModelAsync(departmentToUpdate, "", s => s.Name, s => s.StartDate, s => s.Budget, s => s.InstructorId))
            {
                try
                {
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Index");
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    var exceptionEntry = ex.Entries.Single();
                    // Using a NoTracking query means we get the entity but it is not tracked by the context
                    // and will not be merged with existing entities in the context.
                    var databaseEntity = await _context.Departments.AsNoTracking().SingleAsync(d => d.DepartmentId == ((Department)exceptionEntry.Entity).DepartmentId);
                    var databaseEntry = _context.Entry(databaseEntity);

                    var databaseName = (string)databaseEntry.Property("Name").CurrentValue;
                    var proposedName = (string)exceptionEntry.Property("Name").CurrentValue;
                    if (databaseName != proposedName)
                    {
                        ModelState.AddModelError("Name", $"Current value: {databaseName}");
                    }
                    var databaseBudget = (decimal)databaseEntry.Property("Budget").CurrentValue;
                    var proposedBudget = (decimal)exceptionEntry.Property("Budget").CurrentValue;
                    if (databaseBudget != proposedBudget)
                    {
                        ModelState.AddModelError("Budget", $"Current value: {databaseBudget:c}");
                    }
                    var databaseStartDate = (DateTime)databaseEntry.Property("StartDate").CurrentValue;
                    var proposedStartDate = (DateTime)exceptionEntry.Property("StartDate").CurrentValue;
                    if (databaseStartDate != proposedStartDate)
                    {
                        ModelState.AddModelError("StartDate", $"Current value: {databaseStartDate:d}");
                    }
                    var databaseInstructorId = (int)databaseEntry.Property("InstructorId").CurrentValue;
                    var proposedInstructorId = (int)exceptionEntry.Property("InstructorId").CurrentValue;
                    if (databaseInstructorId != proposedInstructorId)
                    {
                        var databaseInstructor = await _context.Instructors.SingleAsync(i => i.Id == databaseInstructorId);
                        ModelState.AddModelError("InstructorId", $"Current value: {databaseInstructor.FullName}");
                    }

                    ModelState.AddModelError(string.Empty, "The record you attempted to edit "
                            + "was modified by another user after you got the original value. The "
                            + "edit operation was canceled and the current values in the database "
                            + "have been displayed. If you still want to edit this record, click "
                            + "the Save button again. Otherwise click the Back to List hyperlink.");
                    departmentToUpdate.RowVersion = (byte[])databaseEntry.Property("RowVersion").CurrentValue;
                    ModelState.Remove("RowVersion");
                }
            }
            ViewData["InstructorId"] = new SelectList(_context.Instructors, "Id", "FullName", departmentToUpdate.InstructorId);
            return View(departmentToUpdate);
        }

        public async Task<IActionResult> Delete(int? id, bool? concurrencyError)
        {
            if (id == null)
            {
                return NotFound();
            }

            var department = await _context.Departments
                .Include(d => d.Administrator)
                .AsNoTracking()
                .SingleOrDefaultAsync(m => m.DepartmentId == id);

            if (department == null)
            {
                if (concurrencyError.GetValueOrDefault())
                {
                    return RedirectToAction("Index");
                }
                return NotFound();
            }

            if (concurrencyError.GetValueOrDefault())
            {
                ViewData["ConcurrencyErrorMessage"] = "The record you attempted to delete "
                    + "was modified by another user after you got the original values. "
                    + "The delete operation was canceled and the current values in the "
                    + "database have been displayed. If you still want to delete this "
                    + "record, click the Delete button again. Otherwise "
                    + "click the Back to List hyperlink.";
            }

            return View(department);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Department department)
        {
            try
            {
                if (!await _context.Departments.AnyAsync(m => m.DepartmentId == department.DepartmentId)) return RedirectToAction("Index");
                _context.Departments.Remove(department);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            catch (DbUpdateConcurrencyException /* ex */)
            {
                //Log the error (uncomment ex variable name and write a log.)
                return RedirectToAction("Delete", new { concurrencyError = true, id = department.DepartmentId });
            }
        }
    }
}
