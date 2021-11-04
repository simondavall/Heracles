using System;
using Heracles.Application.GpxTrackAggregate;

namespace Heracles.Infrastructure.Gpx.Processors
{
    public static class PaceProcessor
    {
        public static TimeSpan GetAveragePace(TrackAggregate trackAggregate)
        {
            var duration = trackAggregate.Duration;
            if (duration == TimeSpan.Zero)
            {
                return TimeSpan.Zero; // do not set average pace if no duration
            }
            return TimeSpan.FromMinutes(duration.TotalMinutes / trackAggregate.Distance);
        }
    }
}
