using Heracles.Application.TrackAggregate;
using static System.Math;

namespace Heracles.Application.Entities.Points
{
    public abstract class Point
    {
        protected readonly TrackPoint CurrentPoint;
        protected readonly TrackPoint PrevPoint;
        private readonly TrackPoint _startPoint;

        protected Point(TrackPoint currentPoint, TrackPoint prevPoint, TrackPoint startPoint)
        {
            CurrentPoint = currentPoint;
            PrevPoint = prevPoint;
            _startPoint = startPoint;
        }

        public double Accuracy { get; } = 0;

        public double Altitude => CurrentPoint.Elevation;

        public virtual double DeltaDistance => GetDistanceBetweenLatLongInMeters(CurrentPoint.Latitude, CurrentPoint.Longitude, 
            PrevPoint.Latitude, PrevPoint.Longitude);
        public virtual double DeltaPause { get; } = 0;
        public virtual double DeltaTime => (CurrentPoint.Time - PrevPoint.Time).TotalMilliseconds;
        public double Latitude => CurrentPoint.Latitude;
        public double Longitude => CurrentPoint.Longitude;
        public double Timestamp => (CurrentPoint.Time - _startPoint.Time).TotalMilliseconds;
        public string Type { get; protected init; }

        public static Point CreatePoint(CreatePointArgs args)
        {
            if (args.CurrentSegmentIndex == 0 && args.CurrentPointIndex == 0)
            {
                return new StartPoint(args.CurrentPoint, args.PreviousPoint, args.StartPoint);
            }

            if (args.CurrentSegmentIndex == args.LastSegmentIndex && args.CurrentPointIndex == args.LastPointInSegmentIndex)
            {
                return new EndPoint(args.CurrentPoint, args.PreviousPoint, args.StartPoint);
            }

            if (args.CurrentPointIndex == args.LastPointInSegmentIndex)
            {
                return new PausePoint(args.CurrentPoint, args.PreviousPoint, args.StartPoint);
            }

            if (args.CurrentPointIndex == 0)
            {
                return new ResumePoint(args.CurrentPoint, args.PreviousPoint, args.StartPoint);
            }

            return new InterimPoint(args.CurrentPoint, args.PreviousPoint, args.StartPoint);
        }

        private double GetDistanceBetweenLatLongInMeters(double lat1, double lon1, double lat2, double lon2)
        {
            const int radius = 6_371_000; // Radius of the earth in m
            var dLat = Deg2Rad(lat2 - lat1);
            var dLon = Deg2Rad(lon2 - lon1);
            var a =
                    Sin(dLat / 2) * Sin(dLat / 2) +
                    Cos(Deg2Rad(lat1)) * Cos(Deg2Rad(lat2)) *
                    Sin(dLon / 2) * Sin(dLon / 2)
                ;
            var c = 2 * Atan2(Sqrt(a), Sqrt(1 - a));
            var distance = radius * c; // Distance in m
            return distance;
        }

        private static double Deg2Rad(double deg)
        {
            return deg * (PI / 180);
        }
    }
}
