using System;

namespace Heracles.Application.Services.Import.Progress
{
    public static class ProgressHelper
    {
        public static decimal GetProgress(int itemsProcessedSoFar, long totalItems)
        {
            return (decimal)(Math.Floor(itemsProcessedSoFar * 10000D / totalItems) / 10000);
        }
    }
}
