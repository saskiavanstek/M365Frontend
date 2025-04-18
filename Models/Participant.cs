// Models/Participant.cs
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LabTrainer.Models
{
    public class Participant
    {
        [Key]
        public int Id { get; set; }
        public string UniqueLink { get; set; }
        public int TrainingSessionId { get; set; }
        [ForeignKey("TrainingSessionId")]
        public TrainingSession TrainingSession { get; set; }
        public int KlantId { get; set; } // Koppel de klant aan de deelnemer

        public Klant Klant { get; set; }
    }
}