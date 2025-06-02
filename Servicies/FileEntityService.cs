public class FileEntityService : IFileEntityService
{
        private readonly IFileEntityRepository _fileEntityRepository;
        private readonly IFolderRepository _folderRepository;

        public FileEntityService(IFileEntityRepository fileEntityRepository, IFolderRepository folderRepository)
        {
            _fileEntityRepository = fileEntityRepository;
            _folderRepository = folderRepository;
        }

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