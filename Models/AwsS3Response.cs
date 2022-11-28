namespace AwsStorageApi.Models;

public class AwsS3Response
{
    public int StatusCode { get; set; }
    public string Message { get; set; } = null!;
}
