using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Heracles.Application.TrackAggregate;
using Heracles.Infrastructure.Gpx.Processors;
using NUnit.Framework;

namespace Heracles.Infrastructure.UnitTests.Gpx.Processors
{
    public class DurationProcessorTests
    {
        [Test]
        public void TrackDuration_NullSegments_ReturnsZeroDuration()
        {
            var result = DurationProcessor.TrackDuration(null);

            result.Should().Be(TimeSpan.Zero);
        }
        [Test]
        public void TrackDuration_ZeroSegments_ReturnsZeroDuration()
        {
            var result = DurationProcessor.TrackDuration(Enumerable.Empty<TrackSegment>());

            result.Should().Be(TimeSpan.Zero);
        }
        [Test]
        public void TrackDistance_SegmentsDistanceZero_ReturnsZeroDuration()
        {
            var segments = new List<TrackSegment>()
            {
                new() {Duration = TimeSpan.Zero},
                new() {Duration = TimeSpan.Zero}
            };

            var result = DurationProcessor.TrackDuration(segments);

            result.Should().Be(TimeSpan.Zero);
        }
        [Test]
        public void TrackDistance_SegmentsPositiveDuration_ReturnsTotalDuration()
        {
            var segments = new List<TrackSegment>()
            {
                new() {Duration = TimeSpan.FromMinutes(10)},
                new() {Duration = TimeSpan.FromMinutes(5)}
            };

            var result = DurationProcessor.TrackDuration(segments);

            result.Should().Be(TimeSpan.FromMinutes(15));
        }
        [Test]
        public void SegmentDuration_NullPoints_ReturnsZeroDuration()
        {
            var result = DurationProcessor.SegmentDuration(null);

            result.Should().Be(TimeSpan.Zero);
        }
        [Test]
        public void SegmentDuration_ZeroSegments_ReturnsZeroDuration()
        {
            var result = DurationProcessor.SegmentDuration(new List<TrackPoint>());

            result.Should().Be(TimeSpan.Zero);
        }

        [Test]
        public void SegmentDuration_PointsDurationZero_ReturnsZeroDuration()
        {
            var trackPoints = new List<TrackPoint>()
            {
                new() {Time = DateTime.Parse("2021-1-01T12:00:00")},
                new() {Time = DateTime.Parse("2021-1-01T12:00:00")}
            };

            var result = DurationProcessor.SegmentDuration(trackPoints);

            result.Should().Be(TimeSpan.Zero);
        }

        [Test]
        public void SegmentDuration_PointsDuration30Min_Returns30MinTimespan()
        {
            var trackPoints = new List<TrackPoint>()
            {
                new() {Time = DateTime.Parse("2021-1-01T12:00:00")},
                new() {Time = DateTime.Parse("2021-1-01T12:30:00")}
            };

            var result = DurationProcessor.SegmentDuration(trackPoints);

            result.Should().Be(TimeSpan.FromMinutes(30));
        }
        [Test]
        public void SegmentDuration_ThreePointsDuration30Min_Returns30MinTimespan()
        {
            var trackPoints = new List<TrackPoint>()
            {
                new() {Time = DateTime.Parse("2021-1-01T12:00:00")},
                new() {Time = DateTime.Parse("2021-1-01T12:20:00")},
                new() {Time = DateTime.Parse("2021-1-01T12:30:00")}
            };

            var result = DurationProcessor.SegmentDuration(trackPoints);

            result.Should().Be(TimeSpan.FromMinutes(30));
        }
    }
}
