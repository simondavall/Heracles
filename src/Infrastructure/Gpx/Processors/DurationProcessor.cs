using System;
using System.Collections.Generic;
using System.Linq;
using Heracles.Application.TrackAggregate;

namespace Heracles.Infrastructure.Gpx.Processors
{
    public static class DurationProcessor
    {
        internal static TimeSpan SegmentDuration(IList<TrackPoint> trackPoints)
        {
            if (trackPoints is null)
            {
                return TimeSpan.Zero;
            }

            var duration = TimeSpan.Zero;

            if (trackPoints.Count > 1)
            {
                duration = trackPoints[^1].Time - trackPoints[0].Time;
            }
            return duration;
        }

        internal static TimeSpan TrackDuration(IEnumerable<TrackSegment> trackSegments)
        {
            if (trackSegments is null)
            {
                return TimeSpan.Zero;
            }

            return trackSegments.Aggregate(TimeSpan.Zero, (current, trackSegment) => current + trackSegment.Duration);
        }

    }
}
