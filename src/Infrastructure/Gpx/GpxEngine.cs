using System.IO;
using Microsoft.AspNetCore.Http;

namespace Heracles.Infrastructure.Gpx
{
    public static class GpxEngine
    {
        public static GpxTrack GetGpxTrackFromStream(Stream fileStream)
        {
            using (Stream input = fileStream)
            {
                using (GpxReader reader = new GpxReader(input))
                {
                    reader.Read();
                    return reader.Track;
                }
            }
        }

        public static GpxTrack GetGpxTrackFromFile(IFormFile file)
        {
            if (file != null && file.Length > 0)
            {
                return GetGpxTrackFromStream(file.OpenReadStream());
            }
            return null;
        }
    }
}
