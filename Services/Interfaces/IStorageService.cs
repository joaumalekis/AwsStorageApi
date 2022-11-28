using AwsStorageApi.Models;

namespace AwsStorageApi.Services.Interfaces
{
    public interface IStorageService
    {
        Task<byte[]> DownloadFileAsync(string file);

        Task<AwsS3Response> UploadFileAsync(AwsS3Request awsS3Request);

        Task<bool> DeleteFileAsync(string fileName, string versionId = "");
    }
}