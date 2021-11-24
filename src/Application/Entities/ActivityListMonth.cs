using System.Collections.Generic;

namespace Heracles.Application.Entities
{
    public class ActivityListMonth
    {
        public int ActivityYearMonth { get; set; }
        public int Count { get; set; }
        public List<ActivityListItem> Activities { get; set; }
    }
}
