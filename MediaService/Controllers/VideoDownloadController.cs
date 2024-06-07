using MediaService.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MediaService.Services;
using System.Threading.Tasks;

namespace MediaService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VideoDownloadController : ControllerBase
    {
        private readonly VideoDownloadService _videoDownloadService;
        private readonly ILogger<VideoDownloadController> _logger;

        public VideoDownloadController(VideoDownloadService videoDownloadService, ILogger<VideoDownloadController> logger)
        {
            _videoDownloadService = videoDownloadService;
            _logger = logger;
        }

        [HttpPost("downloadMp4")]
        public async Task<IActionResult> DownloadMp4([FromBody] VideoDownloadRequest request)
        {
            _logger.LogInformation("Received request to download MP4.");
            if (request == null)
            {
                return BadRequest( new { error = "Request body is null." });
            }

            if (string.IsNullOrWhiteSpace(request.VideoUrl))
            {
                _logger.LogWarning("Download MP4 request failed: Video URL is required.");
                return BadRequest(new { error = "The videoUrl field is required." });
            }

            try
            {
                _logger.LogInformation($"Attempting to download video from URL: {request.VideoUrl}");
                var filePath = await _videoDownloadService.DownloadVideoAsync(request.VideoUrl);
                _logger.LogInformation($"Video downloaded successfully, file path: {filePath}");
                return Ok(new { filePath });
            }

            catch(System.Exception ex)
            {
                _logger.LogError($"Error in downloading video: {ex}");
                return BadRequest(ex.Message);
                
            }
        }

        [HttpPost("downloadMp3")]
        public async Task<IActionResult> DownloadMp3([FromBody] VideoDownloadRequest request)
        {
            try
            {
                var filePath = await _videoDownloadService.ConvertToMp3Async(request.VideoUrl);
                return Ok(filePath);
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
