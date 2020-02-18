using System.Collections.Generic;

namespace ScrapApi.Models
{
    public class RepositoryDetailsModel
    {
        public string FileExtension { get; set; }
        public long TotalLines { get; set; }
        public long TotalBytes { get; set; }
        public List<string> Files { get; set; }
    }
}
