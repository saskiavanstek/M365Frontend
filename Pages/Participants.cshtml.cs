using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Headers;
using System.Text.Json.Serialization;
using Markdig;
using Microsoft.AspNetCore.Authorization;
using LabTrainer.Data;
using LabTrainer.Models;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace LabTrainer.Pages
{
    [AllowAnonymous]
    public class ParticipantsModel : PageModel
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly LabTrainerContext _context;

        public string LearnModuleLink { get; set; }

        public ParticipantsModel(HttpClient httpClient, IConfiguration configuration, LabTrainerContext context)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _context = context;
        }

        [BindProperty(SupportsGet = true)]
        public string SessionId { get; set; }

        [BindProperty(SupportsGet = true)]
        public string RepositoryId { get; set; }

        [BindProperty]
        public string SelectedFilePath { get; set; }

        public List<LabFileItem> FilesAndFolders { get; set; }
        public string ErrorMessage { get; set; }
        public string MarkdownContent { get; set; }
        public string VerloopBericht { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            if (string.IsNullOrEmpty(SessionId) || string.IsNullOrEmpty(RepositoryId))
            {
                return NotFound();
            }

            string uniqueLinkPrefix = SessionId.Substring(0, SessionId.IndexOf('-'));

            var trainingSession = await _context.TrainingSessions
                .FirstOrDefaultAsync(ts => ts.UniqueLinkPrefix == uniqueLinkPrefix);

            if (trainingSession == null || !trainingSession.IsActive)
            {
                VerloopBericht = "Deze trainingssessie is niet actief.";
                return Page();
            }

            // 👉 Extract only the module code (e.g., MS-4005 from MS-4005-Craft-effective-prompts...)
            // Zoek een patroon zoals MS-4005, MS-4010 etc. aan het begin van de RepositoryId
            var match = Regex.Match(RepositoryId ?? "", @"^(MS-\d{4})", RegexOptions.IgnoreCase);
            var moduleCode = match.Success ? match.Groups[1].Value : RepositoryId;


            // Mapping dictionary
            var learnModuleLinks = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                { "MS-4005", "https://learn.microsoft.com/nl-nl/training/courses/ms-4005" },
                { "MS-4007", "https://learn.microsoft.com/nl-nl/training/courses/ms-4007" },
                { "MS-4010", "https://learn.microsoft.com/nl-nl/training/paths/build-plugins-connectors-microsoft-copilot-microsoft-365/" },
                { "MS-4011", "https://learn.microsoft.com/en-us/training/paths/build-custom-copilots-microsoft-teams/" },
                { "MS-4015", "https://learn.microsoft.com/en-us/training/paths/build-custom-copilots-microsoft-teams/" },
                { "MS-4004", "https://learn.microsoft.com/en-us/training/courses/ms-4004" }
            };

            if (!string.IsNullOrEmpty(moduleCode) && learnModuleLinks.TryGetValue(moduleCode, out var url))
            {
                LearnModuleLink = url;
            }

            try
            {
                var functionApiKey = _configuration["AzureFunctionSettings:LabFilesFunctionKey"];
                var apiUrl = $"https://m365labfunctions.azurewebsites.net/api/repositories/{RepositoryId}/labfiles?code={functionApiKey}";
                FilesAndFolders = await GetLabFilesFromFunction(apiUrl);
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Fout bij het ophalen van lab inhoud: {ex.Message}";
                Console.WriteLine($"Fout bij het ophalen van lab inhoud: {ex.Message}");
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!string.IsNullOrEmpty(SelectedFilePath))
            {
                try
                {
                    string branch = "master";
                    if (RepositoryId == "MS-4010-Extend-Microsoft365-Copilot-declarative-agents-VS-Code")
                    {
                        branch = "main";
                    }

                    var markdownApiUrl = $"https://raw.githubusercontent.com/IT-M365-Training/{RepositoryId}/{branch}/{SelectedFilePath}";
                    var markdownText = await _httpClient.GetStringAsync(markdownApiUrl);

                    string imageBaseUrl = $"https://raw.githubusercontent.com/IT-M365-Training/{RepositoryId}/{branch}/Instructions/Labs/media/";

                    string rewrittenMarkdown = Regex.Replace(
                        markdownText,
                        @"!\[(.*?)\]\(((\.\./|\./)?media/([^)]+))\)",
                        match =>
                        {
                            var altText = match.Groups[1].Value;
                            var imageFilename = match.Groups[4].Value;
                            return $"![{altText}]({imageBaseUrl}{imageFilename})";
                        },
                        RegexOptions.IgnoreCase
                    );

                    var pipeline = new MarkdownPipelineBuilder()
                        .UseAdvancedExtensions()
                        .Build();

                    MarkdownContent = Markdown.ToHtml(rewrittenMarkdown, pipeline);
                }
                catch (Exception ex)
                {
                    ErrorMessage = $"Fout bij het ophalen van markdown bestand: {ex.Message}";
                    Console.WriteLine($"Fout bij het ophalen van markdown bestand: {ex.Message}");
                }
            }
            else
            {
                ErrorMessage = "Selecteer een labbestand.";
            }

            return await OnGetAsync();
        }

        private async Task<List<LabFileItem>> GetLabFilesFromFunction(string apiUrl)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, apiUrl);
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var response = await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                ErrorMessage = $"Fout bij het ophalen van lab inhoud: {response.StatusCode} ({response.ReasonPhrase})";
                Console.WriteLine($"Fout bij het ophalen van lab inhoud: {response.StatusCode} ({response.ReasonPhrase})");
                return new List<LabFileItem>();
            }

            var responseContent = await response.Content.ReadAsStringAsync();
            var labFiles = JsonSerializer.Deserialize<List<LabFileItem>>(responseContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return labFiles ?? new List<LabFileItem>();
        }

        public class LabFileItem
        {
            public string Name { get; set; }
            public string Path { get; set; }
            public bool IsFolder { get; set; }
            public List<LabFileItem> SubItems { get; set; }
        }
    }
}
