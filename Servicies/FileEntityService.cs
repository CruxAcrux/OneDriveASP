/// <summary>
/// Service responsible for managing file operations including upload, download, deletion,
/// and retrieval of files for a specific user.
/// </summary>
public class FileEntityService : IFileEntityService
{
    private readonly IFileEntityRepository _fileEntityRepository;
    private readonly IFolderRepository _folderRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="FileEntityService"/> class.
    /// </summary>
    /// <param name="fileEntityRepository">The repository for file entity operations.</param>
    /// <param name="folderRepository">The repository for folder operations.</param>
    public FileEntityService(IFileEntityRepository fileEntityRepository, IFolderRepository folderRepository)
    {
        _fileEntityRepository = fileEntityRepository;
        _folderRepository = folderRepository;
    }

    /// <summary>
    /// Uploads a file to the specified folder after validating user permissions.
    /// </summary>
    /// <param name="model">The file upload data transfer object containing file details.</param>
    /// <param name="userId">The ID of the user uploading the file.</param>
    /// <returns>
    /// A <see cref="FileEntityResponseDto"/> indicating success or failure,
    /// along with file information if successful.
    /// </returns>
    /// <remarks>
    /// Validates that the target folder exists and belongs to the user,
    /// checks for duplicate filenames in the same folder, and stores the file content as a byte array.
    /// </remarks>
    public async Task<FileEntityResponseDto> UploadFileAsync(UploadFileDto model, string userId)
    {
        try
        {
            var folder = await _folderRepository.GetByIdAsync(model.FolderId);
            if (folder == null || folder.UserId != userId)
            {
                return new FileEntityResponseDto
                {
                    Success = false,
                    Message = "Folder not found or you do not have permission."
                };
            }

            var existingFile = await _fileEntityRepository.GetByNameAndFolderAsync(model.FileName, model.FolderId, userId);
            if (existingFile != null)
            {
                return new FileEntityResponseDto
                {
                    Success = false,
                    Message = "A file with this name already exists in the folder."
                };
            }

            using var memoryStream = new MemoryStream();
            await model.File.CopyToAsync(memoryStream);

            var fileEntity = new FileEntity
            {
                Id = Guid.NewGuid(),
                Name = model.FileName,
                ContentType = model.File.ContentType,
                Data = memoryStream.ToArray(),
                FolderId = model.FolderId,
                UserId = userId,
                CreatedAt = DateTime.UtcNow
            };

            await _fileEntityRepository.AddAsync(fileEntity);
            return new FileEntityResponseDto
            {
                Success = true,
                Message = "File uploaded successfully.",
                FileId = fileEntity.Id,
                FileName = fileEntity.Name
            };
        }
        catch (Exception ex)
        {
            return new FileEntityResponseDto
            {
                Success = false,
                Message = $"Failed to upload file: {ex.Message}"
            };
        }
    }

    /// <summary>
    /// Downloads a file after validating user permissions.
    /// </summary>
    /// <param name="fileId">The ID of the file to download.</param>
    /// <param name="userId">The ID of the user requesting the download.</param>
    /// <returns>
    /// A <see cref="FileDownloadDto"/> containing the file data if successful,
    /// or an error message if the operation fails.
    /// </returns>
    public async Task<FileDownloadDto> DownloadFileAsync(Guid fileId, string userId)
    {
        try
        {
            var file = await _fileEntityRepository.GetByIdAsync(fileId);
            if (file == null || file.UserId != userId)
            {
                return new FileDownloadDto
                {
                    Success = false,
                    Message = "File not found or you do not have permission."
                };
            }

            return new FileDownloadDto
            {
                Success = true,
                Message = "File retrieved successfully.",
                FileName = file.Name,
                ContentType = file.ContentType,
                Data = file.Data
            };
        }
        catch (Exception ex)
        {
            return new FileDownloadDto
            {
                Success = false,
                Message = $"Failed to download file: {ex.Message}"
            };
        }
    }

    /// <summary>
    /// Deletes a file after validating user permissions.
    /// </summary>
    /// <param name="fileId">The ID of the file to delete.</param>
    /// <param name="userId">The ID of the user requesting the deletion.</param>
    /// <returns>
    /// A string message indicating success or failure of the operation.
    /// </returns>
    public async Task<string> DeleteFileAsync(Guid fileId, string userId)
    {
        try
        {
            var file = await _fileEntityRepository.GetByIdAsync(fileId);
            if (file == null || file.UserId != userId)
            {
                return "File not found or you do not have permission.";
            }

            await _fileEntityRepository.DeleteAsync(file);
            return "File deleted successfully.";
        }
        catch (Exception ex)
        {
            return $"Failed to delete file: {ex.Message}";
        }
    }

    /// <summary>
    /// Retrieves all files belonging to a specific user.
    /// </summary>
    /// <param name="userId">The ID of the user whose files should be retrieved.</param>
    /// <returns>
    /// A <see cref="FileEntityListDto"/> containing the list of files if successful,
    /// or an error message if the operation fails.
    /// </returns>
    public async Task<FileEntityListDto> GetAllFilesByUserAsync(string userId)
    {
        try
        {
            var files = await _fileEntityRepository.GetAllByUserAsync(userId);
            var fileDtos = files.Select(f => new FileEntityDto
            {
                FileId = f.Id,
                FileName = f.Name,
                FolderId = f.FolderId,
                CreatedAt = f.CreatedAt
            }).ToList();

            return new FileEntityListDto
            {
                Success = true,
                Message = "Files retrieved successfully.",
                Files = fileDtos
            };
        }
        catch (Exception ex)
        {
            return new FileEntityListDto
            {
                Success = false,
                Message = $"Failed to retrieve files: {ex.Message}",
                Files = new List<FileEntityDto>()
            };
        }
    }
}