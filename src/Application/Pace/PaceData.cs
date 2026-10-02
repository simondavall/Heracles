using Heracles.Application.Data;

namespace Heracles.Application.Pace;

public sealed class PaceData
{
    public Guid TrackId { get; set; }
    public int TrackPointId { get; set; }
    public int Seq { get; set; }
    public double CumulativeDistance { get; set; }
    public int CumulativeTime { get; set; }
    public Track Track { get; set; } = null!;
    public TrackPoint TrackPoint { get; set; } = null!;
}