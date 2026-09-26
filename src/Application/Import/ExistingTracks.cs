using System.Collections.Generic;
using System.Threading.Tasks;
using Heracles.Application.Data;

namespace Heracles.Application.Import;

public interface IExistingTracks
{
    void AddTrack(string trackName);
    bool TrackExists(string trackName);
}

public class ExistingTracks : IExistingTracks
{
    private readonly ITrackRepository _trackRepository;

    private ExistingTracks(ITrackRepository trackRepository) {
        _trackRepository = trackRepository;
    }

    public static async Task<ExistingTracks> CreateAsync(ITrackRepository trackRepository) {
        var existingTracks = new ExistingTracks(trackRepository);
        await existingTracks.InitializeAsync();
        return existingTracks;
    }

    private IList<string> Tracks { get; set; }

    public void AddTrack(string trackName) {
        Tracks.Add(trackName);
    }

    private async Task InitializeAsync() {
        Tracks = await _trackRepository.GetExistingTracksAsync() ?? new List<string>();
    }

    public bool TrackExists(string trackName) {
        return Tracks.Contains(trackName);
    }
}