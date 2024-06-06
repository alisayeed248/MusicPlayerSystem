using System.Diagnostics;
using System.Threading.Tasks;

namespace MediaService.Services
{
    public class VideoDownloadService
    {
        public async Task<string> DownloadVideoAsync(string videoUrl)
        {
            // we'll need to change the output path to be using S3
            string outputPath = "path_to_save_downloaded_file";
            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "yt-dlp",
                    Arguments = $"{videoUrl} -o {outputPath}",
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                }
            };
            process.Start();
            await process.WaitForExitAsync();

            if (process.ExitCode == 0)
            {
                return outputPath; // Return the path of the downloaded file
            }
            else
            {
                throw new Exception("Failed to download video.");
            }
        }
    }
}
