/// <summary>
/// Service responsible for managing folder operations including creation, retrieval,
/// and deletion of folders along with their associated files.
/// </summary>
public class FolderService : IFolderService
{
    private readonly IFolderRepository _folderRepository;
    private readonly IFileEntityRepository _fileEntityRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="FolderService"/> class.
    /// </summary>
    /// <param name="folderRepository">The repository for folder operations.</param>
    /// <param name="fileEntityRepository">The repository for file operations.</param>
    public FolderService(IFolderRepository folderRepository, IFileEntityRepository fileEntityRepository)
    {
        _folderRepository = folderRepository;
        _fileEntityRepository = fileEntityRepository;
    }

    /// <summary>
    /// Creates a new folder for the specified user.
    /// </summary>
    /// <param name="model">The folder creation data transfer object.</param>
    /// <param name="userId">The ID of the user creating the folder.</param>
    /// <returns>
    /// A <see cref="FolderResponseDto"/> indicating success or failure,
    /// along with folder details if successful.
    /// </returns>
    /// <remarks>
    /// Validates that a folder with the same name doesn't already exist for the user.
    /// </remarks>
    public async Task<FolderResponseDto> CreateFolderAsync(CreateFolderDto model, string userId)
    {
        try
        {
            var existingFolder = await _folderRepository.GetByNameAndUserAsync(model.Name, userId);
            if (existingFolder != null)
            {
                return new FolderResponseDto
                {
                    Success = false,
                    Message = "A folder with this name already exists."
                };
            }

            var folder = new FolderEntity
            {
                Id = Guid.NewGuid(),
                Name = model.Name,
                UserId = userId,
                CreatedAt = DateTime.UtcNow
            };

            await _folderRepository.AddAsync(folder);
            return new FolderResponseDto
            {
                Success = true,
                Message = "Folder created successfully.",
                FolderId = folder.Id,
                Name = folder.Name
            };
        }
        catch (Exception ex)
        {
            return new FolderResponseDto
            {
                Success = false,
                Message = $"Failed to create folder: {ex.Message}"
            };
        }
    }

    /// <summary>
    /// Retrieves all folders belonging to a user along with their files.
    /// </summary>
    /// <param name="userId">The ID of the user whose folders should be retrieved.</param>
    /// <returns>
    /// An enumerable of <see cref="FolderResponseDto"/> containing folder information
    /// and their associated files.
    /// </returns>
    /// <remarks>
    /// This method performs a complete folder tree retrieval including all files
    /// contained within each folder.
    /// </remarks>
    public async Task<IEnumerable<FolderResponseDto>> GetUserFoldersAsync(string userId)
    {
        try
        {
            var folders = await _folderRepository.GetAllByUserAsync(userId);
            var folderDtos = new List<FolderResponseDto>();

            foreach (var folder in folders)
            {
                var files = await _fileEntityRepository.GetAllByFolderAndUserAsync(folder.Id, userId);
                var fileDtos = files.Select(f => new FileEntityDto
                {
                    FileId = f.Id,
                    FileName = f.Name,
                    FolderId = f.FolderId,
                    CreatedAt = f.CreatedAt
                }).ToList();

                folderDtos.Add(new FolderResponseDto
                {
                    Success = true,
                    Message = "Folder retrieved successfully.",
                    FolderId = folder.Id,
                    Name = folder.Name,
                    Files = fileDtos
                });
            }

            return folderDtos;
        }
        catch (Exception ex)
        {
            return new List<FolderResponseDto>
            {
                new FolderResponseDto 
                { 
                    Success = false, 
                    Message = $"Failed to retrieve folders: {ex.Message}",
                    Files = new List<FileEntityDto>()
                }
            };
        }
    }

    /// <summary>
    /// Deletes a folder and all its contained files after validating user permissions.
    /// </summary>
    /// <param name="folderId">The ID of the folder to delete.</param>
    /// <param name="userId">The ID of the user requesting the deletion.</param>
    /// <returns>
    /// A string message indicating success or failure of the operation.
    /// </returns>
    /// <remarks>
    /// This operation performs a cascading delete of all files contained within the folder
    /// before deleting the folder itself.
    /// </remarks>
    public async Task<string> DeleteFolderAsync(Guid folderId, string userId)
    {
        try
        {
            var folder = await _folderRepository.GetByIdAsync(folderId);
            if (folder == null || folder.UserId != userId)
            {
                return "Folder not found or you do not have permission to delete it.";
            }

            var files = await _fileEntityRepository.GetAllByFolderAndUserAsync(folderId, userId);
            foreach (var file in files)
            {
                await _fileEntityRepository.DeleteAsync(file);
            }

            await _folderRepository.DeleteAsync(folder);
            return "Folder deleted successfully.";
        }
        catch (Exception ex)
        {
            return $"Failed to delete folder: {ex.Message}";
        }
    }
}