using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Linq;
using LabTrainer.Data;
using LabTrainer.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;


namespace LabTrainer.Pages // Zorg ervoor dat de namespace correct is
{
    public class TrainerLinksModel : PageModel // Zorg ervoor dat de klasse correct is
    {
        private readonly LabTrainerContext _context;

        public TrainerLinksModel(LabTrainerContext context)
        {
            _context = context;
        }

        public List<Participant> Links { get; set; }
        public int? SessieId { get; set; }
        public int? KlantId { get; set; }
        public DateTime? StartDatum { get; set; }
        public DateTime? EindDatum { get; set; }
        public bool ActieveLinks { get; set; } = true;

        public void OnGet(int? sessieId, int? klantId, DateTime? startDatum, DateTime? eindDatum, bool actieveLinks = true)
{
    SessieId = sessieId;
    KlantId = klantId;
    StartDatum = startDatum;
    EindDatum = eindDatum;
    ActieveLinks = actieveLinks;

    IQueryable<Participant> query = _context.Participants.Include(p => p.TrainingSession);

    if (SessieId.HasValue)
    {
        query = query.Where(p => p.TrainingSessionId == SessieId.Value);
    }

    if (KlantId.HasValue)
    {
        query = query.Where(p => p.KlantId == KlantId.Value);
    }

    if (StartDatum.HasValue)
    {
        query = query.Where(p => p.TrainingSession.StartDate >= StartDatum.Value.Date);
    }

    if (EindDatum.HasValue)
    {
        query = query.Where(p => p.TrainingSession.ExpiryDate <= EindDatum.Value.Date);
    }

    if (ActieveLinks)
    {
        query = query.Where(p => p.TrainingSession.ExpiryDate >= DateTime.Now);
    }
    else
    {
        query = query.Where(p => p.TrainingSession.ExpiryDate < DateTime.Now);
    }

    Links = query.ToList();
}
    }
}