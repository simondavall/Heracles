using System.Threading.Tasks;
using Heracles.Application.TrackAggregate;
using Microsoft.AspNetCore.Http;

namespace Heracles.Application.Interfaces
{
    public interface IGpxService
    {
        Task<Track> LoadLoadContentsOfGpxFile(IFormFile file);
    }
}
