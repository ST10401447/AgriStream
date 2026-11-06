using AgriStream.mvc.Models;
using AgriStream.mvc.Models;
using Microsoft.EntityFrameworkCore;

namespace AgriStream.mvc.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Device> Devices { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Usernames must be unique
            modelBuilder.Entity<User>().HasIndex(u => u.Username).IsUnique();

            // Starting devices
            modelBuilder.Entity<Device>().HasData(
                new Device { DeviceId = 1, DeviceCode = "SENSOR-001", FieldName = "North Field", Status = "Idle", LastUpdated = new DateTime(2026, 11, 1) },
                new Device { DeviceId = 2, DeviceCode = "SENSOR-002", FieldName = "South Field", Status = "Idle", LastUpdated = new DateTime(2026, 11, 1) },
                new Device { DeviceId = 3, DeviceCode = "SENSOR-003", FieldName = "East Field", Status = "Idle", LastUpdated = new DateTime(2026, 11, 1) }
            );
        }
    }
}