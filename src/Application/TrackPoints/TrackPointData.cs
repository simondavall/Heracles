namespace Heracles.Application.TrackPoints;

public sealed class TrackPointData
{
    public Guid TrackId { get; set; }
    public int TrackPointId { get; set; }
    public int Seq { get; set; }
    public double CumulativeDistance { get; set; }
    public int CumulativeTime { get; set; }
    public double Elevation { get; set; }
}