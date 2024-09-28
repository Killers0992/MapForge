using MapForge.API.Enums;

namespace MapForge.API.Models
{
    public class FileAction
    {
        public string FullPath { get; set; }
        public string FileName { get; set; }
        public FileActionType Type { get; set; }
    }
}
