using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Heracles.Application.TrackAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Heracles.Infrastructure.Data
{
    public class GpxDbContextSeed
    {
        public static async Task SeedAsync(IServiceProvider services)
        {
            var dbContext = services.GetRequiredService<GpxDbContext>();
            await RunMigrationsIfNeeded(dbContext);
            await SeedExampleDataIfNoDataExists(dbContext);
        }

        private static async Task SeedExampleDataIfNoDataExists(GpxDbContext dbContext)
        {
            if (!await dbContext.Tracks.AnyAsync())
            {
                var gpxTrack = new Track
                {
                    Name = "Testing Track",
                    TrackSegments = new List<TrackSegment>
                    {
                        new TrackSegment
                        {
                            TrackPoints = new List<TrackPoint>
                            {
                                new TrackPoint(),
                                new TrackPoint(),
                                new TrackPoint()
                            }
                        },
                        new TrackSegment
                        {
                            TrackPoints = new List<TrackPoint>
                            {
                                new TrackPoint(),
                                new TrackPoint(),
                                new TrackPoint()
                            }
                        }
                    }
                };

                await dbContext.AddAsync(gpxTrack, CancellationToken.None);
                await dbContext.SaveChangesAsync(CancellationToken.None);
            }
        }

        private static async Task RunMigrationsIfNeeded(GpxDbContext dbContext)
        {
            if (dbContext.Database.IsSqlServer())
            {
                await dbContext.Database.MigrateAsync();
            }
        }
    }
}
