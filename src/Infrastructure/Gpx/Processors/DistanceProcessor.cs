using System;
using System.Collections.Generic;
using Heracles.Application.TrackAggregate;

namespace Heracles.Infrastructure.Gpx.Processors
{
    public static class DistanceProcessor
    {
        internal static double SegmentDistance(IList<TrackPoint> trackPoints)
        {
            double distance = 0;
            TrackPoint previousTrackPoint = null;

            foreach (var trackPoint in trackPoints)
            {
                if (previousTrackPoint != null)
                {
                    distance += GetDistanceFromLatLonInKm(
                        previousTrackPoint.Latitude,
                        previousTrackPoint.Longitude,
                        trackPoint.Latitude,
                        trackPoint.Longitude);
                }
                previousTrackPoint = trackPoint;
            }

            return distance;
        }

        internal static double SessionDistance(ICollection<TrackSegment> trackSegments)
        {
            double distance = 0;
            if (trackSegments != null && trackSegments.Count > 0)
            {
                foreach (var trackSegment in trackSegments)
                {
                    distance += trackSegment.Distance;
                }
            }
            return Math.Round(distance, 2);
        }

        private static double GetDistanceFromLatLonInKm(double lat1, double lon1, double lat2, double lon2)
        {
            var earthRadius = 6371; // Radius of the earth in km
            var dLat = DegreesToRadians(lat2 - lat1);
            var dLon = DegreesToRadians(lon2 - lon1);
            var a =
                    Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                    Math.Cos(DegreesToRadians(lat1)) * Math.Cos(DegreesToRadians(lat2)) *
                    Math.Sin(dLon / 2) * Math.Sin(dLon / 2)
                ;

            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            var distance = earthRadius * c; // Distance in km
            return distance;
        }

        private static double DegreesToRadians(double degrees)
        {
            return degrees * (Math.PI / 180);
        }

    }
}
