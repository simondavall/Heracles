using Heracles.Application.Data;
using Heracles.Application.TrackPoints;
using Xunit;

namespace Heracles.Application.UnitTests.TrackPoints;

public sealed class TrackPointDataServiceTests
{
    [Fact]
    public async Task GetAsync_CreatesAndPersistsMissingTrackPointData() {
        var repository = new FakeTrackPointDataRepository();
        var service = new TrackPointDataService(repository);
        var track = CreateTrack();

        var result = await service.GetAsync(track, TestContext.Current.CancellationToken);

        Assert.NotEmpty(result);
        Assert.NotNull(repository.SavedData);

        Assert.Equal(track.TrackSegments.Sum(segment => segment.TrackPoints.Count), repository.SavedData.Count);

        Assert.Equal(
            Enumerable.Range(0, repository.SavedData.Count),
            repository.SavedData.Select(x => x.Seq));
    }

    [Fact]
    public async Task GetAsync_ReturnsPersistedDataWithoutSaving() {
        var track = CreateTrack();
        var existing = CreateExistingTrackPointData(track);
        var repository = new FakeTrackPointDataRepository(existing);
        var service = new TrackPointDataService(repository);

        var result = await service.GetAsync(track, TestContext.Current.CancellationToken);

        Assert.Same(existing, result);
        Assert.Null(repository.SavedData);
    }

    [Fact]
    public async Task GetAsync_DoesNotAddPauseTimeOrDistanceAcrossSegments() {
        var repository = new FakeTrackPointDataRepository();
        var service = new TrackPointDataService(repository);

        var result = await service.GetAsync(CreateTrack(), TestContext.Current.CancellationToken);

        var lastFirstSegment = result[2];
        var firstSecondSegment = result[3];

        Assert.Equal(lastFirstSegment.CumulativeDistance, firstSecondSegment.CumulativeDistance);
        Assert.Equal(lastFirstSegment.CumulativeTime, firstSecondSegment.CumulativeTime);
    }

    [Fact]
    public async Task GetAsync_UsesTrackWideZeroBasedSequence() {
        var expected = new[] { 0, 1, 2, 3, 4, 5 };

        var repository = new FakeTrackPointDataRepository();
        var service = new TrackPointDataService(repository);

        var result = await service.GetAsync(CreateTrack(), TestContext.Current.CancellationToken);

        Assert.Equal(expected, result.Select(x => x.Seq).ToArray());
    }

    [Fact]
    public async Task GetAsync_RetainsSourceTrackPointId() {
        var repository = new FakeTrackPointDataRepository();
        var service = new TrackPointDataService(repository);
        var track = CreateTrack();

        var expected =
            track
                .TrackSegments
                .OrderBy(segment => segment.Seq)
                .SelectMany(segment => segment.TrackPoints.OrderBy(point => point.Seq))
                .Select(point => point.Id)
                .ToArray();

        var result = await service.GetAsync(track, TestContext.Current.CancellationToken);

        Assert.Equal(expected, result.Select(x => x.TrackPointId).ToArray());
    }

    [Fact]
    public async Task GetAsync_RetainsElevation() {
        var repository = new FakeTrackPointDataRepository();
        var service = new TrackPointDataService(repository);
        var track = CreateTrack();

        var expected =
            track
                .TrackSegments
                .OrderBy(segment => segment.Seq)
                .SelectMany(segment => segment.TrackPoints.OrderBy(point => point.Seq))
                .Select(point => point.Elevation)
                .ToArray();

        var result = await service.GetAsync(track, TestContext.Current.CancellationToken);

        Assert.Equal(expected, result.Select(x => x.Elevation).ToArray());
    }

    [Fact]
    public async Task GetAsync_ReturnsEmptyWhenTrackHasNoPoints() {
        var repository = new FakeTrackPointDataRepository();
        var service = new TrackPointDataService(repository);
        var track = new Track { Id = Guid.NewGuid(), Name = "Test" };

        var result = await service.GetAsync(track, TestContext.Current.CancellationToken);

        Assert.Empty(result);
        Assert.Null(repository.SavedData);
    }

    private static Track CreateTrack() {
        var start = new DateTime(2024, 4, 22, 8, 0, 0, DateTimeKind.Utc);

        var firstSegment =
            new TrackSegment {
                Seq = 0,
                TrackPoints = [
                    Point(1, 0, 51.5000, -0.1000, 25, start),
                    Point(2, 1, 51.5009, -0.1000, 30, start.AddSeconds(30)),
                    Point(3, 2, 51.5018, -0.1000, 35, start.AddSeconds(60))
                ]
            };

        var secondSegment =
            new TrackSegment {
                Seq = 1,
                TrackPoints = [
                    Point(4, 0, 51.5018, -0.1000, 40, start.AddMinutes(5)),
                    Point(5, 1, 51.5027, -0.1000, 45, start.AddMinutes(5).AddSeconds(30)),
                    Point(6, 2, 51.5036, -0.1000, 50, start.AddMinutes(6))
                ]
            };

        return new Track { Id = Guid.NewGuid(), Name = "Test", TrackSegments = [firstSegment, secondSegment] };
    }

    private static TrackPoint Point(int id, int seq, double latitude, double longitude, double elevation, DateTime time) {
        return new TrackPoint {
            Id = id,
            Seq = seq,
            Latitude = latitude,
            Longitude = longitude,
            Elevation = elevation,
            Time = time
        };
    }

    private static IReadOnlyList<TrackPointData>
        CreateExistingTrackPointData(Track track) {
        var points =
            track
                .TrackSegments
                .OrderBy(segment => segment.Seq)
                .SelectMany(segment => segment.TrackPoints.OrderBy(point => point.Seq))
                .ToArray();

        return points
            .Select((point, index) =>
                new TrackPointData {
                    TrackId = track.Id,
                    TrackPointId = point.Id,
                    Seq = index,
                    CumulativeDistance = index * 0.1,
                    CumulativeTime = index * 30,
                    Elevation = point.Elevation
                })
            .ToArray();
    }

    private sealed class FakeTrackPointDataRepository : ITrackPointDataRepository
    {
        private readonly IReadOnlyList<TrackPointData> _existing;

        public FakeTrackPointDataRepository(IReadOnlyList<TrackPointData>? existing = null) {
            _existing = existing ?? Array.Empty<TrackPointData>();
        }

        public IReadOnlyList<TrackPointData>? SavedData { get; private set; }

        public Task<IReadOnlyList<TrackPointData>> GetAsync(Guid trackId,
            CancellationToken cancellationToken = default) {
            return Task.FromResult(_existing);
        }

        public Task SaveAsync(IReadOnlyCollection<TrackPointData> trackPointData,
            CancellationToken cancellationToken = default) {
            SavedData = trackPointData.ToArray();

            return Task.CompletedTask;
        }
    }
}