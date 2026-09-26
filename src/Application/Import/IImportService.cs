#nullable enable
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Forms;

namespace Heracles.Application.Import;

public interface IImportService
{
    Task<ImportFilesResult> ImportTracksFromGpxFilesAsync(
        IReadOnlyList<IBrowserFile> files,
        long maxAllowedSize,
        Action<decimal>? progress = null,
        CancellationToken cancellationToken = default);
}