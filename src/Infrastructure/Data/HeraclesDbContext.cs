using Heracles.Application.Tracks;
using Heracles.Application.Weather;
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
        public DbSet<TrackPointData> TrackPointData { get; set; }

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

                entity.HasOne(weather => weather.Track)
                    .WithOne()
                    .HasForeignKey<ActivityWeather>(weather => weather.TrackId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
            
            modelBuilder.Entity<TrackPointData>(entity =>
            {
                entity.ToTable("TrackPointData");

                entity.HasKey(data =>
                    new
                    {
                        data.TrackId,
                        data.Seq
                    });

                entity.HasIndex(data => data.TrackPointId)
                    .IsUnique();

                entity.HasOne<Track>()
                    .WithMany()
                    .HasForeignKey(data => data.TrackId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne<TrackPoint>()
                    .WithOne()
                    .HasForeignKey<TrackPointData>(data => data.TrackPointId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}