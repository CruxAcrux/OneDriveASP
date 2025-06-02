    public class FolderResponseDto
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public Guid? FolderId { get; set; }
        public string? Name { get; set; }
        public List<FileEntityDto> Files { get; set; } = new List<FileEntityDto>(); // New field
    }

