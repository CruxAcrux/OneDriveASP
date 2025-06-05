/// <summary>Service for managing user folders and their contents</summary>
public interface IFolderService
{
    /// <summary>Creates a new folder for the specified user</summary>
    /// <param name="model">Folder creation details</param>
    /// <param name="userId">Owner's user ID</param>
    /// <returns>Folder creation result or error</returns>
    Task<FolderResponseDto> CreateFolderAsync(CreateFolderDto model, string userId);

    /// <summary>Retrieves all folders belonging to a user</summary>
    /// <param name="userId">Target user ID</param>
    /// <returns>Collection of user's folders with their contents</returns>
    Task<IEnumerable<FolderResponseDto>> GetUserFoldersAsync(string userId);

    /// <summary>Deletes a folder and all its contents</summary>
    /// <param name="folderId">Target folder ID</param>
    /// <param name="userId">Requesting user ID</param>
    /// <returns>Success message or error</returns>
    Task<string> DeleteFolderAsync(Guid folderId, string userId);
}