using System;
using Heracles.Domain;
using Microsoft.EntityFrameworkCore;

namespace Heracles.Application.TrackAggregate
{
    public class TrackPoint : BaseEntity<int>
    {
        public int Seq { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double Elevation { get; set; }
        public DateTime Time { get; set; } = DateTime.UtcNow;
        public Guid TrackSegmentId { get; set; }
    }
}
