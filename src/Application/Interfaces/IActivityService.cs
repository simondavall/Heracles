using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Heracles.Application.Entities;
using Heracles.Application.TrackAggregate;

namespace Heracles.Application.Interfaces
{
    public interface IActivityService
    {
        Task<Track> GetActivity(Guid trackId);
        Task<ActivityInfo> GetActivityInfoAsync(Guid trackId);
        Task<List<ActivityListItem>> GetActivitiesByDate(DateTime startDate, Guid? trackId = null);
        Task<IList<ActivityListMonth>> GetActivitiesSummaryByMonths(Track track);
        Task<Track> GetMostRecentActivity();
        Task<(int rank, int count)> GetActivityRank(Track track);
    }
}
