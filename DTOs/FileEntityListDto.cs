    public class FileEntityListDto
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public List<FileEntityDto> Files { get; set; } = new List<FileEntityDto>();
    }

    public class FileEntityDto
    {
        public Guid FileId { get; set; }
        public string FileName { get; set; } = string.Empty;
        public Guid FolderId { get; set; }
        public DateTime CreatedAt { get; set; }
    }