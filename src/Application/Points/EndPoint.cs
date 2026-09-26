using Heracles.Application.Data;

namespace Heracles.Application.Points
{
    public class EndPoint : Point
    {
        public EndPoint(TrackPoint currentPoint, TrackPoint prevPoint, TrackPoint startPoint) : base(currentPoint, prevPoint, startPoint) {
            Type = "EndPoint";
        }
    }
}