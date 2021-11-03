using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dlg.Krakow.Gpx;
using Heracles.Application.GpxTrackAggregate;
using Heracles.Application.Interfaces;
using Heracles.Domain.Interfaces;
using Heracles.Infrastructure.Gpx.Processors;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Heracles.Infrastructure.Gpx
{
    public class GpxService : IGpxService
    {
        private readonly IRepository<TrackAggregate> _gpxRepository;
        private readonly ILogger<GpxService> _logger;

        public GpxService(IRepository<TrackAggregate> gpxRepository, ILogger<GpxService> logger)
        {
            _gpxRepository = gpxRepository;
            _logger = logger;
        }

        public async Task<TrackAggregate> LoadLoadContentsOfGpxFile(IFormFile file)
        {
            try
            {
                var gpxTrack = GpxEngine.GetGpxTrackFromFile(file);
                if (gpxTrack != null)
                {
                    var track = CreateTrackAggregate(gpxTrack);
                    return track;
                }
            }
            catch (Exception e)
            {
                //todo throw custom exception. Declare exception in App/Domain. Put e as inner exception.
                _logger.LogError(e, $"GpxService Failed to create TrackAggregate for file {file.FileName} with message: {e.Message}");
            }

            return null;
        }

        private static TrackAggregate CreateTrackAggregate(GpxTrack track)
        {
            var trackAggregate = new TrackAggregate()
            {
                Name = track.Name,
                Time = track.Time ?? DateTime.Now
            };

            ICollection<TrackSegment> trackSegments = new List<TrackSegment>();

            foreach (var gpxTrackSegment in track.Segments)
            {
                trackSegments.Add(CreateTrackSegment(gpxTrackSegment));
            }

            trackAggregate.Elevation = ElevationProcessor.SessionElevation(trackSegments);
            trackAggregate.Duration = DurationProcessor.SessionDuration(trackSegments);
            trackAggregate.Distance = DistanceProcessor.SessionDistance(trackSegments);
            trackAggregate.Calories = CaloriesProcessor.GetCaloriesBurned(trackAggregate);
            trackAggregate.Pace = PaceProcessor.GetAveragePace(trackAggregate);
            trackAggregate.ActivityType = ActivityProcessor.GetActivityType(trackAggregate);
            trackAggregate.Speed = SpeedProcessor.GetAverageSpeed(trackAggregate);
            trackAggregate.TrackSegments = trackSegments;

            return trackAggregate;
        }

        private static TrackSegment CreateTrackSegment(GpxTrackSegment gpxTrackSegment)
        {
            var trackSegment = new TrackSegment();
            var trackPoints = new List<TrackPoint>();
            foreach (var point in gpxTrackSegment.TrackPoints)
            {
                trackPoints.Add(CreateTrackPoint(point));
            }
            trackSegment.TrackPoints = trackPoints;
            trackSegment.Elevation = ElevationProcessor.SegmentElevation(trackPoints);
            trackSegment.Duration = DurationProcessor.SegmentDuration(trackPoints);
            trackSegment.Distance = DistanceProcessor.SegmentDistance(trackPoints);

            return trackSegment;
        }

        private static TrackPoint CreateTrackPoint(GpxTrackPoint gpxTrackPoint)
        {
            var trackPoint = new TrackPoint
            {
                Time = gpxTrackPoint.Time ?? DateTime.Now,
                Elevation = gpxTrackPoint.Elevation ?? 0,
                Latitude = gpxTrackPoint.Latitude,
                Longitude = gpxTrackPoint.Longitude
            };
            return trackPoint;
        }
    }
}
