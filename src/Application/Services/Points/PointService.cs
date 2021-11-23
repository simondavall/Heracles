using System.Collections.Generic;
using Heracles.Application.Entities.Points;
using Heracles.Application.Interfaces;
using Heracles.Application.TrackAggregate;

namespace Heracles.Application.Services.Points
{
    public class PointService : IPointService
    {
        public List<Point> GetPoints(Track track)
        {
            var points = new List<Point>();
            var startPoint = track.TrackSegments[0].TrackPoints[0];
            TrackPoint prevPoint = null;
            for (var i = 0; i < track.TrackSegments.Count; i++)
            {
                for (var j = 0; j < track.TrackSegments[i].TrackPoints.Count; j++)
                {
                    var currentPoint = track.TrackSegments[i].TrackPoints[j];
                    var createPointArgs = new CreatePointArgs
                    {
                        CurrentSegmentIndex = i,
                        CurrentPointIndex = j,
                        LastSegmentIndex = track.TrackSegments.Count - 1,
                        LastPointInSegmentIndex = track.TrackSegments[i].TrackPoints.Count - 1,
                        CurrentPoint = track.TrackSegments[i].TrackPoints[j],
                        PreviousPoint = prevPoint,
                        StartPoint = startPoint
                    };

                    var point = Point.CreatePoint(createPointArgs);
                    points.Add(point);

                    prevPoint = currentPoint;
                }
            }
            
            return points;
        }
    }
}
