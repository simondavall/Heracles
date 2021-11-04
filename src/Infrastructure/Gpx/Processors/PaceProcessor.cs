using System;
using Heracles.Application.TrackAggregate;

namespace Heracles.Infrastructure.Gpx.Processors
{
    public static class PaceProcessor
    {
        public static TimeSpan GetAveragePace(Track trackAggregate)
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
