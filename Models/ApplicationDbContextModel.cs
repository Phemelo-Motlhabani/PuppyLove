using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Scaffolding.Internal;

namespace PupV1.Models;

public partial class ApplicationDbContextModel : IdentityDbContext<IdentityUser>
{
    public ApplicationDbContextModel()
    {
    }

    public ApplicationDbContextModel(DbContextOptions<ApplicationDbContextModel> options)
        : base(options)
    {
    }

    public virtual DbSet<Admin> Admins { get; set; }

    public virtual DbSet<Breeder> Breeders { get; set; }

    public virtual DbSet<Breedtype> Breedtypes { get; set; }

    public virtual DbSet<Client> Clients { get; set; }

    public virtual DbSet<Litter> Litters { get; set; }

    public virtual DbSet<Parkrecommendation> Parkrecommendations { get; set; }

    public virtual DbSet<Puppy> Puppies { get; set; }

    public virtual DbSet<Puppyrequest> Puppyrequests { get; set; }

    public virtual DbSet<Review> Reviews { get; set; }

    public virtual DbSet<Skill> Skills { get; set; }

    public virtual DbSet<Trainer> Trainers { get; set; }

    public virtual DbSet<Trainerskill> Trainerskills { get; set; }

    public virtual DbSet<Trainingrequest> Trainingrequests { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
//#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseMySql("server=localhost;port=3306;database=mydb;user=root;password=1234", Microsoft.EntityFrameworkCore.ServerVersion.Parse("8.0.42-mysql"));

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb3_general_ci")
            .HasCharSet("utf8mb3");

        modelBuilder.Entity<Admin>(entity =>
        {
            entity.HasKey(e => e.AdminId).HasName("PRIMARY");
        });

        modelBuilder.Entity<Breeder>(entity =>
        {
            entity.HasKey(e => e.BreederId).HasName("PRIMARY");

            entity.Property(e => e.VerificationStatus).IsFixedLength();
        });

        modelBuilder.Entity<Breedtype>(entity =>
        {
            entity.HasKey(e => e.BreedId).HasName("PRIMARY");

            entity.Property(e => e.ActivityLevel).IsFixedLength();
            entity.Property(e => e.Grooming).IsFixedLength();
            entity.Property(e => e.Size).IsFixedLength();
        });

        modelBuilder.Entity<Client>(entity =>
        {
            entity.HasKey(e => e.ClientId).HasName("PRIMARY");

            entity.Property(e => e.ClientId).ValueGeneratedOnAdd();
        });

        modelBuilder.Entity<Litter>(entity =>
        {
            entity.HasKey(e => e.LitterId).HasName("PRIMARY");

            entity.Property(e => e.LitterId).ValueGeneratedNever();

            entity.HasOne(d => d.Breed).WithMany(p => p.Litters).HasConstraintName("fk_Litter_Breed");

            entity.HasOne(d => d.Breeder).WithMany(p => p.Litters).HasConstraintName("fk_Litter_Breeder");
        });

        modelBuilder.Entity<Parkrecommendation>(entity =>
        {
            entity.HasKey(e => e.ParkCode).HasName("PRIMARY");

            entity.Property(e => e.ParkCode).ValueGeneratedNever();

            entity.HasOne(d => d.Trainer).WithMany(p => p.Parkrecommendations).HasConstraintName("fk_ParkRecommendation_Trainer");
        });

        modelBuilder.Entity<Puppy>(entity =>
        {
            entity.HasKey(e => e.PuppyId).HasName("PRIMARY");

            entity.Property(e => e.PuppyId).ValueGeneratedNever();
            entity.Property(e => e.Gender).IsFixedLength();
            entity.Property(e => e.SaleStatus).IsFixedLength();
            entity.Property(e => e.Size).IsFixedLength();
            entity.Property(e => e.TrainingStatus).IsFixedLength();

            entity.HasOne(d => d.Client).WithMany(p => p.Puppies).HasConstraintName("fk_Puppy_Client");

            entity.HasOne(d => d.Litter).WithMany(p => p.Puppies).HasConstraintName("fk_Puppy_Litter");
        });

        modelBuilder.Entity<Puppyrequest>(entity =>
        {
            entity.HasKey(e => e.RequestId).HasName("PRIMARY");

            entity.Property(e => e.RequestId).ValueGeneratedNever();
            entity.Property(e => e.Status).IsFixedLength();

            entity.HasOne(d => d.Breeder).WithMany(p => p.Puppyrequests).HasConstraintName("fk_PuppyRequest_Breeder");

            entity.HasOne(d => d.Client).WithMany(p => p.Puppyrequests).HasConstraintName("fk_PuppyRequest_Client");
        });

        modelBuilder.Entity<Review>(entity =>
        {
            entity.HasKey(e => e.ReviewId).HasName("PRIMARY");

            entity.Property(e => e.ReviewId).ValueGeneratedNever();

            entity.HasOne(d => d.Breeder).WithMany(p => p.Reviews).HasConstraintName("fk_Review_Breeder");

            entity.HasOne(d => d.Client).WithMany(p => p.Reviews).HasConstraintName("fk_Review_Client");

            entity.HasOne(d => d.Trainer).WithMany(p => p.Reviews).HasConstraintName("fk_Review_Trainer");
        });

        modelBuilder.Entity<Skill>(entity =>
        {
            entity.HasKey(e => e.SkillId).HasName("PRIMARY");

            entity.Property(e => e.SkillId).ValueGeneratedNever();
        });

        modelBuilder.Entity<Trainer>(entity =>
        {
            entity.HasKey(e => e.TrainerId).HasName("PRIMARY");

            entity.Property(e => e.TrainerId).ValueGeneratedOnAdd();
            entity.Property(e => e.VerificationStatus).IsFixedLength();

            entity.HasMany(d => d.Breeds).WithMany(p => p.Trainers)
                .UsingEntity<Dictionary<string, object>>(
                    "Breedtrainer",
                    r => r.HasOne<Breedtype>().WithMany()
                        .HasForeignKey("BreedId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("fk_BreedTrainer_Breed"),
                    l => l.HasOne<Trainer>().WithMany()
                        .HasForeignKey("TrainerId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("fk_BreedTrainer_Trainer"),
                    j =>
                    {
                        j.HasKey("TrainerId", "BreedId")
                            .HasName("PRIMARY")
                            .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0 });
                        j.ToTable("breedtrainer");
                        j.HasIndex(new[] { "BreedId" }, "fk_BreedTrainer_Breed");
                        j.IndexerProperty<int>("TrainerId").HasColumnName("TrainerID");
                        j.IndexerProperty<int>("BreedId").HasColumnName("BreedID");
                    });
        });

        modelBuilder.Entity<Trainerskill>(entity =>
        {
            entity.HasKey(e => new { e.TrainerId, e.SkillId })
                .HasName("PRIMARY")
                .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0 });

            entity.HasOne(d => d.Skill).WithMany(p => p.Trainerskills)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_TrainerSkill_Skill");

            entity.HasOne(d => d.Trainer).WithMany(p => p.Trainerskills)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_TrainerSkill_Trainer");
        });

        modelBuilder.Entity<Trainingrequest>(entity =>
        {
            entity.HasKey(e => e.TrequestId).HasName("PRIMARY");

            //entity.Property(e => e.TrequestId).ValueGeneratedNever();
            entity.Property(e => e.RequestStatus).IsFixedLength(false);

            entity.HasOne(d => d.Client)
                .WithMany(p => p.Trainingrequests)
                .HasForeignKey(d => d.ClientId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_TrainingRequest_Client");

            entity.HasOne(d => d.Trainer)
                .WithMany(p => p.Trainingrequests)
                .HasForeignKey(d => d.TrainerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_TrainingRequest_Trainer");

            /*entity.HasOne(d => d.Trequest).WithOne(p => p.Trainingrequest)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_TrainingRequest_Trainer");*/
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
