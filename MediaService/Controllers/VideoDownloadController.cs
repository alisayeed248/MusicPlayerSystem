using MediaService.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MediaService.Services;
using System.Threading.Tasks;

namespace MediaService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VideoDownloadController : ControllerBase
    {
        private readonly VideoDownloadService _videoDownloadService;

        public VideoDownloadController(VideoDownloadService videoDownloadService)
        {
            _videoDownloadService = videoDownloadService;
        }

        [HttpPost("download")]
        public async Task<IActionResult> DownloadVideo([FromBody] VideoDownloadRequest request)
        {
            try
            {
                var filePath = await _videoDownloadService.DownloadVideoAsync(request.VideoUrl);
                return Ok(filePath);
            }
            catch(System.Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
