namespace Heracles.Application.Services.Import
{
    public interface IExistingTracks
    {
        void AddTrack(string trackName);
        bool TrackExists(string trackName);
    }
}