using System.Diagnostics;
using System.Threading.Tasks;

namespace MediaService.Services
{
    public class VideoDownloadService
    {
        public async Task<string> DownloadVideoAsync(string videoUrl)
        {
            // we'll need to change the output path to be using S3
            string filename = $"{Guid.NewGuid()}.mp4";
            string outputPath = Path.Combine("D:\\Sayeed\\Downloads", filename);
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

        public async Task<string> ConvertToMp3Async(string videoUrl)
        {
            string filename = $"{Guid.NewGuid()}.mp3";
            string outputPath = Path.Combine("D:\\Sayeed\\Downloads", filename);
            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "yt-dlp",
                    Arguments = $"{videoUrl} --extract-audio --audio-format mp3 -o {outputPath}",
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                }
            };
            process.Start();
            string standardOutput = await process.StandardOutput.ReadToEndAsync();
            string standardError = await process.StandardError.ReadToEndAsync();
            await process.WaitForExitAsync();

            if (process.ExitCode == 0)
            {
                return outputPath;
            }
            else
            {
                throw new Exception($"Failed to convert video to MP3. Error: {standardError}");
            }
        }
    }
}
