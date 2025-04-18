using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using LabTrainer.Data;
using LabTrainer.Models;
using System.Linq;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using System.ComponentModel.DataAnnotations;

namespace LabTrainer.Pages
{
   // [AllowAnonymous]
    public class TrainerModel : PageModel
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly HttpClient _httpClient;
        private readonly LabTrainerContext _context;
        private readonly IConfiguration _configuration;

        public TrainerModel(HttpClient httpClient, LabTrainerContext context, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _httpClient = httpClient;
            _context = context;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor; // Initialiseer het veld met de geïnjecteerde instantie
        }

        [BindProperty]
        [Required(ErrorMessage = "Selecteer een repository.")]
        public string RepositoryName { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "Selecteer een klant.")]
        public int KlantId { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "De startdatum is verplicht.")]
        public DateTime StartDate { get; set; }

        [BindProperty]
        public DateTime ExpiryDate { get; set; }

        [BindProperty]
        [Range(1, int.MaxValue, ErrorMessage = "Het aantal deelnemers moet meer dan 0 zijn.")]
        public int NumberOfParticipants { get; set; }

        public List<SelectListItem> RepositoryOptions { get; set; }

        public string Message { get; set; }
        public List<string>? ParticipantLinks { get; set; } // Maak ParticipantLinks nullable
        public List<Klant> Klanten { get; set; }

        public async Task OnGetAsync()
        {
            Console.WriteLine("OnGetAsync aangeroepen"); // Loggen
            RepositoryOptions = await GetRepositoriesFromAzureFunction();
            StartDate = DateTime.Now.Date;
            ExpiryDate = DateTime.Now.Date;
            Klanten = _context.Klanten.ToList(); // Haal de klanten op uit de database
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                RepositoryOptions = await GetRepositoriesFromAzureFunction();
                Klanten = _context.Klanten.ToList(); // Laad de klanten opnieuw
                return Page();
            }

            try
            {
                // Haal de laatste teller op
                int lastCounter = await _context.TrainingSessions.MaxAsync(ts => (int?)ts.SessionCounter) ?? 0;

                Console.WriteLine($"KlantId: {KlantId}");
                var trainingSession = new TrainingSession
                {
                    RepositoryName = RepositoryName,
                    StartDate = StartDate,
                    ExpiryDate = ExpiryDate,
                    NumberOfParticipants = NumberOfParticipants,
                    UniqueLinkPrefix = (lastCounter + 1).ToString(),
                    SessionCounter = lastCounter + 1,
                    IsActive = ExpiryDate >= DateTime.Now,
                    KlantId = KlantId // Koppel de klant aan de trainingsessie
                };

                _context.TrainingSessions.Add(trainingSession);
                await _context.SaveChangesAsync();
                _context.Entry(trainingSession).Reload();


                var baseUrl = $"{_httpContextAccessor.HttpContext.Request.Scheme}://{_httpContextAccessor.HttpContext.Request.Host.Value}";


                for (int i = 0; i < NumberOfParticipants; i++)
                {
                    var participant = new Participant
                    {
                        TrainingSessionId = trainingSession.Id,
                        UniqueLink = $"{baseUrl}/Participants/{trainingSession.UniqueLinkPrefix}-{i + 1}/{RepositoryName}?IsStudent=true",
                        KlantId = KlantId // Koppel de klant aan de trainingsessie
                    };

                    _context.Participants.Add(participant);
                }

                await _context.SaveChangesAsync();

                Message = "Trainingssessie aangemaakt!";
                ParticipantLinks = _context.Participants
                    .Where(p => p.TrainingSessionId == trainingSession.Id)
                    .Select(p => p.UniqueLink)
                    .ToList();

                //Reset Formulier velden
                RepositoryName = null;
                StartDate = DateTime.Now.Date;
                ExpiryDate = DateTime.Now.Date;
                NumberOfParticipants = 0;

                //Laad Repositories opnieuw
                RepositoryOptions = await GetRepositoriesFromAzureFunction();
                Klanten = _context.Klanten.ToList(); // Haal de klanten op uit de database

                return Page();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Fout bij het opslaan van de gegevens: {ex.Message}");
                ModelState.AddModelError(string.Empty, "Er is een fout opgetreden bij het opslaan van de gegevens.");
                RepositoryOptions = await GetRepositoriesFromAzureFunction();
                Klanten = _context.Klanten.ToList(); // Laad de klanten opnieuw
                return Page();
            }
        }

        private async Task<List<SelectListItem>> GetRepositoriesFromAzureFunction()
        {
            Console.WriteLine("GetRepositoriesFromAzureFunction aangeroepen"); // Loggen
            try
            {
                //var functionKey = _configuration["AzureFunctionSettings:RepositoriesFunctionKey"];
                var FunctionRepoUrl = _configuration["AzureFunctionSettings:RepositoriesFunctionUrl"];

                Console.WriteLine($"Aanroepen van URL: {FunctionRepoUrl}"); // Loggen

                var response = await _httpClient.GetAsync(FunctionRepoUrl);
                Console.WriteLine($"Response status code: {response.StatusCode}"); // Loggen
                response.EnsureSuccessStatusCode();
                var json = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"JSON response: {json}"); // Loggen
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var repositories = JsonSerializer.Deserialize<List<GitHubRepo>>(json, options);

                if (repositories != null)
                {
                    Console.WriteLine($"Aantal repositories na deserialisatie: {repositories.Count}"); // Loggen
                    return repositories.Select(repo => new SelectListItem
                    {
                        Value = repo.Name,
                        Text = repo.Name
                    }).ToList();
                }
                else
                {
                    Console.WriteLine("Fout: Deserialisatie van repositories gaf null.");
                    return new List<SelectListItem>();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Fout bij het ophalen van repositories: {ex.Message}");
                return new List<SelectListItem>();
            }
        }

     public class GitHubRepo
{
    [JsonPropertyName("id")]
    public int Id { get; set; } // Verander string naar int

    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("url")]
    public string Url { get; set; }
}
    }
}