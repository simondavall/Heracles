using System;
using System.Collections.Generic;
using Heracles.Domain;

namespace Heracles.Application.GpxTrackAggregate
{
    public class TrackSegment : BaseEntity
    {
        public double Distance { get; set; }
        public TimeSpan Duration { get; set; } = TimeSpan.Zero;
        public double Elevation { get; set; }
        public int Calories { get; set; }
        public ICollection<TrackPoint> TrackPoints { get; set; }
    }
}
