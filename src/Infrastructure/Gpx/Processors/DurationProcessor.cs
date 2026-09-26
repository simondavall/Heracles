using Heracles.Application.Data;

namespace Heracles.Infrastructure.Gpx.Processors
{
    public static class DurationProcessor
    {
        internal static TimeSpan SegmentDuration(IList<TrackPoint> trackPoints)
        {
            if (trackPoints.Count == 0)
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

        internal static TimeSpan TrackDuration(IList<TrackSegment> trackSegments)
        {
            if (trackSegments.Count == 0)
            {
                return TimeSpan.Zero;
            }

            return trackSegments.Aggregate(TimeSpan.Zero, (current, trackSegment) => current + trackSegment.Duration);
        }

    }
}
