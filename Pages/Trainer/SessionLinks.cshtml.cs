using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using LabTrainer.Data;
using LabTrainer.Models;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Net.Mail;
using System.Net;
using Microsoft.AspNetCore.Authorization;

namespace LabTrainer.Pages.Trainer
{
   // [AllowAnonymous]
    public class SessionLinksModel : PageModel
    {
        private readonly LabTrainerContext _context;

        public SessionLinksModel(LabTrainerContext context)
        {
            _context = context;
        }

        // Het geselecteerde sessie object
        public TrainingSession SelectedSession { get; set; }

        public async Task OnGetAsync(int sessionId)
        {
            // Haal de geselecteerde sessie op op basis van het ID
            SelectedSession = await _context.TrainingSessions
                .Include(s => s.Participants) // Zorg ervoor dat we de deelnemers ook ophalen
                .FirstOrDefaultAsync(s => s.Id == sessionId);
        }

        // POST actie voor het versturen van de e-mail
        public async Task<IActionResult> OnPostEmailLinksAsync()
        {
            if (SelectedSession != null)
            {
                // Zet de e-mailgegevens samen
                var emailSubject = "Deelnemers Links voor Sessie: " + SelectedSession.RepositoryName;
                var emailBody = "<h3>Deelnemers Links:</h3><ul>";

                foreach (var participant in SelectedSession.Participants)
                {
                    emailBody += $"<li><a href='{participant.UniqueLink}'>Link voor {participant.UniqueLink}</a></li>";
                }
                emailBody += "</ul>";

                // Stel de e-mail in
                var fromAddress = new MailAddress("jouw-email@voorbeeld.com", "Training Kalender");
                var toAddress = new MailAddress("deelnemer@example.com", "Deelnemer"); // Kan aangepast worden naar een lijst van deelnemers.
                const string fromPassword = "jouw-email-wachtwoord"; // Stel dit veilig in
                const string subject = "Deelnemers Links voor Sessie";

                using (var smtp = new SmtpClient("smtp.example.com") // Gebruik je eigen SMTP-server
                {
                    Port = 587,
                    Credentials = new NetworkCredential(fromAddress.Address, fromPassword),
                    EnableSsl = true,
                })
                {
                    using (var message = new MailMessage(fromAddress, toAddress)
                    {
                        Subject = subject,
                        Body = emailBody,
                        IsBodyHtml = true
                    })
                    {
                        await smtp.SendMailAsync(message);
                    }
                }
                // Redirect na het verzenden van de e-mail (terug naar de kalender)
                return RedirectToPage("/Trainer/Calendar");
            }

            return Page();
        }
    }
}
