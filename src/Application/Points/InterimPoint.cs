using Heracles.Application.Data;

namespace Heracles.Application.Points
{
    public class InterimPoint : Point
    {
        public InterimPoint(TrackPoint currentPoint, TrackPoint prevPoint, TrackPoint startPoint) : base(currentPoint, prevPoint, startPoint) {
            Type = "TrackPoint";
        }
    }
}