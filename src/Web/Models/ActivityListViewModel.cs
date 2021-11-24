using System.Collections.Generic;

namespace Heracles.Web.Models
{
    public class ActivityListViewModel
    {
        public IEnumerable<ActivitiesByMonthViewModel> ActivityListMonths { get; set; }
        public int ActiveMonthTab { get; set; }
        public string SelectedActivityId { get; set; }
    }
}
