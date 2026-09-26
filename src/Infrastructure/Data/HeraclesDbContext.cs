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

        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken()) {
            return await base.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        }

        public override int SaveChanges() {
            return SaveChangesAsync().GetAwaiter().GetResult();
        }
    }
}