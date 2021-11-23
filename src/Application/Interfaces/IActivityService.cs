using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Heracles.Application.Entities;
using Heracles.Application.TrackAggregate;

namespace Heracles.Application.Interfaces
{
    public interface IActivityService
    {
        Task<ActivityInfo> GetActivityInfoAsync(Guid trackId);
        Task<List<ActivityListItem>> GetActivitiesByDate(DateTime startDate);
        Task<IList<ActivityListMonth>> GetActivitiesSummaryByMonths();
        Task<Track> GetMostRecentActivity();
    }
}
