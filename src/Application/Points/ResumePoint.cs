using Heracles.Application.Data;

namespace Heracles.Application.Points
{
    public class ResumePoint : Point
    {
        public ResumePoint(TrackPoint currentPoint, TrackPoint prevPoint, TrackPoint startPoint) : base(currentPoint, prevPoint, startPoint) {
            Type = "ResumePoint";
        }

        public override double DeltaPause => (CurrentPoint.Time - PrevPoint.Time).TotalMilliseconds;

        public override double DeltaTime => 0;
    }
}