using Heracles.Application.Configuration;
using Heracles.Application.Data;
using Heracles.Application.TrackPoints;

namespace Heracles.Application.Pace;

public interface IPaceService
{
    Task<IReadOnlyList<PaceObservation>> GetPaceAsync(Track track, CancellationToken cancellationToken = default);
}

public sealed class PaceService : IPaceService
{
    private readonly ITrackPointDataService _trackPointDataService;
    private readonly PaceSettings _settings;

    public PaceService(ITrackPointDataService trackPointDataService, PaceSettings settings) {
        _trackPointDataService = trackPointDataService;
        _settings = settings;
    }

    public async Task<IReadOnlyList<PaceObservation>> GetPaceAsync(Track track, CancellationToken cancellationToken = default) {
        var trackPointData = await _trackPointDataService.GetAsync(track, cancellationToken);
        if (trackPointData.Count < 2)
            return [];

        return CalculateObservations(trackPointData);
    }

    private IReadOnlyList<PaceObservation> CalculateObservations(IReadOnlyList<TrackPointData> trackPointData) {
        var observations = new List<PaceObservation>();

        for (var centre = 0; centre < trackPointData.Count; centre++)
            AddObservation(trackPointData, centre, observations);

        return observations;
    }

    private void AddObservation(IReadOnlyList<TrackPointData> trackPointData, int centre, ICollection<PaceObservation> observations) {
        var start = Math.Max(0, centre - _settings.WindowRadius);
        var end = Math.Min(trackPointData.Count - 1, centre + _settings.WindowRadius);

        var distance = trackPointData[end].CumulativeDistance - trackPointData[start].CumulativeDistance;
        var elapsedSeconds = trackPointData[end].CumulativeTime - trackPointData[start].CumulativeTime;
        if (distance <= 0 || elapsedSeconds <= 0)
            return;
        
        observations.Add(
            new PaceObservation(
                trackPointData[centre].TrackPointId,
                trackPointData[centre].CumulativeDistance,
                elapsedSeconds / distance));
    }
}

public sealed record PaceObservation(
    int TrackPointId,
    double Distance,
    double SecondsPerKilometre);