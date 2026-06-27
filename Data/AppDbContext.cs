using Microsoft.EntityFrameworkCore;
using Pravesh.API.Entities;
using Pravesh.API.Enums;

namespace Pravesh.API.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    // ── DbSets
    public DbSet<User>Users{ get; set; }
    public DbSet<Society>Societies{ get; set; }
    public DbSet<Flat>Flats{ get; set; }
    public DbSet<Gate>Gates{ get; set; }
    public DbSet<VisitorPass>VisitorPasses{ get; set; }
    public DbSet<EntryLog>EntryLogs{ get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // ── User 
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasIndex(u => u.Email).IsUnique();

            entity.Property(u => u.Role)
                  .HasConversion<string>();   // stored as VARCHAR in MySQL

            // User → Flat (resident lives in a flat)
            entity.HasOne(u => u.Flat)
                  .WithOne(f => f.Resident)
                  .HasForeignKey<User>(u => u.FlatId)
                  .OnDelete(DeleteBehavior.SetNull);

            // User → Society
            entity.HasOne(u => u.Society)
                  .WithMany(s => s.Users)
                  .HasForeignKey(u => u.SocietyId)
                  .OnDelete(DeleteBehavior.SetNull);
        });

        // ── Society 
        modelBuilder.Entity<Society>(entity =>
        {
            // Society → Admin (User); separate relationship from the Users collection
            entity.HasOne(s => s.Admin)
                  .WithMany()
                  .HasForeignKey(s => s.AdminId)
                  .OnDelete(DeleteBehavior.SetNull);
        });

        // ── Flat 
        modelBuilder.Entity<Flat>(entity =>
        {
            // Composite unique: no duplicate flat numbers within the same society
            entity.HasIndex(f => new { f.SocietyId, f.FlatNumber }).IsUnique();

            entity.HasOne(f => f.Society)
                  .WithMany(s => s.Flats)
                  .HasForeignKey(f => f.SocietyId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // ── Gate 
        modelBuilder.Entity<Gate>(entity =>
        {
            entity.HasOne(g => g.Society)
                  .WithMany(s => s.Gates)
                  .HasForeignKey(g => g.SocietyId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(g => g.AssignedGuard)
                  .WithMany()
                  .HasForeignKey(g => g.AssignedGuardId)
                  .OnDelete(DeleteBehavior.SetNull);
        });

        // ── VisitorPass 
        modelBuilder.Entity<VisitorPass>(entity =>
        {
            entity.HasIndex(vp => vp.Uuid).IsUnique();

            entity.Property(vp => vp.PassType)
                  .HasConversion<string>();

            entity.Property(vp => vp.Status)
                  .HasConversion<string>();

            entity.HasOne(vp => vp.Flat)
                  .WithMany(f => f.VisitorPasses)
                  .HasForeignKey(vp => vp.FlatId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(vp => vp.Resident)
                  .WithMany(u => u.VisitorPasses)
                  .HasForeignKey(vp => vp.ResidentId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // ── EntryLog 
        modelBuilder.Entity<EntryLog>(entity =>
        {
            entity.Property(el => el.ScanResult)
                  .HasConversion<string>();

            entity.Property(el => el.DenyReason)
                  .HasConversion<string>();

            entity.HasOne(el => el.VisitorPass)
                  .WithMany(vp => vp.EntryLogs)
                  .HasForeignKey(el => el.PassId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(el => el.Gate)
                  .WithMany(g => g.EntryLogs)
                  .HasForeignKey(el => el.GateId)
                  .OnDelete(DeleteBehavior.SetNull);

            entity.HasOne(el => el.Guard)
                  .WithMany(u => u.EntryLogs)
                  .HasForeignKey(el => el.GuardId)
                  .OnDelete(DeleteBehavior.SetNull);
        });
    }
}
