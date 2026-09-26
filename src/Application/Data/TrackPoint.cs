using System;

namespace Heracles.Application.Data
{
    public class TrackPoint
    {
        public int Id { get; set; }
        public int Seq { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double Elevation { get; set; }
        public DateTime Time { get; set; } = DateTime.UtcNow;
        public Guid TrackSegmentId { get; set; }
    }
}
