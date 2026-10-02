using Heracles.Application.Configuration;
using Heracles.Application.Data;
using Heracles.Application.Pace;
using Heracles.Application.TrackPoints;
using Xunit;

namespace Heracles.Application.UnitTests.Pace;

public sealed class PaceServiceTests
{
    [Fact]
    public async Task GetPaceAsync_ReturnsEmptyWhenFewerThanTwoPoints() {
        var dataService =
            new FakeTrackPointDataService(
                [
                    Data(0, 0, 0)
                ]);

        var service =
            new PaceService(
                dataService,
                new PaceSettings(1));

        var result =
            await service.GetPaceAsync(
                CreateTrack(),
                TestContext.Current.CancellationToken);

        Assert.Empty(result);
    }

    [Fact]
    public async Task GetPaceAsync_CalculatesPaceFromTrackPointData() {
        var dataService =
            new FakeTrackPointDataService(
                [
                    Data(0, 0, 0),
                    Data(1, 0.1, 30),
                    Data(2, 0.2, 60)
                ]);

        var service = new PaceService(dataService, new PaceSettings(WindowRadius: 1));

        var result = await service.GetPaceAsync(CreateTrack(), TestContext.Current.CancellationToken);

        Assert.Equal(3, result.Count);

        Assert.Equal(1, result[0].TrackPointId);
        Assert.Equal(0, result[0].Distance);
        Assert.Equal(300, result[0].SecondsPerKilometre, 6);

        Assert.Equal(2, result[1].TrackPointId);
        Assert.Equal(0.1, result[1].Distance);
        Assert.Equal(300, result[1].SecondsPerKilometre, 6);

        Assert.Equal(3, result[2].TrackPointId);
        Assert.Equal(0.2, result[2].Distance);
        Assert.Equal(300, result[2].SecondsPerKilometre, 6);
    }
    
    [Fact]
    public async Task GetPaceAsync_CalculatesObservationForEveryEligiblePoint() {
        var dataService =
            new FakeTrackPointDataService(
            [
                Data(0, 0, 0),
                Data(1, 0.1, 30),
                Data(2, 0.2, 60),
                Data(3, 0.3, 90),
                Data(4, 0.4, 120),
                Data(5, 0.5, 150)
            ]);

        var service = new PaceService(dataService, new PaceSettings(WindowRadius: 1));

        var result = await service.GetPaceAsync(CreateTrack(), TestContext.Current.CancellationToken);

        Assert.Equal(6, result.Count);

        Assert.Equal(
            [1, 2, 3, 4, 5, 6],
            result
                .Select(observation => observation.TrackPointId)
                .ToArray());
    }
    
    [Fact]
    public async Task GetPaceAsync_WindowCanCrossSegmentBoundary() {
        var dataService =
            new FakeTrackPointDataService(
                [
                    Data(0, 0, 0),
                    Data(1, 0.1, 30),
                    Data(2, 0.2, 60),
                    Data(3, 0.2, 60),
                    Data(4, 0.3, 90),
                    Data(5, 0.4, 120)
                ]);

        var service = new PaceService(dataService, new PaceSettings(WindowRadius: 2));

        var result = await service.GetPaceAsync(CreateTrack(), TestContext.Current.CancellationToken);

        Assert.Contains(result, observation => observation.Distance == 0.2);
    }

    [Fact]
    public async Task GetPaceAsync_SkipsObservationWhenWindowHasNoDistance() {
        var dataService = new FakeTrackPointDataService([Data(0, 0, 0), Data(1, 0, 30)]);
        var service = new PaceService(dataService, new PaceSettings(WindowRadius: 1));

        var result = await service.GetPaceAsync(CreateTrack(), TestContext.Current.CancellationToken);

        Assert.Empty(result);
    }

    [Fact]
    public async Task GetPaceAsync_SkipsObservationWhenWindowHasNoElapsedTime() {
        var dataService = new FakeTrackPointDataService([Data(0, 0, 0), Data(1, 0.1, 0)]);
        var service = new PaceService(dataService, new PaceSettings(WindowRadius: 1));

        var result =
            await service.GetPaceAsync(
                CreateTrack(),
                TestContext.Current.CancellationToken);

        Assert.Empty(result);
    }

    private static TrackPointData Data(
        int seq,
        double cumulativeDistance,
        int cumulativeTime) {
        return new TrackPointData {
            TrackId = Guid.NewGuid(),
            TrackPointId = seq + 1,
            Seq = seq,
            CumulativeDistance = cumulativeDistance,
            CumulativeTime = cumulativeTime
        };
    }

    private static Track CreateTrack() {
        return new Track {
            Id = Guid.NewGuid(),
            Name = "Test"
        };
    }

    private sealed class FakeTrackPointDataService
        : ITrackPointDataService
    {
        private readonly IReadOnlyList<TrackPointData> _data;

        public FakeTrackPointDataService(
            IReadOnlyList<TrackPointData> data) {
            _data = data;
        }

        public Task<IReadOnlyList<TrackPointData>> GetAsync(
            Track track,
            CancellationToken cancellationToken = default) {
            return Task.FromResult(_data);
        }
    }
}