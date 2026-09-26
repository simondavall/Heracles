using System.Threading;
using System.Threading.Tasks;
using Heracles.Application.Data;
using Microsoft.AspNetCore.Components.Forms;

namespace Heracles.Application.Import;

public interface IGpxService
{
    Task<Track> LoadContentsOfGpxFileAsync(IBrowserFile file, long maxAllowedSize, CancellationToken cancellationToken = default);
}