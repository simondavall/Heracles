using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Heracles.Application.Services.Import
{
    public interface IImportService
    {
        Task<ImportFilesResult> ImportTracksFromGpxFilesAsync(IFormFileCollection files, IExistingTracks existingTracks = null, Action<decimal> progress = null);
    }
}