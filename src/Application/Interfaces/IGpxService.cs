using Heracles.Application.TrackAggregate;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Http;

namespace Heracles.Application.Interfaces
{
    public interface IGpxService
    {
        Track LoadContentsOfGpxFile(IFormFile file);
        Track LoadContentsOfGpxFile(IBrowserFile file);
    }
}
