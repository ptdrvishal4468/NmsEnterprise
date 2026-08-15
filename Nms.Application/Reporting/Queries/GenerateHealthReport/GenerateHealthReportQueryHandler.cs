using System.Text.Json;
using MediatR;
using Nms.Application.Common.Interfaces;
using Nms.Application.Reporting.Dtos;
using Nms.Domain.Enums;
using Nms.Domain.Interfaces;

namespace Nms.Application.Reporting.Queries.GenerateHealthReport;

public class GenerateHealthReportQueryHandler : IRequestHandler<GenerateHealthReportQuery, ReportExportResultDto>
{
    private readonly IDeviceHealthHistoryRepository _healthHistoryRepository;
    private readonly IDeviceRepository _deviceRepository;
    private readonly ICsvReportFormatter _csvReportFormatter;

    public GenerateHealthReportQueryHandler(
        IDeviceHealthHistoryRepository healthHistoryRepository,
        IDeviceRepository deviceRepository,
        ICsvReportFormatter csvReportFormatter)
    {
        _healthHistoryRepository = healthHistoryRepository ?? throw new ArgumentNullException(nameof(healthHistoryRepository));
        _deviceRepository = deviceRepository ?? throw new ArgumentNullException(nameof(deviceRepository));
        _csvReportFormatter = csvReportFormatter ?? throw new ArgumentNullException(nameof(csvReportFormatter));
    }

    public async Task<ReportExportResultDto> Handle(GenerateHealthReportQuery request, CancellationToken cancellationToken)
    {
        var histories = await _healthHistoryRepository.GetAllAsync(cancellationToken);
        var devices = await _deviceRepository.GetAllAsync(cancellationToken);
        var deviceLookup = devices.ToDictionary(d => d.Id);

        var query = histories.AsEnumerable();

        if (request.Status.HasValue)
        {
            query = query.Where(h => h.Status == request.Status.Value);
        }

        if (request.MinHealthScore.HasValue)
        {
            query = query.Where(h => h.HealthScore >= request.MinHealthScore.Value);
        }

        if (request.MaxHealthScore.HasValue)
        {
            query = query.Where(h => h.HealthScore <= request.MaxHealthScore.Value);
        }

        if (request.FromUtc.HasValue)
        {
            query = query.Where(h => h.TimestampUtc >= request.FromUtc.Value);
        }

        if (request.ToUtc.HasValue)
        {
            query = query.Where(h => h.TimestampUtc <= request.ToUtc.Value);
        }

        var records = query
            .OrderByDescending(h => h.TimestampUtc)
            .Select(h =>
            {
                var device = deviceLookup.TryGetValue(h.DeviceId, out var dev) ? dev : null;
                return new DeviceHealthReportDto
                {
                    DeviceId = h.DeviceId,
                    DeviceName = device != null ? device.Name : "Unknown",
                    IpAddress = device != null ? device.IpAddress : "Unknown",
                    DeviceType = device != null ? device.DeviceType : DeviceType.Unknown,
                    Status = h.Status,
                    HealthScore = h.HealthScore,
                    HealthReason = h.Reason,
                    LastEvaluatedUtc = h.TimestampUtc
                };
            })
            .ToList();

        var timestamp = DateTime.UtcNow.ToString("yyyyMMdd_HHmmss");

        if (request.Format == ReportFormat.Csv)
        {
            return new ReportExportResultDto
            {
                FileName = $"Health_Report_{timestamp}.csv",
                ContentType = "text/csv",
                Data = _csvReportFormatter.FormatToCsv(records),
                TotalRecords = records.Count
            };
        }

        return new ReportExportResultDto
        {
            FileName = $"Health_Report_{timestamp}.json",
            ContentType = "application/json",
            Data = JsonSerializer.SerializeToUtf8Bytes(records, new JsonSerializerOptions { WriteIndented = true }),
            TotalRecords = records.Count
        };
    }
}