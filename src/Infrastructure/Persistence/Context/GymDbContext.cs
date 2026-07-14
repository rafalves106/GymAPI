using GymAPI.Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;

namespace GymAPI.Infrastructure.Persistence.Context;

public class GymDbContext : DbContext
{
    public GymDbContext(DbContextOptions<GymDbContext> options) : base(options) { }

    public DbSet<ExerciseEntity> Exercises => Set<ExerciseEntity>();
    public DbSet<ExerciseMuscleGroupEntity> ExerciseMuscleGroups => Set<ExerciseMuscleGroupEntity>();
    public DbSet<ExerciseEquipmentEntity> ExerciseEquipments => Set<ExerciseEquipmentEntity>();
    public DbSet<UserEntity> Users => Set<UserEntity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<ExerciseEntity>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Description).IsRequired().HasMaxLength(500);
            entity.Property(e => e.VideoUrl).HasMaxLength(500);
        });

        modelBuilder.Entity<ExerciseMuscleGroupEntity>(entity =>
        {
            entity.HasKey(e => new { e.ExerciseId, e.MuscleGroup });

            entity.HasOne(e => e.Exercise)
                .WithMany(ex => ex.ExerciseMuscleGroups)
                .HasForeignKey(e => e.ExerciseId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<ExerciseEquipmentEntity>(entity =>
        {
            entity.HasKey(e => new { e.ExerciseId, e.EquipmentType });

            entity.HasOne(e => e.Exercise)
                .WithMany(ex => ex.ExerciseEquipments)
                .HasForeignKey(e => e.ExerciseId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<UserEntity>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Email).IsUnique();
            entity.Property(e => e.Email).IsRequired().HasMaxLength(256);
            entity.Property(e => e.PasswordHash).IsRequired();
            entity.Property(e => e.FirstName).IsRequired().HasMaxLength(50);
            entity.Property(e => e.LastName).IsRequired().HasMaxLength(50);
        });
    }
}
