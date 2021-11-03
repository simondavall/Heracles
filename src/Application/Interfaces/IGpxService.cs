using System.Threading.Tasks;
using Heracles.Application.GpxTrackAggregate;
using Microsoft.AspNetCore.Http;

namespace Heracles.Application.Interfaces
{
    public interface IGpxService
    {
        Task<TrackAggregate> LoadLoadContentsOfGpxFile(IFormFile file);
    }
}
