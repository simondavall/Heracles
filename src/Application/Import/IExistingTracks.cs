namespace Heracles.Application.Import
{
    public interface IExistingTracks
    {
        void AddTrack(string trackName);
        bool TrackExists(string trackName);
    }
}