using Microsoft.AspNetCore.Mvc;
using LabTrainer.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace LabTrainer.Controllers
{
   // [AllowAnonymous]
    [Route("api/[controller]")]
    [ApiController]
    public class SessionsController : ControllerBase
    {
        private readonly LabTrainerContext _context;

        public SessionsController(LabTrainerContext context)
        {
            _context = context;
        }

        // GET: api/sessions?start=2025-04-01&end=2025-06-01
[HttpGet]
public async Task<IActionResult> GetSessions()
{
    // Loggen of de query wordt uitgevoerd
    Console.WriteLine("Fetching sessions...");

    var sessions = await _context.TrainingSessions
        //.Where(s => s.IsActive) // Alleen actieve sessies
        .Select(s => new
        {
            id = s.Id,
            title = s.RepositoryName,
            start = s.StartDate.ToString("yyyy-MM-dd")
        })
        .ToListAsync();

    if (!sessions.Any())
    {
        Console.WriteLine("Geen sessies gevonden.");
    }

    return Ok(sessions);
}

    }
}
