using System;
using System.Collections;
using System.Collections.Generic;
using FluentAssertions;
using Heracles.Application.Entities.Points;
using Heracles.Application.TrackAggregate;
using NUnit.Framework;

namespace Heracles.Application.UnitTests.Entities
{
    class PointsTests
    {
        private readonly TrackPoint _currentTrackPoint;
        private readonly Dictionary<string, Point> _testPoints;

        public PointsTests()
        {
            _currentTrackPoint = new TrackPoint()
            {
                Elevation = 27.0,
                Time = new DateTime(2021, 11, 21, 16, 18, 01),
                Id = 1,
                Latitude = 51.361737000,
                Longitude = -0.348546000,
                Seq = 1,
                TrackSegmentId = Guid.NewGuid()
            };
            var prevTrackPoint = new TrackPoint()
            {
                Elevation = 27.0,
                Time = new DateTime(2021,11,21,16,17,56),
                Id = 0,
                Latitude = 51.361712000,
                Longitude = -0.348546000,
                Seq = 0,
                TrackSegmentId = Guid.NewGuid()
            };
            _testPoints = new Dictionary<string, Point>()
            {
                {"StartPoint", new StartPoint(_currentTrackPoint, null, _currentTrackPoint)},
                {"EndPoint", new EndPoint(_currentTrackPoint, prevTrackPoint, prevTrackPoint)},
                {"PausePoint", new PausePoint(_currentTrackPoint, prevTrackPoint, prevTrackPoint)},
                {"ResumePoint", new ResumePoint(_currentTrackPoint, prevTrackPoint, prevTrackPoint)},
                {"InterimPoint", new InterimPoint(_currentTrackPoint, prevTrackPoint, prevTrackPoint)},
            };
        }

        [TestCaseSource(typeof(TestData), nameof(TestData.TypeCheck))]
        public string Type_ShouldBeCorrectType(string pointType)
        {
            return _testPoints[pointType].Type;
        }

        [TestCaseSource(typeof(TestData), nameof(TestData.DeltaDistanceCheck))]
        public double DeltaDistance_ShouldCorrectDistance(string pointType)
        {
            return _testPoints[pointType].DeltaDistance;
        }

        [TestCaseSource(typeof(TestData), nameof(TestData.TimestampCheck))]
        public double Timestamp_ShouldCorrectTimestamp(string pointType)
        {
            return _testPoints[pointType].Timestamp;
        }

        [Test]
        public void StartPoint_DeltaPause_ShouldBeZero()
        {
            _testPoints["StartPoint"].DeltaPause.Should().Be(0);
        }

        [TestCaseSource(typeof(TestData), nameof(TestData.DeltaTimeCheck))]
        public double DeltaTime_ShouldBeCorrectDeltaTime(string pointType)
        {
             return _testPoints[pointType].DeltaTime;
        }

        [Test]
        public void StartPoint_Elevation_ShouldCurrentPointElevation()
        {
            _testPoints["StartPoint"].Altitude.Should().Be(_currentTrackPoint.Elevation);
        }

        [Test]
        public void StartPoint_Longitude_ShouldCurrentPointLongitude()
        {
            _testPoints["StartPoint"].Longitude.Should().Be(_currentTrackPoint.Longitude);
        }

        [Test]
        public void StartPoint_Latitude_ShouldCurrentPointLatitude()
        {
            _testPoints["StartPoint"].Latitude.Should().Be(_currentTrackPoint.Latitude);
        }

        class TestData
        {
            public static IEnumerable TypeCheck
            {
                get
                {
                    yield return new TestCaseData("StartPoint").Returns("StartPoint");
                    yield return new TestCaseData("EndPoint").Returns("EndPoint");
                    yield return new TestCaseData("PausePoint").Returns("PausePoint");
                    yield return new TestCaseData("ResumePoint").Returns("ResumePoint");
                    yield return new TestCaseData("InterimPoint").Returns("TrackPoint");
                }
            }

            public static IEnumerable DeltaDistanceCheck
            {
                get
                {
                    yield return new TestCaseData("StartPoint").Returns(0);
                    yield return new TestCaseData("EndPoint").Returns(2.7798731662062512d);
                    yield return new TestCaseData("PausePoint").Returns(2.7798731662062512d);
                    yield return new TestCaseData("ResumePoint").Returns(2.7798731662062512d);
                    yield return new TestCaseData("InterimPoint").Returns(2.7798731662062512d);
                }
            }

            public static IEnumerable TimestampCheck
            {
                get
                {
                    yield return new TestCaseData("StartPoint").Returns(0);
                    yield return new TestCaseData("EndPoint").Returns(5000);
                    yield return new TestCaseData("PausePoint").Returns(5000);
                    yield return new TestCaseData("ResumePoint").Returns(5000);
                    yield return new TestCaseData("InterimPoint").Returns(5000);
                }
            }

            public static IEnumerable DeltaTimeCheck
            {
                get
                {
                    yield return new TestCaseData("StartPoint").Returns(0);
                    yield return new TestCaseData("EndPoint").Returns(5000);
                    yield return new TestCaseData("PausePoint").Returns(5000);
                    yield return new TestCaseData("ResumePoint").Returns(0);
                    yield return new TestCaseData("InterimPoint").Returns(5000);
                }
            }

        }

    }
}
