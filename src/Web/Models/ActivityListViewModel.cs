using System.Collections.Generic;

namespace Heracles.Web.Models
{
    public class ActivityListViewModel
    {
        public IEnumerable<ActivitiesByMonthViewModel> ActivityListMonths { get; set; }
    }
}
