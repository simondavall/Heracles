using System;
using Heracles.Application.TrackAggregate;

namespace Heracles.Infrastructure.Gpx.Processors
{
    public static class SpeedProcessor
    {
        public static double GetAverageSpeed(Track track)
        {
            if (track.Duration == TimeSpan.Zero || track.Distance <= 0)
            {
                return 0; // do not set average speed if no duration.
            }
            return track.Distance / track.Duration.TotalHours;
        }
    }
}
