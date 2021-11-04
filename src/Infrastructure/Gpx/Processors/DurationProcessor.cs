using System;
using System.Collections.Generic;
using Heracles.Application.TrackAggregate;

namespace Heracles.Infrastructure.Gpx.Processors
{
    public static class DurationProcessor
    {
        internal static TimeSpan SegmentDuration(IList<TrackPoint> trackPoints)
        {
            var duration = TimeSpan.Zero;
            if (trackPoints.Count > 1)
            {
                duration = trackPoints[^1].Time - trackPoints[0].Time;
            }
            return duration;
        }

        internal static TimeSpan SessionDuration(ICollection<TrackSegment> trackSegments)
        {
            var duration = TimeSpan.Zero;

            if (trackSegments is not {Count: > 0}) 
                return duration;

            foreach (var trackSegment in trackSegments)
            {
                duration += trackSegment.Duration;
            }
            return duration;
        }

    }
}
