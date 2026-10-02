using Heracles.Application.Configuration;
using Heracles.Application.Data;
using Heracles.Application.Pace;
using Xunit;

namespace Heracles.Application.UnitTests.Pace;

public sealed class PaceServiceTests
{
    [Fact]
    public async Task GetPaceAsync_CreatesAndPersistsMissingPaceData()
    {
        var repository = new FakePaceRepository();
        var service = new PaceService(repository, new PaceSettings(1, 1));
        var track = CreateTrack();

        var result = await service.GetPaceAsync(track, TestContext.Current.CancellationToken);

        Assert.NotEmpty(result);
        Assert.NotNull(repository.SavedData);

        Assert.Equal(track.TrackSegments.Sum(segment => segment.TrackPoints.Count), repository.SavedData.Count);
        Assert.Equal(Enumerable.Range(0, repository.SavedData.Count), repository.SavedData.Select(x => x.Seq));
    }

    [Fact]
    public async Task GetPaceAsync_ReusesCompletePersistedData()
    {
        var track = CreateTrack();
        var existing = CreateExistingPaceData(track);
        var repository = new FakePaceRepository(existing);
        var service = new PaceService(repository, new PaceSettings(1, 1));

        await service.GetPaceAsync(track, TestContext.Current.CancellationToken);

        Assert.Null(repository.SavedData);
    }

    [Fact]
    public async Task GetPaceAsync_ReplacesIncompletePersistedData()
    {
        var track = CreateTrack();

        var existing =
            CreateExistingPaceData(track)
                .Take(2)
                .ToArray();

        var repository = new FakePaceRepository(existing);
        var service = new PaceService(repository, new PaceSettings(1, 1));

        await service.GetPaceAsync(track, TestContext.Current.CancellationToken);

        Assert.NotNull(repository.SavedData);
        Assert.Equal(track.TrackSegments.Sum(segment => segment.TrackPoints.Count), repository.SavedData.Count);
    }

    [Fact]
    public async Task GetPaceAsync_DoesNotAddPauseTimeOrDistanceAcrossSegments()
    {
        var repository = new FakePaceRepository();
        var service = new PaceService(repository, new PaceSettings(1, 1));
        var track = CreateTrack();

        await service.GetPaceAsync(track, TestContext.Current.CancellationToken);

        var data = repository.SavedData!;

        var lastFirstSegment = data[2];
        var firstSecondSegment = data[3];

        Assert.Equal(lastFirstSegment.CumulativeDistance, firstSecondSegment.CumulativeDistance);
        Assert.Equal(lastFirstSegment.CumulativeTime, firstSecondSegment.CumulativeTime);
    }

    [Fact]
    public async Task GetPaceAsync_UsesTrackWideZeroBasedSequence()
    {
        var repository = new FakePaceRepository();
        var service = new PaceService(repository, new PaceSettings(1, 1));

        await service.GetPaceAsync(CreateTrack(), TestContext.Current.CancellationToken);

        Assert.Equal(
            new[] { 0, 1, 2, 3, 4, 5 },
            repository.SavedData!
                .Select(x => x.Seq)
                .ToArray());
    }

    [Fact]
    public async Task GetPaceAsync_IncludesFinalPointWhenStrideDoesNotLandOnIt()
    {
        var track = CreateSingleSegmentTrack(pointCount: 6);
        var repository = new FakePaceRepository();
        var service = new PaceService(repository, new PaceSettings(WindowRadius: 1, Stride: 2));

        var result = await service.GetPaceAsync(track, TestContext.Current.CancellationToken);

        Assert.Equal(repository.SavedData![^1].CumulativeDistance, result[^1].Distance);
    }

    [Fact]
    public async Task GetPaceAsync_WindowCanCrossSegmentBoundary()
    {
        var track = CreateTrack();
        var repository = new FakePaceRepository();
        var service = new PaceService(repository, new PaceSettings(WindowRadius: 2, Stride: 1));

        var result = await service.GetPaceAsync(track, TestContext.Current.CancellationToken);

        Assert.NotEmpty(result);

        var boundaryDistance = repository.SavedData![3].CumulativeDistance;

        Assert.Contains(result, observation => observation.Distance == boundaryDistance);
    }

    private static Track CreateTrack()
    {
        var start =
            new DateTime(
                2024,
                4,
                22,
                8,
                0,
                0,
                DateTimeKind.Utc);

        var firstSegment =
            new TrackSegment
            {
                Seq = 0,
                TrackPoints =
                [
                    Point(1, 0, 51.5000, -0.1000, start),
                    Point(2, 1, 51.5009, -0.1000, start.AddSeconds(30)),
                    Point(3, 2, 51.5018, -0.1000, start.AddSeconds(60))
                ]
            };

        var secondSegment =
            new TrackSegment
            {
                Seq = 1,
                TrackPoints =
                [
                    Point(4, 0, 51.5018, -0.1000, start.AddMinutes(5)),
                    Point(5, 1, 51.5027, -0.1000, start.AddMinutes(5).AddSeconds(30)),
                    Point(6, 2, 51.5036, -0.1000, start.AddMinutes(6))
                ]
            };

        return new Track
        {
            Id = Guid.NewGuid(),
            Name = "Test",
            TrackSegments =
            [
                firstSegment,
                secondSegment
            ]
        };
    }

    private static Track CreateSingleSegmentTrack(
        int pointCount)
    {
        var start = new DateTime(2024, 4, 22, 8, 0, 0, DateTimeKind.Utc);

        var points =
            Enumerable.Range(0, pointCount)
                .Select(index =>
                    Point(
                        index + 1,
                        index,
                        51.5 + index * 0.0009,
                        -0.1,
                        start.AddSeconds(index * 30)))
                .ToList();

        return new Track
        {
            Id = Guid.NewGuid(),
            Name = "Test",
            TrackSegments =
            [
                new TrackSegment
                {
                    Seq = 0,
                    TrackPoints = points
                }
            ]
        };
    }

    private static TrackPoint Point(int id, int seq, double latitude, double longitude, DateTime time)
    {
        return new TrackPoint
        {
            Id = id,
            Seq = seq,
            Latitude = latitude,
            Longitude = longitude,
            Time = time
        };
    }

    private static IReadOnlyList<PaceData> CreateExistingPaceData(Track track)
    {
        var points =
            track.TrackSegments
                .OrderBy(segment => segment.Seq)
                .SelectMany(segment => segment.TrackPoints.OrderBy(point => point.Seq))
                .ToArray();

        return points
            .Select((point, index) =>
                new PaceData
                {
                    TrackId = track.Id,
                    TrackPointId = point.Id,
                    Seq = index,
                    CumulativeDistance = index * 0.1,
                    CumulativeTime = index * 30
                })
            .ToArray();
    }

    private sealed class FakePaceRepository : IPaceRepository
    {
        private readonly IReadOnlyList<PaceData> _existing;

        public FakePaceRepository(IReadOnlyList<PaceData>? existing = null)
        {
            _existing = existing ?? Array.Empty<PaceData>();
        }

        public IReadOnlyList<PaceData>? SavedData { get; private set; }

        public Task<IReadOnlyList<PaceData>> GetAsync(Guid trackId, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(_existing);
        }

        public Task ReplaceAsync(Guid trackId, IReadOnlyCollection<PaceData> paceData, CancellationToken cancellationToken = default)
        {
            SavedData = paceData.ToArray();

            return Task.CompletedTask;
        }
    }
}