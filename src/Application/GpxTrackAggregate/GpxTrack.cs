using System;
using System.Collections.Generic;
using Heracles.Domain;
using Heracles.Domain.Interfaces;

namespace Heracles.Application.GpxTrackAggregate
{
    public class GpxTrack : BaseEntity, IAggregateRoot
    {
        public virtual string Name { get; set; }
        public virtual DateTime Time { get; set; } = DateTime.UtcNow;
        public virtual double Distance { get; set; }
        public virtual TimeSpan Duration { get; set; } = TimeSpan.Zero;
        public virtual ActivityType ActivityType { get; set; } = ActivityType.Unknown;
        public virtual double Elevation { get; set; }
        public virtual int Calories { get; set; }
        public virtual TimeSpan Pace { get; set; } = TimeSpan.Zero;
        public virtual double Speed { get; set; }

        public virtual ICollection<GpxTrackSegment> TrackSegments { get; set; }
    }
}
