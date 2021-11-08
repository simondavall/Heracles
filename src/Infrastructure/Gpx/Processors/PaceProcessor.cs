using System;
using Heracles.Application.TrackAggregate;

namespace Heracles.Infrastructure.Gpx.Processors
{
    public static class PaceProcessor
    {
        public static TimeSpan GetAveragePace(Track track)
        {
            if (track.Duration <= TimeSpan.Zero || track.Distance <= 0)
            {
                return TimeSpan.Zero; // do not set average pace if no duration
            }
            return TimeSpan.FromMinutes(track.Duration.TotalMinutes / track.Distance);
        }
    }
}
