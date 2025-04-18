using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using LabTrainer.Data;
using LabTrainer.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.AspNetCore.Authorization;

namespace LabTrainer.Pages.Trainer
{
   // [AllowAnonymous]
    public class ParticipantLinksModel : PageModel
    {
        private readonly LabTrainerContext _context;

        public ParticipantLinksModel(LabTrainerContext context)
        {
            _context = context;
        }

        // Voor dropdown
        public List<TrainingSession> SessionsWithParticipants { get; set; }

        // Voor weergave van de gefilterde sessie
        public TrainingSession SelectedSession { get; set; }

        // Voor binding van filterparameter in querystring
        public int? SessionFilter { get; set; }

        public async Task OnGetAsync(int? sessionFilter)
        {
            // Alle sessies met deelnemers ophalen
            SessionsWithParticipants = await _context.TrainingSessions
                .Include(s => s.Participants)
                .OrderByDescending(s => s.StartDate)
                .ToListAsync();

            // Filter opslaan
            SessionFilter = sessionFilter;

            // Enkel geselecteerde sessie ophalen voor weergave
            if (SessionFilter.HasValue)
            {
                SelectedSession = SessionsWithParticipants
                    .FirstOrDefault(s => s.Id == SessionFilter.Value);
            }
        }
    }
}
