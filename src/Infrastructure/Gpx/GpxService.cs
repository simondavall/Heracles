using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dlg.Krakow.Gpx;
using Heracles.Application.Interfaces;
using Heracles.Application.TrackAggregate;
using Heracles.Infrastructure.Gpx.Processors;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Heracles.Infrastructure.Gpx
{
    public class GpxService : IGpxService
    {
        private readonly ILogger<GpxService> _logger;

        public GpxService(ILogger<GpxService> logger)
        {
            _logger = logger;
        }

        public async Task<Track> LoadLoadContentsOfGpxFile(IFormFile file)
        {
            try
            {
                // TODO Convert this to async/await
                var gpxTrack = GpxEngine.GetGpxTrackFromFile(file);
                if (gpxTrack != null)
                {
                    var track = CreateTrackAggregate(gpxTrack);
                    return track;
                }
            }
            catch (Exception e)
            {
                //TODO throw custom exception. Declare exception in App/Domain. Put e as inner exception.
                _logger.LogError(e, $"GpxService Failed to create TrackAggregate for file {file.FileName} with message: {e.Message}");
            }

            return null;
        }

        private static Track CreateTrackAggregate(GpxTrack gpxTrack)
        {
            var track = new Track()
            {
                Name = gpxTrack.Name,
                Time = gpxTrack.Time ?? DateTime.Now
            };

            ICollection<TrackSegment> trackSegments = new List<TrackSegment>();

            foreach (var gpxTrackSegment in gpxTrack.Segments)
            {
                trackSegments.Add(CreateTrackSegment(gpxTrackSegment, track.Id));
            }

            track.Elevation = ElevationProcessor.SessionElevation(trackSegments);
            track.Duration = DurationProcessor.SessionDuration(trackSegments);
            track.Distance = DistanceProcessor.SessionDistance(trackSegments);
            track.Calories = CaloriesProcessor.GetCaloriesBurned(track);
            track.Pace = PaceProcessor.GetAveragePace(track);
            track.ActivityType = ActivityProcessor.GetActivityType(track);
            track.Speed = SpeedProcessor.GetAverageSpeed(track);
            track.TrackSegments = trackSegments;

            return track;
        }

        private static TrackSegment CreateTrackSegment(GpxTrackSegment gpxTrackSegment, Guid trackId)
        {
            var trackSegment = new TrackSegment();
            var trackPoints = new List<TrackPoint>();
            foreach (var point in gpxTrackSegment.TrackPoints)
            {
                trackPoints.Add(CreateTrackPoint(point, trackSegment.Id));
            }

            trackSegment.TrackId = trackId;
            trackSegment.TrackPoints = trackPoints;
            trackSegment.Elevation = ElevationProcessor.SegmentElevation(trackPoints);
            trackSegment.Duration = DurationProcessor.SegmentDuration(trackPoints);
            trackSegment.Distance = DistanceProcessor.SegmentDistance(trackPoints);

            return trackSegment;
        }

        private static TrackPoint CreateTrackPoint(GpxPoint gpxTrackPoint, Guid trackSegmentId)
        {
            var trackPoint = new TrackPoint
            {
                Time = gpxTrackPoint.Time ?? DateTime.Now,
                Elevation = gpxTrackPoint.Elevation ?? 0,
                Latitude = gpxTrackPoint.Latitude,
                Longitude = gpxTrackPoint.Longitude,
                TrackSegmentId = trackSegmentId
            };
            return trackPoint;
        }
    }
}
