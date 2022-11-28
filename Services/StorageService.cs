using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Transfer;
using AwsStorageApi.Models;
using AwsStorageApi.Services.Interfaces;
using Microsoft.Extensions.Options;

namespace AwsStorageApi.Services;

public class StorageService : IStorageService
{
    private readonly AwsConfiguration _awsConfiguration;

    public StorageService(IOptions<AwsConfiguration> options)
    {
        _awsConfiguration = options.Value;
    }

    public Task<bool> DeleteFileAsync(string fileName, string versionId = "")
    {
        throw new NotImplementedException();
    }

    public Task<byte[]> DownloadFileAsync(string file)
    {
        throw new NotImplementedException();
    }

    public async Task<AwsS3Response> UploadFileAsync(AwsS3Request awsS3Request)
    {
        var credentials = new BasicAWSCredentials(_awsConfiguration.AccessKey, _awsConfiguration.SecretKey);
        var config = new AmazonS3Config()
        {
            RegionEndpoint = Amazon.RegionEndpoint.SAEast1
        };

        var response = new AwsS3Response();
        try
        {
            var uploadRequest = new TransferUtilityUploadRequest()
            {
                InputStream = awsS3Request.InputStream,
                Key = awsS3Request.Name,
                BucketName = _awsConfiguration.BucketName,
                CannedACL = S3CannedACL.NoACL
            };

            using var client = new AmazonS3Client(credentials, config);

            var transferUtility = new TransferUtility(client);

            await transferUtility.UploadAsync(uploadRequest);

            response.StatusCode = 201;
            response.Message = $"{awsS3Request.Name} has been uploaded sucessfully";
        }
        catch (AmazonS3Exception s3Ex)
        {
            response.StatusCode = (int)s3Ex.StatusCode;
            response.Message = s3Ex.Message;
        }
        catch (Exception ex)
        {
            response.StatusCode = 500;
            response.Message = ex.Message;
        }

        return response;
    }
}