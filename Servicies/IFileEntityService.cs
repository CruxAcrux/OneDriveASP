    public interface IFileEntityService
    {
        Task<FileEntityResponseDto> UploadFileAsync(UploadFileDto model, string userId);
        Task<FileDownloadDto> DownloadFileAsync(Guid fileId, string userId);
        Task<string> DeleteFileAsync(Guid fileId, string userId);
        Task<FileEntityListDto> GetAllFilesByUserAsync(string userId); // New method
    }