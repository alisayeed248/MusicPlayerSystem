using Amazon.S3;
using Amazon.S3.Model;

namespace MediaService.Services
{
    public class S3UploadVideoService
    {
        private readonly IAmazonS3 _client;

        public S3UploadVideoService(IAmazonS3 client)
        {
            _client = client;
        }

        //public async Task<string> UploadFileAsync(string filePath)
        //{

        //}
    }
}
