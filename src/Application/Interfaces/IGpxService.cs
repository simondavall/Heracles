using System.Threading;
using System.Threading.Tasks;
using Heracles.Application.TrackAggregate;
using Microsoft.AspNetCore.Components.Forms;

namespace Heracles.Application.Interfaces;

public interface IGpxService
{
    Task<Track> LoadContentsOfGpxFileAsync(IBrowserFile file, long maxAllowedSize, CancellationToken cancellationToken = default);
}