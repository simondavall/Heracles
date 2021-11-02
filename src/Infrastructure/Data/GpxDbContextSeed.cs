using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Heracles.Application.GpxTrackAggregate;
using Heracles.Domain.Interfaces;

namespace Heracles.Infrastructure.Data
{
    public class GpxDbContextSeed
    {
        public static async Task SeedAsync(GpxDbContext dbContext)
        {
            var gpxTrack = new GpxTrack
            {
                Name = "Testing Track",
                Time = DateTime.Now,
                Distance = 0,
                Duration = TimeSpan.Zero,
                ActivityType = ActivityType.Unknown,
                Elevation = 0,
                Calories = 0,
                Pace = TimeSpan.Zero,
                Speed = 0,
                TrackSegments = new List<GpxTrackSegment>
                {
                    new GpxTrackSegment
                    {
                        Distance = 0,
                        Duration = TimeSpan.Zero,
                        Elevation = 0,
                        Calories = 0,
                        TrackPoints = new List<GpxTrackPoint>
                        {
                            new GpxTrackPoint
                            {
                                Latitude = 0,
                                Longitude = 0,
                                Time = DateTime.Now
                            },
                            new GpxTrackPoint
                            {
                                Latitude = 0,
                                Longitude = 0,
                                Time = DateTime.Now
                            },
                            new GpxTrackPoint
                            {
                                Latitude = 0,
                                Longitude = 0,
                                Time = DateTime.Now
                            }
                        }

                    }, 
                    new GpxTrackSegment
                    {
                        Distance = 0,
                        Duration = TimeSpan.Zero,
                        Elevation = 0,
                        Calories = 0,
                        TrackPoints = new List<GpxTrackPoint>
                        {
                            new GpxTrackPoint
                            {
                                Latitude = 0,
                                Longitude = 0,
                                Time = DateTime.Now
                            },
                            new GpxTrackPoint
                            {
                                Latitude = 0,
                                Longitude = 0,
                                Time = DateTime.Now
                            },
                            new GpxTrackPoint
                            {
                                Latitude = 0,
                                Longitude = 0,
                                Time = DateTime.Now
                            }
                        }
                    }
                }
            };
             
            await dbContext.AddAsync(gpxTrack, CancellationToken.None);
            await dbContext.SaveChangesAsync(CancellationToken.None);
        }
    }
}
