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
        private readonly VideoDownloadService _downloadService;

        public VideoDownloadController(VideoDownloadService downloadService)
        {
            _downloadService = downloadService;
        }

        [HttpPost("download")]
        public async Task<IActionResult> DownloadVideo([FromBody] string videoUrl)
        {
            try
            {
                var filePath = await _downloadService.DownloadVideoAsync(videoUrl);
                return Ok(filePath);
            }
            catch(System.Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
