using System.Collections.Generic;
using Heracles.Application.Data;

namespace Heracles.Application.Points
{
    public interface IPointService
    {
        List<Point> GetPoints(Track track);
    }
}
