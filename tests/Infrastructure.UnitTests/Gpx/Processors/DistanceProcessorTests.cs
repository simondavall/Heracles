using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Heracles.Application.TrackAggregate;
using Heracles.Infrastructure.Gpx.Processors;
using NUnit.Framework;

namespace Heracles.Infrastructure.UnitTests.Gpx.Processors
{
    public class DistanceProcessorTests
    {
        [Test]
        public void TrackDistance_NullSegments_ReturnsZeroDistance()
        {
            var result = DistanceProcessor.TrackDistance(null);

            result.Should().Be(0);
        }
        [Test]
        public void TrackDistance_ZeroSegments_ReturnsZeroDistance()
        {
            var result = DistanceProcessor.TrackDistance(Enumerable.Empty<TrackSegment>());

            result.Should().Be(0);
        }
        [Test]
        public void TrackDistance_SegmentsDistanceZero_ReturnsZeroDistance()
        {
            var segments = new List<TrackSegment>()
            {
                new() {Distance = 0},
                new() {Distance = 0}
            };

            var result = DistanceProcessor.TrackDistance(segments);

            result.Should().Be(0);
        }
        [Test]
        public void TrackDistance_SegmentsPositiveDistance_ReturnsTotalDistance()
        {
            var segments = new List<TrackSegment>()
            {
                new() {Distance = 10},
                new() {Distance = 15}
            };

            var result = DistanceProcessor.TrackDistance(segments);

            result.Should().Be(25);
        }

        [Test]
        public void SegmentDistance_NullTrackPoints_ReturnsZeroDistance()
        {
            var result = DistanceProcessor.SegmentDistance(null);

            result.Should().Be(0);
        }

        [Test]
        public void SegmentDistance_EmptyTrackPoints_ReturnsZeroDistance()
        {
            var result = DistanceProcessor.SegmentDistance(Enumerable.Empty<TrackPoint>());

            result.Should().Be(0);
        }

        [Test]
        public void SegmentDistance_OneTrackPoint_ReturnsZeroDistance()
        {
            var trackPoints = new List<TrackPoint>
            {
                new TrackPoint()
            };

            var result = DistanceProcessor.SegmentDistance(trackPoints);

            result.Should().Be(0);
        }

        [Test]
        public void SegmentDistance_TwoTrackPoints_ReturnsPositiveDistance()
        {
            var trackPoints = new List<TrackPoint>
            {
                new(){Latitude = 0, Longitude = 0},
                new(){Latitude = 0.1, Longitude = 0.1}
            };

            var result = DistanceProcessor.SegmentDistance(trackPoints);

            result.Should().BeGreaterThan(0);
        }

        [Test]
        public void SegmentDistance_OutwardAndReturn_ReturnsTwiceOutwardDistance()
        {
            var trackPoints2 = new List<TrackPoint>
            {
                new(){Latitude = 0, Longitude = 0},
                new(){Latitude = 0.1, Longitude = 0.1}
            };

            var trackPoints3 = new List<TrackPoint>
            {
                new(){Latitude = 0, Longitude = 0},
                new(){Latitude = 0.1, Longitude = 0.1},
                new(){Latitude = 0, Longitude = 0}
            };

            var result2 = DistanceProcessor.SegmentDistance(trackPoints2);
            var result3 = DistanceProcessor.SegmentDistance(trackPoints3);

            result3.Should().Be(result2 * 2);
        }
    }
}
