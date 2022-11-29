using System.Net;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using AwsStorageApi.Models;
using AwsStorageApi.Services.Interfaces;
using Microsoft.Extensions.Options;

namespace AwsStorageApi.Services;

public class StorageService : IStorageService
{
    private readonly AwsConfiguration _awsConfiguration;
    private readonly IAmazonS3 _awsS3Client;

    public StorageService(IOptions<AwsConfiguration> options)
    {
        _awsConfiguration = options.Value;
        var credentials = new BasicAWSCredentials(_awsConfiguration.AccessKey, _awsConfiguration.SecretKey);
        _awsS3Client = new AmazonS3Client(credentials, Amazon.RegionEndpoint.SAEast1);
    }

    public async Task DeleteFileAsync(string fileName)
    {
        var request = new DeleteObjectRequest
        {
            BucketName = _awsConfiguration.BucketName,
            Key = fileName
        };

        await _awsS3Client.DeleteObjectAsync(request);
    }

    public async Task<byte[]> DownloadFileAsync(string file)
    {
        MemoryStream? ms = null;

        var getObjectRequest = new GetObjectRequest
        {
            BucketName = _awsConfiguration.BucketName,
            Key = file
        };

        using (var response = await _awsS3Client.GetObjectAsync(getObjectRequest))
        {
            if (response.HttpStatusCode == HttpStatusCode.OK)
            {
                using (ms = new MemoryStream())
                {
                    await response.ResponseStream.CopyToAsync(ms);
                }
            }
        }

        if (ms is null || ms.ToArray().Length < 1)
            throw new FileNotFoundException($"The document '{file}' is not found");

        return ms.ToArray();
    }

    public async Task<AwsS3Response> UploadFileAsync(AwsS3Request awsS3Request)
    {
        var response = new AwsS3Response();
        try
        {
            var uploadRequest = new TransferUtilityUploadRequest
            {
                InputStream = awsS3Request.InputStream,
                Key = awsS3Request.Name,
                BucketName = _awsConfiguration.BucketName,
                CannedACL = S3CannedACL.NoACL
            };

            var transferUtility = new TransferUtility(_awsS3Client);

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

    public bool IsFileExists(string fileName)
    {
        try
        {
            var request = new GetObjectMetadataRequest
            {
                BucketName = _awsConfiguration.BucketName,
                Key = fileName
            };

            var result = _awsS3Client.GetObjectMetadataAsync(request).Result;

            return true;
        }
        catch (Exception ex)
        {
            if (ex.InnerException is not AmazonS3Exception awsEx) throw;
            if (string.Equals(awsEx.ErrorCode, "NoSuchBucket"))
                return false;

            if (string.Equals(awsEx.ErrorCode, "NotFound"))
                return false;

            throw;
        }
    }
}