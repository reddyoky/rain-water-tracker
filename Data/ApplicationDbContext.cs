using Microsoft.EntityFrameworkCore;
using WaterTracker.Models;

namespace WaterTracker.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<AppUser> Users { get; set; }
    public DbSet<GlassType> GlassTypes { get; set; }
    public DbSet<WaterLog> WaterLogs { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // WaterLog → AppUser ilişkisi
        modelBuilder.Entity<WaterLog>()
            .HasOne(w => w.AppUser)
            .WithMany(u => u.WaterLogs)
            .HasForeignKey(w => w.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // Seed: Hazır bardak türleri (migration'a işlenir)
        modelBuilder.Entity<GlassType>().HasData(
            new GlassType { Id = 1, Name = "Küçük Bardak", CapacityMl = 200, Emoji = "🥛" },
            new GlassType { Id = 2, Name = "Büyük Bardak", CapacityMl = 300, Emoji = "🥤" },
            new GlassType { Id = 3, Name = "Şişe",         CapacityMl = 500, Emoji = "💧" },
            new GlassType { Id = 4, Name = "Büyük Şişe",   CapacityMl = 750, Emoji = "🍶" }
        );
    }
}
