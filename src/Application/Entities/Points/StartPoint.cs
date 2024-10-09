using Heracles.Application.TrackAggregate;

namespace Heracles.Application.Entities.Points
{
    public class StartPoint : Point
    {
        public StartPoint(TrackPoint currentPoint, TrackPoint prevPoint, TrackPoint startPoint) : base(currentPoint, prevPoint, startPoint)
        {
            Type = "StartPoint";
        }

        public override double DeltaDistance => 0;

        public override double DeltaTime => 0;
    }
}
