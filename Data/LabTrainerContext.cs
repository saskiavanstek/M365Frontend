// Data/LabTrainerContext.cs
using Microsoft.EntityFrameworkCore;
using LabTrainer.Models;

namespace LabTrainer.Data
{
    public class LabTrainerContext : DbContext
    {
        public LabTrainerContext(DbContextOptions<LabTrainerContext> options) : base(options) { }

        public DbSet<TrainingSession> TrainingSessions { get; set; }
        public DbSet<Participant> Participants { get; set; }
        public DbSet<Klant> Klanten { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Participant>()
                .HasOne(p => p.Klant)
                .WithMany()
                .HasForeignKey(p => p.KlantId);
        }
    }
}