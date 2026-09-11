using System.Text.Json;
using MediatR;
using Nms.Application.Common.Interfaces;
using Nms.Application.Reporting.Dtos;
using Nms.Domain.Enums;
using Nms.Domain.Interfaces;

namespace Nms.Application.Reporting.Queries.GenerateDeviceReport;

public class GenerateDeviceReportQueryHandler : IRequestHandler<GenerateDeviceReportQuery, ReportExportResultDto>
{
    private readonly IDeviceRepository _deviceRepository;
    private readonly ICsvReportFormatter _csvReportFormatter;

    public GenerateDeviceReportQueryHandler(
        IDeviceRepository deviceRepository,
        ICsvReportFormatter csvReportFormatter)
    {
        _deviceRepository = deviceRepository ?? throw new ArgumentNullException(nameof(deviceRepository));
        _csvReportFormatter = csvReportFormatter ?? throw new ArgumentNullException(nameof(csvReportFormatter));
    }

    public async Task<ReportExportResultDto> Handle(GenerateDeviceReportQuery request, CancellationToken cancellationToken)
    {
        var devices = await _deviceRepository.GetAllAsync(cancellationToken);
        var query = devices.AsEnumerable();

        if (request.DeviceType.HasValue)
        {
            query = query.Where(d => d.DeviceType == request.DeviceType.Value);
        }

        if (request.Status.HasValue)
        {
            query = query.Where(d => d.Status == request.Status.Value);
        }

        if (!string.IsNullOrWhiteSpace(request.Vendor))
        {
            query = query.Where(d => d.Vendor != null && d.Vendor.Equals(request.Vendor, StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrWhiteSpace(request.Site))
        {
            query = query.Where(d => d.Site != null && d.Site.Equals(request.Site, StringComparison.OrdinalIgnoreCase));
        }

        var records = query
            .OrderBy(d => d.Name)
            .Select(d => new DeviceReportDto
            {
                Id = d.Id,
                Name = d.Name,
                IpAddress = d.IpAddress,
                Hostname = d.Hostname,
                Vendor = d.Vendor,
                Model = d.Model,
                SerialNumber = d.SerialNumber,
                FirmwareVersion = d.FirmwareVersion,
                MacAddress = d.MacAddress,
                Site = d.Site,
                Location = d.Location,
                DeviceType = d.DeviceType,
                Status = d.Status,
                LastSeenUtc = d.LastSeenUtc
            })
            .ToList();

        var timestamp = DateTime.UtcNow.ToString("yyyyMMdd_HHmmss");

        if (request.Format == ReportFormat.Csv)
        {
            return new ReportExportResultDto
            {
                FileName = $"Device_Report_{timestamp}.csv",
                ContentType = "text/csv",
                Data = _csvReportFormatter.FormatToCsv(records),
                TotalRecords = records.Count
            };
        }

        return new ReportExportResultDto
        {
            FileName = $"Device_Report_{timestamp}.json",
            ContentType = "application/json",
            Data = JsonSerializer.SerializeToUtf8Bytes(records, new JsonSerializerOptions { WriteIndented = true }),
            TotalRecords = records.Count
        };
    }
}