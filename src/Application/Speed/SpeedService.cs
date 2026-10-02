using Heracles.Application.Configuration;
using Heracles.Application.Data;
using Heracles.Application.TrackPoints;

namespace Heracles.Application.Speed;

public interface ISpeedService
{
    Task<IReadOnlyList<SpeedObservation>> GetSpeedAsync(Track track, CancellationToken cancellationToken = default);
}

public sealed class SpeedService : ISpeedService
{
    private readonly ITrackPointDataService _trackPointDataService;
    private readonly SpeedSettings _settings;

    public SpeedService(ITrackPointDataService trackPointDataService, SpeedSettings settings) {
        _trackPointDataService = trackPointDataService;
        _settings = settings;
    }

    public async Task<IReadOnlyList<SpeedObservation>> GetSpeedAsync(Track track, CancellationToken cancellationToken = default) {
        var trackPointData = await _trackPointDataService.GetAsync(track, cancellationToken);

        if (trackPointData.Count < 2)
            return [];

        return CalculateObservations(trackPointData);
    }

    private IReadOnlyList<SpeedObservation> CalculateObservations(IReadOnlyList<TrackPointData> trackPointData) {
        var observations = new List<SpeedObservation>();

        for (var centre = 0; centre < trackPointData.Count; centre += _settings.Stride)
            AddObservation(trackPointData, centre, observations);

        var lastIndex = trackPointData.Count - 1;

        if (lastIndex % _settings.Stride != 0)
            AddObservation(trackPointData, lastIndex, observations);

        return observations;
    }

    private void AddObservation(IReadOnlyList<TrackPointData> trackPointData, int centre, ICollection<SpeedObservation> observations) {
        var start = Math.Max(0, centre - _settings.WindowRadius);
        var end = Math.Min(trackPointData.Count - 1, centre + _settings.WindowRadius);
        var distance = trackPointData[end].CumulativeDistance - trackPointData[start].CumulativeDistance;
        var elapsedSeconds = trackPointData[end].CumulativeTime - trackPointData[start].CumulativeTime;

        if (distance <= 0 || elapsedSeconds <= 0)
            return;

        const int secondsPerHour = 3600;
        var kilometresPerHour = distance / elapsedSeconds * secondsPerHour;

        observations.Add(new SpeedObservation(trackPointData[centre].CumulativeDistance, kilometresPerHour));
    }
}

public sealed record SpeedObservation(
    double Distance,
    double KilometresPerHour);