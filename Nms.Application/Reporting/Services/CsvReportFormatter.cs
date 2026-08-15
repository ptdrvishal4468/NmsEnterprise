using System.Reflection;
using System.Text;
using Nms.Application.Common.Interfaces;

namespace Nms.Application.Reporting.Services;

public class CsvReportFormatter : ICsvReportFormatter
{
    public byte[] FormatToCsv<T>(IEnumerable<T> records)
    {
        var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
        var sb = new StringBuilder();

        // Write CSV header
        var header = string.Join(",", properties.Select(p => EscapeValue(p.Name)));
        sb.AppendLine(header);

        // Write data rows
        foreach (var record in records)
        {
            var line = string.Join(",", properties.Select(p =>
            {
                var value = p.GetValue(record);
                return FormatCell(value);
            }));
            sb.AppendLine(line);
        }

        return Encoding.UTF8.GetBytes(sb.ToString());
    }

    private static string FormatCell(object? value)
    {
        if (value == null) return string.Empty;

        if (value is DateTime dt)
        {
            return EscapeValue(dt.ToString("yyyy-MM-dd HH:mm:ss 'UTC'"));
        }

        return EscapeValue(value.ToString() ?? string.Empty);
    }

    private static string EscapeValue(string value)
    {
        if (string.IsNullOrEmpty(value)) return string.Empty;

        if (value.Contains(',') || value.Contains('"') || value.Contains('\n') || value.Contains('\r'))
        {
            return $"\"{value.Replace("\"", "\"\"")}\"";
        }

        return value;
    }
}