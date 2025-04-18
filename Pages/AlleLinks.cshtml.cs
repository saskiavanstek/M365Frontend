using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Linq;
using LabTrainer.Data;
using LabTrainer.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
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
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;


namespace LabTrainer.Pages
{
  //  [AllowAnonymous]
    public class AlleLinksModel : PageModel
    {
        private readonly LabTrainerContext _context;

        public AlleLinksModel(LabTrainerContext context)
        {
            _context = context;
        }

        public List<ParticipantViewModel> Links { get; set; }
        public List<SelectListItem> Klanten { get; set; }

        public int? KlantIdFilter { get; set; }
        public DateTime? StartDatumFilter { get; set; }

        public void OnGet(int? klantIdFilter, DateTime? startDatumFilter)
        {
            KlantIdFilter = klantIdFilter;
            StartDatumFilter = startDatumFilter;

            Klanten = _context.Klanten.Select(k => new SelectListItem
            {
                Value = k.KlantId.ToString(),
                Text = k.KlantNaam
            }).ToList();

            var query = _context.Participants
                .Include(p => p.TrainingSession)
                .Include(p => p.Klant)
                .Select(p => new ParticipantViewModel
                {
                    Id = p.Id,
                    UniqueLink = p.UniqueLink,
                    TrainingSessionId = p.TrainingSessionId,
                    KlantId = p.KlantId,
                    KlantNaam = p.Klant.KlantNaam,
                    RepositoryName = p.TrainingSession.RepositoryName,
                    StartDate = p.TrainingSession.StartDate,
                    ExpiryDate = p.TrainingSession.ExpiryDate,
                    IsActive = p.TrainingSession.IsActive
                });

            if (KlantIdFilter.HasValue)
            {
                query = query.Where(p => p.KlantId == KlantIdFilter.Value);
            }

            if (StartDatumFilter.HasValue)
            {
                query = query.Where(p => p.StartDate.Date == StartDatumFilter.Value.Date);
            }

            Links = query.ToList();
        }

        public class ParticipantViewModel
        {
            public int Id { get; set; }
            public string UniqueLink { get; set; }
            public int TrainingSessionId { get; set; }
            public int KlantId { get; set; }
            public string KlantNaam { get; set; }
            public string RepositoryName { get; set; }
            public DateTime StartDate { get; set; }
            public DateTime ExpiryDate { get; set; }
            public bool IsActive { get; set; }
            public string ActiefTekst
            {
                get { return IsActive ? "Ja" : "Nee"; }
            }

            public string StartDateFormatted => StartDate.ToString("dd-MM-yyyy");
            public string ExpiryDateFormatted => ExpiryDate.ToString("dd-MM-yyyy");
        }
    }
}