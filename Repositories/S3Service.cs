using System.IO;
using System.Threading.Tasks;
using Amazon.S3;
using Amazon.S3.Transfer;
using SaleafApi.Interfaces;

namespace SaleafApi.Repositories
{
    public class S3Service : IS3Service
    {
        private readonly IAmazonS3 _s3Client;
        private readonly string _bucketName;

        public S3Service(IAmazonS3 s3Client, IConfiguration configuration)
        {
            _s3Client = s3Client;
            _bucketName = configuration["AWS_BUCKET_NAME"]; // Retrieve bucket name from configuration
        }

        public async Task<Stream> DownloadFileAsync(string fileName)
        {
            var response = await _s3Client.GetObjectAsync(_bucketName, fileName);
            return response.ResponseStream;
        }

        public async Task UploadFileAsync(Stream fileStream, string fileName)
        {
            var fileTransferUtility = new TransferUtility(_s3Client);
            await fileTransferUtility.UploadAsync(fileStream, _bucketName, fileName);
        }
    }
}
