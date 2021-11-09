using System.Collections.Generic;
using Heracles.Application.Services.Import;

namespace Heracles.Web.Models
{
    public class ImportViewModel
    {
        public ImportViewModel()
        {
            FilesFailed = new List<FileResult>();
        }
        public int FilesImported { get; init; }
        public IList<FileResult> FilesFailed { get; init; }

        public string DisplayFilesImported(int numberOfFiles)
        {
            return numberOfFiles == 1 ? "1 file" : numberOfFiles + " files";
        }

        public bool ImportExecuted { get; init; }
    }
}
