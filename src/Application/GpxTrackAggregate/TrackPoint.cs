using System;
using Heracles.Domain;

namespace Heracles.Application.GpxTrackAggregate
{
    public class TrackPoint : BaseEntity
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double Elevation { get; set; }
        public DateTime Time { get; set; } = DateTime.UtcNow;
    }
}
