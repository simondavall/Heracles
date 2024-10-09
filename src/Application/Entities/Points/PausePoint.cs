using Heracles.Application.TrackAggregate;

namespace Heracles.Application.Entities.Points
{
    public class PausePoint : Point
    {
        public PausePoint(TrackPoint currentPoint, TrackPoint prevPoint, TrackPoint startPoint) : base(currentPoint, prevPoint, startPoint)
        {
            Type = "PausePoint";
        }
    }
}
