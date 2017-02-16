using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using First_App.Data;
using First_App.Models.SchoolViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace First_App.Controllers
{
    public class UnivercityController : Controller
    {
        private readonly SchoolDbContext _context;

        public UnivercityController(SchoolDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<ActionResult> About()
        {
            var data = _context.Students.GroupBy(s => s.EnrollmentDate).Select(g => new EnrollmentDateGroup
            {
                EnrollmentDate = g.Key,
                StudentCount = g.Count()
            });

            return View(await data.AsNoTracking().ToListAsync());
        }

        public async Task<ActionResult> AboutCustomCommand()
        {
            var groups = new List<EnrollmentDateGroup>();
            var conn = _context.Database.GetDbConnection();
            try
            {
                await conn.OpenAsync();
                using (var command = conn.CreateCommand())
                {
                    command.CommandText = @"SELECT EnrollmentDate, COUNT(*) AS StudentCount FROM Person
                        WHERE Discriminator = 'Student'
                        GROUP BY EnrollmentDate";

                    var reader = await command.ExecuteReaderAsync();

                    if (reader.HasRows)
                    {
                        while (await reader.ReadAsync())
                        {
                            var row = new EnrollmentDateGroup { EnrollmentDate = reader.GetDateTime(0), StudentCount = reader.GetInt32(1) };
                            groups.Add(row);
                        }
                    }
                    reader.Dispose();
                }
            }
            finally
            {
                conn.Close();
            }
            return View(groups);
        }
    }
}
