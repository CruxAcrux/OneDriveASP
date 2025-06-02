using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class FileEntityController : ControllerBase
{
    private readonly IFileEntityService _fileEntityService;

    public FileEntityController(IFileEntityService fileEntityService)
    {
        _fileEntityService = fileEntityService;
    }

    [HttpPost("upload")]
    public async Task<IActionResult> UploadFile([FromForm] UploadFileDto model)
    {
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized(new FileEntityResponseDto { Success = false, Message = "User not authenticated." });
        }

        var result = await _fileEntityService.UploadFileAsync(model, userId);
        if (!result.Success)
        {
            return BadRequest(result);
        }
        return Ok(result);
    }

    [HttpGet("download/{fileId}")]
    public async Task<IActionResult> DownloadFile(Guid fileId)
    {
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized(new FileDownloadDto { Success = false, Message = "User not authenticated." });
        }

        var result = await _fileEntityService.DownloadFileAsync(fileId, userId);
        if (!result.Success)
        {
            return NotFound(result);
        }

        return File(result.Data!, result.ContentType!, result.FileName);
    }

    [HttpDelete("{fileId}")]
    public async Task<IActionResult> DeleteFile(Guid fileId)
    {
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized("User not authenticated.");
        }

        var result = await _fileEntityService.DeleteFileAsync(fileId, userId);
        if (result.Contains("not found") || result.Contains("permission"))
        {
            return NotFound(result);
        }
        return Ok(result);
    }

    [HttpGet]
    public async Task<IActionResult> GetAllFiles()
    {
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized(new FileEntityListDto
            {
                Success = false,
                Message = "User not authenticated.",
                Files = new List<FileEntityDto>()
            });
        }

        var result = await _fileEntityService.GetAllFilesByUserAsync(userId);
        if (!result.Success)
        {
            return BadRequest(result);
        }
        return Ok(result);
    }
}
