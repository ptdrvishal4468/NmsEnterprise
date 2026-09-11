using System.Text.Json;
using MediatR;
using Nms.Application.Common.Interfaces;
using Nms.Application.Reporting.Dtos;
using Nms.Domain.Enums;
using Nms.Domain.Interfaces;

namespace Nms.Application.Reporting.Queries.GenerateInventoryReport;

public class GenerateInventoryReportQueryHandler : IRequestHandler<GenerateInventoryReportQuery, ReportExportResultDto>
{
    private readonly IDeviceRepository _deviceRepository;
    private readonly INetworkInterfaceRepository _networkInterfaceRepository;
    private readonly ICsvReportFormatter _csvReportFormatter;

    public GenerateInventoryReportQueryHandler(
        IDeviceRepository deviceRepository,
        INetworkInterfaceRepository networkInterfaceRepository,
        ICsvReportFormatter csvReportFormatter)
    {
        _deviceRepository = deviceRepository ?? throw new ArgumentNullException(nameof(deviceRepository));
        _networkInterfaceRepository = networkInterfaceRepository ?? throw new ArgumentNullException(nameof(networkInterfaceRepository));
        _csvReportFormatter = csvReportFormatter ?? throw new ArgumentNullException(nameof(csvReportFormatter));
    }

    public async Task<ReportExportResultDto> Handle(GenerateInventoryReportQuery request, CancellationToken cancellationToken)
    {
        var devices = await _deviceRepository.GetAllAsync(cancellationToken);
        var interfaces = await _networkInterfaceRepository.GetAllAsync(cancellationToken);

        var query = devices.AsEnumerable();

        if (!string.IsNullOrWhiteSpace(request.Vendor))
        {
            query = query.Where(d => d.Vendor != null && d.Vendor.Equals(request.Vendor, StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrWhiteSpace(request.Site))
        {
            query = query.Where(d => d.Site != null && d.Site.Equals(request.Site, StringComparison.OrdinalIgnoreCase));
        }

        var interfaceGroups = interfaces
            .GroupBy(i => i.DeviceId)
            .ToDictionary(g => g.Key, g => g.ToList());

        var records = query
            .OrderBy(d => d.Name)
            .Select(d =>
            {
                var devInterfaces = interfaceGroups.TryGetValue(d.Id, out var list) ? list : new List<Domain.Entities.NetworkInterface>();
                return new InventoryReportDto
                {
                    DeviceId = d.Id,
                    DeviceName = d.Name,
                    IpAddress = d.IpAddress,
                    Vendor = d.Vendor,
                    Model = d.Model,
                    SerialNumber = d.SerialNumber,
                    FirmwareVersion = d.FirmwareVersion,
                    Site = d.Site,
                    Location = d.Location,
                    TotalInterfaces = devInterfaces.Count,
                    ActiveInterfaces = devInterfaces.Count(i => i.OperStatus == InterfaceOperStatus.Up),
                    TotalBandwidthCapacityBps = devInterfaces.Sum(i => i.SpeedBps)
                };
            })
            .ToList();

        var timestamp = DateTime.UtcNow.ToString("yyyyMMdd_HHmmss");

        if (request.Format == ReportFormat.Csv)
        {
            return new ReportExportResultDto
            {
                FileName = $"Inventory_Report_{timestamp}.csv",
                ContentType = "text/csv",
                Data = _csvReportFormatter.FormatToCsv(records),
                TotalRecords = records.Count
            };
        }

        return new ReportExportResultDto
        {
            FileName = $"Inventory_Report_{timestamp}.json",
            ContentType = "application/json",
            Data = JsonSerializer.SerializeToUtf8Bytes(records, new JsonSerializerOptions { WriteIndented = true }),
            TotalRecords = records.Count
        };
    }
}