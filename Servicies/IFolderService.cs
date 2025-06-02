    public interface IFolderService
    {
        Task<FolderResponseDto> CreateFolderAsync(CreateFolderDto model, string userId);
        Task<IEnumerable<FolderResponseDto>> GetUserFoldersAsync(string userId);
        Task<string> DeleteFolderAsync(Guid folderId, string userId);
    }