using System;
using System.Collections.Generic;
using System.Globalization;
using Heracles.Application.Entities;

namespace Heracles.Web.Models
{
    public class ActivitiesByMonthViewModel
    {
        public DateTime ActivityDate { get; set; }
        public int ActivityCount { get; set; }

        public string Year => ActivityDate.ToString("yy", CultureInfo.InvariantCulture);

        public string Month => ActivityDate.ToString("MMM", CultureInfo.InvariantCulture);

        public string DataDate => new DateTime(ActivityDate.Year, ActivityDate.Month, 1).ToString("MMM-dd-yyyy",
            CultureInfo.InvariantCulture);

        public IEnumerable<ActivityListItem> Activities { get; set; }
    }
}
