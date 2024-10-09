using Heracles.Application.TrackAggregate;

namespace Heracles.Application.Entities.Points
{
    public class InterimPoint : Point
    {
        public InterimPoint(TrackPoint currentPoint, TrackPoint prevPoint, TrackPoint startPoint) : base(currentPoint, prevPoint, startPoint)
        {
            Type = "TrackPoint";
        }
    }
}
