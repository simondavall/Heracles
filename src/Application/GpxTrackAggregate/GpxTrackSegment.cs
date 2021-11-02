using System;
using System.Collections.Generic;
using Heracles.Domain;

namespace Heracles.Application.GpxTrackAggregate
{
    public class GpxTrackSegment : BaseEntity
    {
        public virtual double Distance { get; set; }
        public virtual TimeSpan Duration { get; set; } = TimeSpan.Zero;
        public virtual double Elevation { get; set; }
        public virtual int Calories { get; set; }
        public virtual ICollection<GpxTrackPoint> TrackPoints { get; set; }
    }
}
