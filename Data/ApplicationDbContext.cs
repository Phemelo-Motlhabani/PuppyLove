using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using PupV1.Models;
namespace PupV1.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser /*IdentityUser*/, IdentityRole, string>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }
        public DbSet<Trainer> Trainers { get; set; }
        public DbSet<Client> Clients { get; set; }
        public DbSet<Breeder> Breeders { get; set; }
        public DbSet<Trainerskill> Trainerskills { get; set; }
        public DbSet<Skill> Skills { get; set; }
        public DbSet<Trainingrequest>Trainingrequests { get; set; }
        public DbSet<Parkrecommendation>Parkrecommendations { get; set; }
        public DbSet<TrainingProgress>TrainingProgresses { get; set; }
        public DbSet<Puppy>Puppies { get; set; }
        public DbSet<Litter> Litters { get; set; }
        public DbSet<Breedtype> Breedtypes { get; set; }
        public DbSet<PuppyDetails>PuppyDetailss { get; set; }
        public DbSet<Puppyrequest>Puppyrequests { get; set; }
        public DbSet<Review>Reviews { get; set; }
        public DbSet<BreedSpecialization> breedSpecializations { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ApplicationUser>()
                .HasOne(u => u.Trainer)
                .WithOne()
                .HasForeignKey<ApplicationUser>(u => u.TrainerId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ApplicationUser>()
                .HasOne(u => u.Client)
                .WithOne()
                .HasForeignKey<ApplicationUser> (u => u.ClientId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Skill>().HasData(
                new Skill { SkillId = 1, SkillName = "Animal Behaviour Knowledge" },
                new Skill { SkillId = 2, SkillName = "Training Techniques" },
                new Skill { SkillId = 3, SkillName = "Problem Behaviour Management" },
                new Skill { SkillId = 4, SkillName = "Breed_Specific Knowledge" },
                new Skill { SkillId = 5, SkillName = "Safety & First Aid for Dogs" },
                new Skill { SkillId = 6, SkillName = "Crate Training" },
                new Skill { SkillId = 7, SkillName = "House Training" },
                new Skill { SkillId = 8, SkillName = "Recall Training" },
                new Skill { SkillId = 9, SkillName = "Socialization Techniques" },
                new Skill { SkillId = 10, SkillName = "Impluse Control Training" }
                );
        }
    }
}
