using System;
using System.Collections.Generic;
using Heracles.Application.Interfaces;

namespace Heracles.Application.Services.Import
{
    public class ImportProgressService : IImportProgressService
    {
        private readonly Dictionary<Guid, decimal> _importProgress = new();
        
        public decimal GetImportProgress(Guid processId)
        {
            const int processComplete = 1;
            return _importProgress.ContainsKey(processId) ? _importProgress[processId] : processComplete;
        }

        public void InitializeProgress(Guid processId)
        {
            if (!_importProgress.ContainsKey(processId))
            {
                _importProgress.Add(processId, 0);
            }
        }

        public void UpdateProgress(Guid processId, decimal newValue)
        {
            if (!_importProgress.ContainsKey(processId))
            {
                InitializeProgress(processId);
            }
            
            _importProgress[processId] = newValue;
        }

        public void ProgressComplete(Guid processId)
        {
            _importProgress.Remove(processId);
        }
    }
}