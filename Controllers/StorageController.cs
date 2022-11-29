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
    public async Task<IActionResult> UploadFile(IFormFile file, string fileName)
    {
        if (_storageService.IsFileExists(fileName))
            return BadRequest($"File {fileName} already exists");

        await using var memoryStream = new MemoryStream();
        await file.CopyToAsync(memoryStream);

        var awsS3Request = new AwsS3Request
        {
            InputStream = memoryStream,
            Name = fileName
        };

        var resultUpload = await _storageService.UploadFileAsync(awsS3Request);

        return Ok(resultUpload);
    }

    [HttpGet(nameof(DownloadFile))]
    public async Task<IActionResult> DownloadFile(string fileName)
    {
        if (string.IsNullOrEmpty(fileName))
            return BadRequest("The 'documentName' parameter is required");

        if (!_storageService.IsFileExists(fileName))
            return BadRequest("The file does not exist");

        var document = await _storageService.DownloadFileAsync(fileName);

        return File(document, "application/octet-stream", fileName);
    }

    [HttpDelete(nameof(DeleteFile))]
    public async Task<IActionResult> DeleteFile(string fileName)
    {
        if (string.IsNullOrEmpty(fileName))
            return BadRequest("The 'documentName' parameter is required");

        if (!_storageService.IsFileExists(fileName))
            return BadRequest("The file does not exist");

        await _storageService.DeleteFileAsync(fileName);

        return Ok($"The document '{fileName}' is deleted successfully");
    }
}