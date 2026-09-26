using System;

namespace Heracles.Application.Import
{
    public interface IImportProgressService
    {
        decimal GetImportProgress(Guid processId);
        void InitializeProgress(Guid processId);
        void ProgressComplete(Guid processId);
        void UpdateProgress(Guid progressId, decimal newValue);
    }
}