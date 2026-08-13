namespace Nms.Application.ConfigurationBackups.Dtos;

public record ConfigurationDownloadDto
{
    public string FileName { get; init; } = string.Empty;
    public string ContentType { get; init; } = "text/plain";
    public string RawConfigurationContent { get; init; } = string.Empty;
    public string ChecksumSha256 { get; init; } = string.Empty;
}