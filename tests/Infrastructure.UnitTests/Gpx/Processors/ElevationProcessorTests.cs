using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Heracles.Application.TrackAggregate;
using Heracles.Infrastructure.Gpx.Processors;
using NUnit.Framework;

namespace Heracles.Infrastructure.UnitTests.Gpx.Processors
{
    public class ElevationProcessorTests
    {
        [Test]
        public void SegmentElevation_NullTrackPoints_ReturnsZeroElevation()
        {
            var result = ElevationProcessor.SegmentElevation(null);

            result.Should().Be(0);
        }

        [Test]
        public void SegmentElevation_EmptyTrackPoints_ReturnsZeroElevation()
        {
            var result = ElevationProcessor.SegmentElevation(Enumerable.Empty<TrackPoint>());

            result.Should().Be(0);
        }

        [Test]
        public void SegmentElevation_OneTrackPoint_ReturnsZeroElevation()
        {
            var trackPoints = new List<TrackPoint>()
            {
                new() {Elevation = 100}
            };
            var result = ElevationProcessor.SegmentElevation(trackPoints);

            result.Should().Be(0);
        }

        [Test]
        public void SegmentElevation_TwoTrackPoints_ReturnsElevationDifference()
        {
            var trackPoints = new List<TrackPoint>()
            {
                new() {Elevation = 100},
                new() {Elevation = 120}
            };
            var result = ElevationProcessor.SegmentElevation(trackPoints);

            result.Should().Be(20);
        }

        [Test]
        public void SegmentElevation_ThreeTrackPoints_ReturnsOnlyPositiveElevationDifference()
        {
            var trackPoints = new List<TrackPoint>()
            {
                new() {Elevation = 100},
                new() {Elevation = 120},
                new() {Elevation = 110}
            };
            var result = ElevationProcessor.SegmentElevation(trackPoints);

            result.Should().Be(20);
        }

        [Test]
        public void SegmentElevation_FourTrackPoints_ReturnsOnlyPositiveElevationDifference()
        {
            var trackPoints = new List<TrackPoint>()
            {
                new() {Elevation = 100},
                new() {Elevation = 120},
                new() {Elevation = 110},
                new() {Elevation = 115}
            };
            var result = ElevationProcessor.SegmentElevation(trackPoints);

            result.Should().Be(25);
        }

        [Test]
        public void SegmentElevation_FourTrackPointsWithNegative_ReturnsOnlyPositiveElevationDifference()
        {
            var trackPoints = new List<TrackPoint>()
            {
                new() {Elevation = -10},
                new() {Elevation = -5}, // +5
                new() {Elevation = 15}, // +20
                new() {Elevation = 10},
                new() {Elevation = 50} // +40 = Total 65m
            };
            var result = ElevationProcessor.SegmentElevation(trackPoints);

            result.Should().Be(65);
        }

        [Test]
        public void TrackElevation_NullTrackSegments_ReturnsZeroElevation()
        {
            var result = ElevationProcessor.TrackElevation(null);

            result.Should().Be(0);
        }

        [Test]
        public void TrackElevation_EmptyTrackSegments_ReturnsZeroElevation()
        {
            var result = ElevationProcessor.TrackElevation(new List<TrackSegment>());

            result.Should().Be(0);
        }

        [Test]
        public void TrackElevation_OneTrackSegmentWithZeroElevation_ReturnsZeroElevation()
        {
            var segments = new List<TrackSegment>()
            {
                new() {Elevation = 0}
            };
            var result = ElevationProcessor.TrackElevation(segments);

            result.Should().Be(0);
        }

        [Test]
        public void TrackElevation_TwoTrackSegmentWithPositiveElevation_ReturnsPositiveElevation()
        {
            var segments = new List<TrackSegment>()
            {
                new() {Elevation = 0},
                new() {Elevation = 10}
            };
            var result = ElevationProcessor.TrackElevation(segments);

            result.Should().Be(10);
        }
    }
}
