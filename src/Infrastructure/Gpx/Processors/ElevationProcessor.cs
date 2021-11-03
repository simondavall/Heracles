using System;
using System.Collections.Generic;
using System.Linq;
using Heracles.Application.GpxTrackAggregate;

namespace Heracles.Infrastructure.Gpx.Processors
{
    public static class ElevationProcessor
    {
        internal static double SegmentElevation(IEnumerable<TrackPoint> trackPoints)
        {
            double elevation = 0;
            TrackPoint previousTrackPoint = null;

            foreach (var trackPoint in trackPoints)
            {
                if (previousTrackPoint != null && previousTrackPoint.Elevation < trackPoint.Elevation)
                    elevation += Round(trackPoint.Elevation - previousTrackPoint.Elevation, 4);
                previousTrackPoint = trackPoint;
            }

            return elevation;
        }


        internal static double SessionElevation(ICollection<TrackSegment> trackSegments)
        {
            double elevation = 0;
            if (trackSegments is not {Count: > 0}) 
                return elevation;

            return trackSegments.Sum(trackSegment => Round(trackSegment.Elevation, 4));
        }


        private static double Round(double? amount, int places)
        {
            var doubleAmount = amount ?? 0;
            return Math.Round(doubleAmount, places);
        }

    }
}
