

public class UploadFileDto
{
    public string FileName { get; set; } = string.Empty;
    public Guid FolderId { get; set; }
    public IFormFile File { get; set; } = null!;
}