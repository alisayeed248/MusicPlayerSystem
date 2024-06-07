using System.Diagnostics;
using System.Threading.Tasks;

namespace MediaService.Services
{
    public class VideoDownloadService
    {
        public async Task<string> DownloadVideoAsync(string videoUrl)
        {
            string bucketName = "videodownloader-dev-2024";
            string keyName = $"{Guid.NewGuid()}.mp4";
            string awsCliCommand = $"aws s3 cp - s3://{bucketName}/{keyName}";

            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "yt-dlp",
                    Arguments = $"{videoUrl} -o -",
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                }
            };

            var uploadProcess = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "cmd.exe",
                    Arguments = $"/c {awsCliCommand}",
                    RedirectStandardInput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                }
            };

            process.Start();
            uploadProcess.Start();

            using (var outputStream = process.StandardOutput.BaseStream)
            using (var inputStream = uploadProcess.StandardInput.BaseStream)
            {
                await outputStream.CopyToAsync(inputStream);
                inputStream.Close();

            }


            process.WaitForExit();
            uploadProcess.WaitForExit();

            if (process.ExitCode == 0 && uploadProcess.ExitCode == 0)
            {
                return keyName; // Return the S3 key
            }
            else
            {
                throw new Exception("Failed to download or upload video.");
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
