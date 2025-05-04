using Workout.Domain.Data.Base.AbstractClasses;
using Workout.Infrastructure.Data.User;
using Workout.Domain.Extensions;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Workout.Domain.Enums;
using Workout.Infrastructure.Data.Gym;

namespace Workout.Infrastructure.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUserEO>
{
    public DbSet<GymUserEO> GymUsers { get; set; }
    public DbSet<GymSessionEO> GymSessions { get; set; }
    public DbSet<GymExerciseEO> GymExercises { get; set; }
    public DbSet<GymSetEO> GymSets { get; set; }
    public DbSet<GymRepetitionEO> GymRepetitions { get; set; }
    
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {

    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // Validate all Added or Modified entities that inherit from BaseEO
        foreach (var entry in ChangeTracker.Entries<BaseEO>())
        {
            if (entry.State == EntityState.Added || entry.State == EntityState.Modified)
            {
                entry.Entity.Validate();
            }
        }

        return await base.SaveChangesAsync(cancellationToken);
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<GymUserEO>().ToTable(EntityObjectType.GymUserEO.GetDisplayName()).HasKey(x => x. ClassRef);
        //builder.Entity<GymUserEO>().Property(x => x.ClassRef).HasColumnName(EntityObjectType.GymUserEO.GetDisplayName() + "ClassRef"); 

        builder.Entity<GymUserEO>()
            .HasOne(x => x.ApplicationUserEO) 
            .WithOne(x => x.GymUserEO) 
            .HasForeignKey<GymUserEO>(x => x.ParentRef)  
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<GymSessionEO>().ToTable(EntityObjectType.GymSessionEO.GetDisplayName()).HasKey(x => x.ClassRef);
        //builder.Entity<GymSessionEO>().Property(x => x.ClassRef).HasColumnName(EntityObjectType.GymSessionEO.GetDisplayName() + "ClassRef");

        builder.Entity<GymSessionEO>()
            .HasOne(x => x.GymUserEO)  
            .WithMany(x => x.GymSessionEOs)  
            .HasForeignKey(x => x.ParentRef) 
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<GymExerciseEO>().ToTable(EntityObjectType.GymExerciseEO.GetDisplayName()).HasKey(x => x.ClassRef);
        //builder.Entity<GymExerciseEO>().Property(x => x.ClassRef).HasColumnName(EntityObjectType.GymExerciseEO.GetDisplayName() + "ClassRef");

        builder.Entity<GymExerciseEO>()
           .HasOne(x => x.GymSessionEO)  
           .WithMany(x => x.GymExerciseEOs)  
           .HasForeignKey(x => x.ParentRef)  
           .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<GymSetEO>().ToTable(EntityObjectType.GymSetEO.GetDisplayName()).HasKey(x => x.ClassRef);
        //builder.Entity<GymSetEO>().Property(x => x.ClassRef).HasColumnName(EntityObjectType.GymSetEO.GetDisplayName() + "ClassRef");

        builder.Entity<GymSetEO>()
           .HasOne(x => x.GymExerciseEO)
           .WithMany(x => x.GymSetEOs)
           .HasForeignKey(x => x.ParentRef)
           .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<GymRepetitionEO>().ToTable(EntityObjectType.GymRepetitionEO.GetDisplayName()).HasKey(x => x.ClassRef);
        //builder.Entity<GymRepetitionEO>().Property(x => x.ClassRef).HasColumnName(EntityObjectType.GymRepetitionEO.GetDisplayName() + "ClassRef");

        builder.Entity<GymRepetitionEO>()
           .HasOne(x => x.GymSetEO)
           .WithMany(x => x.GymRepetitionEOs)
           .HasForeignKey(x => x.ParentRef)
           .OnDelete(DeleteBehavior.Cascade);
    }
}
