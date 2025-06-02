public class FolderService : IFolderService
{
    private readonly IFolderRepository _folderRepository;
    private readonly IFileEntityRepository _fileEntityRepository;

    public FolderService(IFolderRepository folderRepository, IFileEntityRepository fileEntityRepository)
    {
        _folderRepository = folderRepository;
        _fileEntityRepository = fileEntityRepository;
    }

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