using System;
using Ardalis.Specification;
using Heracles.Application.TrackAggregate;


namespace Heracles.Application.Specifications
{
    public class ActivitiesByDateSpec : Specification<Track>
    {
        public ActivitiesByDateSpec(DateTime startDate, DateTime endDate)
        {
            Query.Where(t => t.Time > startDate & t.Time < endDate)
                .OrderByDescending(t => t.Time);
        }
    }
}
