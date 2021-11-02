using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Heracles.Domain;

namespace Heracles.Application.GpxTrackAggregate
{
    public class GpxTrackPoint : BaseEntity
    {
        public virtual double Latitude { get; set; }
        public virtual double Longitude { get; set; }
        public virtual double? Elevation { get; set; }
        public virtual DateTime? Time { get; set; }
        public virtual int TrackSegmentId { get; set; }

    }
}
