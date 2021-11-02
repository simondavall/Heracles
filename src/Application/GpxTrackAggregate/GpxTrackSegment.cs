using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Heracles.Domain;
using Heracles.Domain.Interfaces;

namespace Heracles.Application.GpxTrackAggregate
{
    public class GpxTrackSegment : BaseEntity
    {
        public virtual ICollection<GpxTrackPoint> TrackPoints { get; set; }

        public virtual int ExerciseSessionId { get; set; }

        public virtual double Distance { get; set; }
        public virtual TimeSpan? Duration { get; set; }
        public virtual double? Elevation { get; set; }
        public virtual int Calories { get; set; }
    }
}
