using AwsStorageApi.Models;

namespace AwsStorageApi.Services.Interfaces
{
    public interface IStorageService
    {
        Task<byte[]> DownloadFileAsync(string fileName);
        Task<AwsS3Response> UploadFileAsync(AwsS3Request awsS3Request);
        Task DeleteFileAsync(string fileName);
        bool IsFileExists(string fileName);
    }
}