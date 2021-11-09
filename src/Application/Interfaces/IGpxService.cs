using Heracles.Application.TrackAggregate;
using Microsoft.AspNetCore.Http;

namespace Heracles.Application.Interfaces
{
    public interface IGpxService
    {
        Track LoadContentsOfGpxFile(IFormFile file);
    }
}
