using System;
using System.Collections.Generic;

namespace Heracles.Application.Import.Progress
{
    public interface IImportProgressService
    {
        decimal GetImportProgress(Guid processId);
        void InitializeProgress(Guid processId);
        void ProgressComplete(Guid processId);
        void UpdateProgress(Guid progressId, decimal newValue);
    }
    
    public class ImportProgressService : IImportProgressService
    {
        private readonly Dictionary<Guid, decimal> _importProgress = new();
        
        public decimal GetImportProgress(Guid processId)
        {
            const decimal processComplete = 1;
            return _importProgress.GetValueOrDefault(processId, processComplete);
        }

        public void InitializeProgress(Guid processId) {
            const decimal initialValue = 0;
            _importProgress.TryAdd(processId, initialValue);
        }

        public void UpdateProgress(Guid processId, decimal newValue)
        {
            if (!_importProgress.ContainsKey(processId))
                InitializeProgress(processId);
            
            
            _importProgress[processId] = newValue;
        }

        public void ProgressComplete(Guid processId)
        {
            _importProgress.Remove(processId);
        }
    }
}