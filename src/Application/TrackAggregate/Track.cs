using System;
using System.Collections.Generic;
using Heracles.Domain;
using Heracles.Domain.Interfaces;

namespace Heracles.Application.TrackAggregate
{
    public class Track : BaseEntity<Guid>, IAggregateRoot
    {
        public Track()
        {
            Id = Guid.NewGuid();
        }

        public string Name { get; set; }
        public DateTime Time { get; set; } = DateTime.UtcNow;
        public double Distance { get; set; }
        public TimeSpan Duration { get; set; } = TimeSpan.Zero;
        public ActivityType ActivityType { get; set; } = ActivityType.Unknown;
        public double Elevation { get; set; }
        public int Calories { get; set; }
        public TimeSpan Pace { get; set; } = TimeSpan.Zero;
        public double Speed { get; set; }

        public ICollection<TrackSegment> TrackSegments { get; set; }
    }
}
