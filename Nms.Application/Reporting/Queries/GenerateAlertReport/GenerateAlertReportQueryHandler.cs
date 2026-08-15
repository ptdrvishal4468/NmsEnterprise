using System.Text.Json;
using MediatR;
using Nms.Application.Common.Interfaces;
using Nms.Application.Reporting.Dtos;
using Nms.Domain.Enums;
using Nms.Domain.Interfaces;

namespace Nms.Application.Reporting.Queries.GenerateAlertReport;

public class GenerateAlertReportQueryHandler : IRequestHandler<GenerateAlertReportQuery, ReportExportResultDto>
{
    private readonly IAlertRepository _alertRepository;
    private readonly IDeviceRepository _deviceRepository;
    private readonly ICsvReportFormatter _csvReportFormatter;

    public GenerateAlertReportQueryHandler(
        IAlertRepository alertRepository,
        IDeviceRepository deviceRepository,
        ICsvReportFormatter csvReportFormatter)
    {
        _alertRepository = alertRepository ?? throw new ArgumentNullException(nameof(alertRepository));
        _deviceRepository = deviceRepository ?? throw new ArgumentNullException(nameof(deviceRepository));
        _csvReportFormatter = csvReportFormatter ?? throw new ArgumentNullException(nameof(csvReportFormatter));
    }

    public async Task<ReportExportResultDto> Handle(GenerateAlertReportQuery request, CancellationToken cancellationToken)
    {
        var alerts = await _alertRepository.GetAllAsync(cancellationToken);
        var devices = await _deviceRepository.GetAllAsync(cancellationToken);
        var deviceLookup = devices.ToDictionary(d => d.Id);

        var query = alerts.AsEnumerable();

        if (request.Severity.HasValue)
        {
            query = query.Where(a => a.Severity == request.Severity.Value);
        }

        if (request.State.HasValue)
        {
            query = query.Where(a => a.State == request.State.Value);
        }

        if (request.MetricType.HasValue)
        {
            query = query.Where(a => a.MetricType == request.MetricType.Value);
        }

        if (request.FromUtc.HasValue)
        {
            query = query.Where(a => a.TriggeredAtUtc >= request.FromUtc.Value);
        }

        if (request.ToUtc.HasValue)
        {
            query = query.Where(a => a.TriggeredAtUtc <= request.ToUtc.Value);
        }

        var records = query
            .OrderByDescending(a => a.TriggeredAtUtc)
            .Select(a =>
            {
                var device = deviceLookup.TryGetValue(a.DeviceId, out var dev) ? dev : null;
                return new AlertReportDto
                {
                    AlertId = a.Id,
                    DeviceId = a.DeviceId,
                    DeviceName = device != null ? device.Name : "Unknown",
                    IpAddress = device != null ? device.IpAddress : "Unknown",
                    MetricType = a.MetricType,
                    Severity = a.Severity,
                    State = a.State,
                    MetricValue = a.MetricValue,
                    ThresholdValue = a.ThresholdValue,
                    Message = a.Message,
                    TriggeredAtUtc = a.TriggeredAtUtc,
                    ResolvedAtUtc = a.ResolvedAtUtc,
                    AcknowledgedBy = a.AcknowledgedBy,
                    SuppressedBy = a.SuppressedBy
                };
            })
            .ToList();

        var timestamp = DateTime.UtcNow.ToString("yyyyMMdd_HHmmss");

        if (request.Format == ReportFormat.Csv)
        {
            return new ReportExportResultDto
            {
                FileName = $"Alert_Report_{timestamp}.csv",
                ContentType = "text/csv",
                Data = _csvReportFormatter.FormatToCsv(records),
                TotalRecords = records.Count
            };
        }

        return new ReportExportResultDto
        {
            FileName = $"Alert_Report_{timestamp}.json",
            ContentType = "application/json",
            Data = JsonSerializer.SerializeToUtf8Bytes(records, new JsonSerializerOptions { WriteIndented = true }),
            TotalRecords = records.Count
        };
    }
}