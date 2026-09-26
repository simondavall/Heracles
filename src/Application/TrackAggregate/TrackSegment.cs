using System;
using System.Collections.Generic;

namespace Heracles.Application.TrackAggregate
{
    public class TrackSegment
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public int Seq { get; set; }
        public double Distance { get; set; }
        public TimeSpan Duration { get; set; } = TimeSpan.Zero;
        public double Elevation { get; set; }
        public int Calories { get; set; }
        public IList<TrackPoint> TrackPoints { get; set; }
        public Guid TrackId { get; set; }
    }
}
