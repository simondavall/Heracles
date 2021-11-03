using System;
using Heracles.Application.GpxTrackAggregate;

namespace Heracles.Infrastructure.Gpx.Processors
{
    public static class SpeedProcessor
    {
        public static double GetAverageSpeed(TrackAggregate trackAggregate)
        {
            var duration = trackAggregate.Duration;
            if (duration == TimeSpan.Zero)
            {
                return 0; // do not set average speed if no duration.
            }
            return trackAggregate.Distance / duration.TotalHours;
        }
    }
}
