/// <summary>Service for managing user file operations</summary>
public interface IFileEntityService
{
    /// <summary>Uploads a file to user storage</summary>
    /// <param name="model">File metadata and content</param>
    /// <param name="userId">Owner's user ID</param>
    /// <returns>Upload result with file ID or error</returns>
    Task<FileEntityResponseDto> UploadFileAsync(UploadFileDto model, string userId);

    /// <summary>Downloads a file if user has permission</summary>
    /// <param name="fileId">Target file ID</param>
    /// <param name="userId">Requesting user ID</param>
    /// <returns>File content with metadata or access error</returns>
    Task<FileDownloadDto> DownloadFileAsync(Guid fileId, string userId);

    /// <summary>Permanently deletes a user's file</summary>
    /// <param name="fileId">Target file ID</param>
    /// <param name="userId">Requesting user ID</param>
    /// <returns>Success message or error</returns>
    Task<string> DeleteFileAsync(Guid fileId, string userId);

    /// <summary>Lists all files belonging to a user</summary>
    /// <param name="userId">Target user ID</param>
    /// <returns>Complete file list or error</returns>
    Task<FileEntityListDto> GetAllFilesByUserAsync(string userId);
}