using System;
using System.Collections.Generic;
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

        public Track LoadContentsOfGpxFile(IFormFile file)
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
                throw;
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

            IList<TrackSegment> trackSegments = new List<TrackSegment>();
            var segmentSequenceIndex = 0;
            foreach (var gpxTrackSegment in gpxTrack.Segments)
            {
                trackSegments.Add(CreateTrackSegment(gpxTrackSegment, track.Id, segmentSequenceIndex));
                segmentSequenceIndex++;
            }

            track.Elevation = ElevationProcessor.TrackElevation(trackSegments);
            track.Duration = DurationProcessor.TrackDuration(trackSegments);
            track.Distance = DistanceProcessor.TrackDistance(trackSegments);
            track.Calories = CaloriesProcessor.GetCaloriesBurned(track);
            track.Pace = PaceProcessor.GetAveragePace(track);
            track.ActivityType = ActivityProcessor.GetActivityType(track);
            track.Speed = SpeedProcessor.GetAverageSpeed(track);
            track.TrackSegments = trackSegments;

            return track;
        }

        private static TrackSegment CreateTrackSegment(GpxTrackSegment gpxTrackSegment, Guid trackId, int sequenceIndex)
        {
            var trackSegment = new TrackSegment { Seq = sequenceIndex };
            var trackPoints = new List<TrackPoint>();
            var pointSequenceIndex = 0;
            foreach (var point in gpxTrackSegment.TrackPoints)
            {
                trackPoints.Add(CreateTrackPoint(point, trackSegment.Id, pointSequenceIndex));
                pointSequenceIndex++;
            }

            trackSegment.TrackId = trackId;
            trackSegment.TrackPoints = trackPoints;
            trackSegment.Elevation = ElevationProcessor.SegmentElevation(trackPoints);
            trackSegment.Duration = DurationProcessor.SegmentDuration(trackPoints);
            trackSegment.Distance = DistanceProcessor.SegmentDistance(trackPoints);

            return trackSegment;
        }

        private static TrackPoint CreateTrackPoint(GpxPoint gpxTrackPoint, Guid trackSegmentId, int sequenceIndex)
        {
            var trackPoint = new TrackPoint
            {   
                Seq = sequenceIndex,
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
