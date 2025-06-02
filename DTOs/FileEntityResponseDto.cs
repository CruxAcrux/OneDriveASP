    public class FileEntityResponseDto
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public Guid? FileId { get; set; }
        public string? FileName { get; set; }
    }