using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class FolderController : ControllerBase
    {
        private readonly IFolderService _folderService;

        public FolderController(IFolderService folderService)
        {
            _folderService = folderService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateFolder([FromBody] CreateFolderDto model)
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new FolderResponseDto { Success = false, Message = "User not authenticated." });
            }

            var result = await _folderService.CreateFolderAsync(model, userId);
            if (!result.Success)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetFolders()
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new FolderResponseDto 
                { 
                    Success = false, 
                    Message = "User not authenticated.",
                    Files = new List<FileEntityDto>()
                });
            }

            var folders = await _folderService.GetUserFoldersAsync(userId);
            return Ok(folders);
        }

        [HttpDelete("{folderId}")]
        public async Task<IActionResult> DeleteFolder(Guid folderId)
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User not authenticated.");
            }

            var result = await _folderService.DeleteFolderAsync(folderId, userId);
            if (result.Contains("not found") || result.Contains("permission"))
            {
                return NotFound(result);
            }
            return Ok(result);
        }
    }