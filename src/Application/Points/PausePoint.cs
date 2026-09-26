using Heracles.Application.Data;

namespace Heracles.Application.Points
{
    public class PausePoint : Point
    {
        public PausePoint(TrackPoint currentPoint, TrackPoint prevPoint, TrackPoint startPoint) : base(currentPoint, prevPoint, startPoint) {
            Type = "PausePoint";
        }
    }
}