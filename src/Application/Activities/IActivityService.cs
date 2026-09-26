using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Heracles.Application.Data;
using Heracles.Application.Entities;

namespace Heracles.Application.Activities
{
    public interface IActivityService
    {
        Task<bool> DeleteActivityAsync(Guid trackId);
        Task<Track> GetActivityAsync(Guid trackId);
        Task<ActivityInfo> GetActivityInfoAsync(Guid trackId);
        Task<List<ActivityListItem>> GetActivitiesByDateAsync(DateTime startDate, Guid? trackId = null);
        Task<IList<ActivityListMonth>> GetActivitiesSummaryByMonthsAsync(Track track);
        Task<IList<ActivityListYear>> GetActivitiesSummaryByYearAsync(Track track);
        Task<Track> GetFirstEverActivityAsync();
        Task<Track> GetMostRecentActivityAsync();
        Task<(int rank, int count)> GetActivityRankAsync(Track track);
    }
}
