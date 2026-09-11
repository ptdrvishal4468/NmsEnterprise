namespace Nms.Application.Common.Interfaces;

public interface ICsvReportFormatter
{
    byte[] FormatToCsv<T>(IEnumerable<T> records);
}