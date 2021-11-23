using Heracles.Application.TrackAggregate;

namespace Heracles.Application.Entities.Points
{
    public class EndPoint : Point
    {
        public EndPoint(TrackPoint currentPoint, TrackPoint prevPoint, TrackPoint startPoint) : base(currentPoint, prevPoint, startPoint)
        {
            Type = "EndPoint";
        }
    }
}
