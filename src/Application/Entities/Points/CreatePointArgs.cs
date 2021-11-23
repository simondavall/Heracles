using Heracles.Application.TrackAggregate;

namespace Heracles.Application.Entities.Points
{
    public class CreatePointArgs
    {
        public int CurrentSegmentIndex { get; init; }
        public int CurrentPointIndex { get; init; }
        public int LastSegmentIndex { get; init; }
        public int LastPointInSegmentIndex { get; init; }
        public TrackPoint CurrentPoint { get; init; }
        public TrackPoint PreviousPoint { get; init; }
        public TrackPoint StartPoint { get; init; }
    }
}
