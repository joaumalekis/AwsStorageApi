using AwsStorageApi.Models;
using AwsStorageApi.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AwsStorageApi.Controllers;

public class StorageController : ControllerBase
{
    private readonly IStorageService _storageService;
    public StorageController(IStorageService storageService)
    {
        _storageService = storageService;
    }

    [HttpPost(nameof(UploadFile))]

    public async Task<IActionResult> UploadFile(IFormFile file, string filePath)
    {
        await using var memoryStream = new MemoryStream();
        await file.CopyToAsync(memoryStream);

        var awsS3Request = new AwsS3Request()
        {
            InputStream = memoryStream,
            Name = filePath
        };

        var result = await _storageService.UploadFileAsync(awsS3Request);

        return Ok(result);
    }
}