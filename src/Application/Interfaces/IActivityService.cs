using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Heracles.Application.Entities;
using Heracles.Application.TrackAggregate;

namespace Heracles.Application.Interfaces
{
    public interface IActivityService
    {
        Task<bool> DeleteActivityAsync(Guid trackId);
        Task<Track> GetActivityAsync(Guid trackId);
        Task<ActivityInfo> GetActivityInfoAsync(Guid trackId);
        Task<List<ActivityListItem>> GetActivitiesByDateAsync(DateTime startDate, Guid? trackId = null);
        Task<IList<ActivityListMonth>> GetActivitiesSummaryByMonthsAsync(Track track);
        Task<Track> GetFirstEverActivityAsync();
        Task<Track> GetMostRecentActivityAsync();
        Task<(int rank, int count)> GetActivityRankAsync(Track track);
    }
}
