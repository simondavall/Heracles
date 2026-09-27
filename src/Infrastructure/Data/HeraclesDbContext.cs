using System.Reflection;
using Heracles.Application.Data;
using Microsoft.EntityFrameworkCore;

namespace Heracles.Infrastructure.Data
{
    public class HeraclesDbContext : DbContext
    {
        public HeraclesDbContext(DbContextOptions<HeraclesDbContext> options) : base(options) {
        }

        public DbSet<Track> Tracks { get; set; }
        public DbSet<TrackSegment> TrackSegments { get; set; }
        public DbSet<TrackPoint> TrackPoints { get; set; }
        public DbSet<ActivityWeather> ActivityWeather { get; set; }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken()) {
            return await base.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        }

        public override int SaveChanges() {
            return SaveChangesAsync().GetAwaiter().GetResult();
        }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ActivityWeather>(entity =>
            {
                entity.ToTable("ActivityWeather");

                entity.HasKey(weather => weather.TrackId);

                // entity.Property(weather => weather.Conditions)
                //     .HasMaxLength(200);
                //
                // entity.Property(weather => weather.Icon)
                //     .HasMaxLength(100);

                entity.HasOne(weather => weather.Track)
                    .WithOne()
                    .HasForeignKey<ActivityWeather>(
                        weather => weather.TrackId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}