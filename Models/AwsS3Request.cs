namespace AwsStorageApi.Models;
public class AwsS3Request
{
    public string Name { get; set; } = null!;
    public MemoryStream InputStream { get; set; } = null!;
}
