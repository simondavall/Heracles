using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Heracles.Domain;
using Heracles.Domain.Interfaces;

namespace Heracles.Application.GpxTrackAggregate
{
    public class GpxTrack : BaseEntity, IAggregateRoot
    {
        public virtual string Name { get; set; }
        public virtual DateTime? Time { get; set; }
        public virtual ICollection<GpxTrackSegment> TrackSegments { get; set; }

        public virtual double Distance { get; set; }
        public virtual TimeSpan? Duration { get; set; }
        public virtual ActivityType ActivityType { get; set; }
        public virtual double? Elevation { get; set; }
        public virtual int Calories { get; set; }
        public virtual TimeSpan Pace { get; set; }
        public virtual double Speed { get; set; }
    }
}
