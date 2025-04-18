// Models/TrainingSession.cs
using System;
using System.ComponentModel.DataAnnotations;

namespace LabTrainer.Models
{
    public class TrainingSession
    {
        public int SessionCounter { get; set; }
        [Key]
        public int Id { get; set; }
        public string RepositoryName { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime ExpiryDate { get; set; }
        public int NumberOfParticipants { get; set; }
        public string UniqueLinkPrefix { get; set; }
        
        public bool IsActive {get; set;}

        public int KlantId { get; set; } // Koppel de klant aan de deelnemer

        public List<Participant> Participants { get; set; }

    }

    
}