using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Heracles.Application.Entities;

namespace Heracles.Application.Interfaces
{
    public interface IActivityService
    {
        public Task<List<ActivityListItem>> GetActivitiesByDate(DateTime startDate);

        Task<IList<ActivityListMonth>> GetActivitiesSummaryByMonths();
    }
}
