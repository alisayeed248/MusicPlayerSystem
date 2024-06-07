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

        [HttpPost("downloadMp4")]
        public async Task<IActionResult> DownloadMp4([FromBody] VideoDownloadRequest request)
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
