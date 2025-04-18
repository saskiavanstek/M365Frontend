using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using LabTrainer.Models;
using LabTrainer.Data;
using System.Threading.Tasks;
using System.Linq;

namespace LabTrainer.Pages.Klanten
{
    public class CreateModel : PageModel
    {
        private readonly LabTrainerContext _context;

        public CreateModel(LabTrainerContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Klant Klant { get; set; }

        public string SuccessMessage { get; set; }
        public string ErrorMessage { get; set; }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var bestaandeKlant = _context.Klanten.FirstOrDefault(k => k.KlantNaam == Klant.KlantNaam);
            if (bestaandeKlant != null)
            {
                ErrorMessage = $"❌ De klant '{Klant.KlantNaam}' bestaat al.";
                return Page();
            }

            _context.Klanten.Add(Klant);
            await _context.SaveChangesAsync();

            SuccessMessage = $"✅ Klant '{Klant.KlantNaam}' succesvol toegevoegd!";
            ModelState.Clear(); // Leeg het formulier na succes
            Klant = new Klant(); // Reset model
            return Page();
        }
    }
}
