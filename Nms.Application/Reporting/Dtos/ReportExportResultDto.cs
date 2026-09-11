using Nms.Domain.Enums;

namespace Nms.Application.Reporting.Dtos;

public class ReportExportResultDto
{
    public string FileName { get; set; } = string.Empty;
    public string ContentType { get; set; } = "application/json";
    public byte[] Data { get; set; } = Array.Empty<byte>();
    public int TotalRecords { get; set; }
}