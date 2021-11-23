using System.Collections.Generic;
using Heracles.Application.Entities.Points;
using Heracles.Application.TrackAggregate;

namespace Heracles.Application.Interfaces
{
    public interface IPointService
    {
        List<Point> GetPoints(Track track);
    }
}
