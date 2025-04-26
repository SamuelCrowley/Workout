using MeetUp.Data.Base.AbstractClasses;
using MeetUp.Data.Gym;
using MeetUp.Data.User;
using MeetUp.Extensions;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using static MeetUp.Enums.Enums;

namespace MeetUp.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUserEO>
{
    public DbSet<GymUserEO> GymUsers { get; set; }
    public DbSet<GymSessionEO> GymSessions { get; set; }
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

        builder.Entity<GymUserEO>()
            .HasOne(x => x.ApplicationUserEO) 
            .WithOne(x => x.GymUserEO) 
            .HasForeignKey<GymUserEO>(x => x.ParentRef)  
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<GymSessionEO>().ToTable(EntityObjectType.GymSessionEO.GetDisplayName()).HasKey(x => x.ClassRef);

        builder.Entity<GymSessionEO>()
            .HasOne(x => x.GymUserEO)  
            .WithMany(x => x.GymSessionEOs)  
            .HasForeignKey(x => x.ParentRef) 
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<GymExerciseEO>().ToTable(EntityObjectType.GymExerciseEO.GetDisplayName()).HasKey(x => x.ClassRef);

        builder.Entity<GymExerciseEO>()
           .HasOne(x => x.GymSessionEO)  
           .WithMany(x => x.GymExerciseEOs)  
           .HasForeignKey(x => x.ParentRef)  
           .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<GymSetEO>().ToTable(EntityObjectType.GymSetEO.GetDisplayName()).HasKey(x => x.ClassRef);

        builder.Entity<GymSetEO>()
           .HasOne(x => x.GymExerciseEO)
           .WithMany(x => x.GymSetEOs)
           .HasForeignKey(x => x.ParentRef)
           .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<GymRepetitionEO>().ToTable(EntityObjectType.GymRepetitionEO.GetDisplayName()).HasKey(x => x.ClassRef);

        builder.Entity<GymRepetitionEO>()
           .HasOne(x => x.GymSetEO)
           .WithMany(x => x.GymRepetitionEOs)
           .HasForeignKey(x => x.ParentRef)
           .OnDelete(DeleteBehavior.Cascade);
    }
}
