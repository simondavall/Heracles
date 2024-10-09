using System.Linq;
using Heracles.Application.TrackAggregate;

namespace Heracles.Application.Services
{
    public static class ActivityRanking
    {
        public static (double upperBound, double lowerBound) GetRankBounds(Track track)
        {
            var upperBound = track.Distance * 1.05;
            var lowerBound = track.Distance * 0.95;

            return (upperBound, lowerBound);
        }

        public static int GetRank(Track track, Track[] tracks)
        {
            return tracks.Count(x => x.Pace < track.Pace) + 1; ;
        }
    }
}
