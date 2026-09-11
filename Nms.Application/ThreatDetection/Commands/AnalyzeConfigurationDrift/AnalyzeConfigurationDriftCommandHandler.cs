using MediatR;
using Nms.Application.Common.Interfaces;
using Nms.Application.ThreatDetection.Dtos;

namespace Nms.Application.ThreatDetection.Commands.AnalyzeConfigurationDrift;

public class AnalyzeConfigurationDriftCommandHandler : IRequestHandler<AnalyzeConfigurationDriftCommand, IReadOnlyList<ConfigurationDriftRecordDto>>
{
    private readonly IConfigurationDriftDetector _driftDetector;

    public AnalyzeConfigurationDriftCommandHandler(IConfigurationDriftDetector driftDetector)
    {
        _driftDetector = driftDetector;
    }

    public async Task<IReadOnlyList<ConfigurationDriftRecordDto>> Handle(AnalyzeConfigurationDriftCommand request, CancellationToken cancellationToken)
    {
        var records = new List<ConfigurationDriftRecordDto>();

        if (request.DeviceId.HasValue)
        {
            var record = await _driftDetector.AnalyzeDeviceDriftAsync(request.DeviceId.Value, cancellationToken);
            records.Add(MapToDto(record));
        }
        else
        {
            var list = await _driftDetector.AnalyzeAllDevicesDriftAsync(cancellationToken);
            records.AddRange(list.Select(MapToDto));
        }

        return records;
    }

    private static ConfigurationDriftRecordDto MapToDto(Domain.Entities.ConfigurationDriftRecord r) => new()
    {
        Id = r.Id,
        TenantId = r.TenantId,
        DeviceId = r.DeviceId,
        DeviceName = r.Device?.Name ?? string.Empty,
        BaselineBackupId = r.BaselineBackupId,
        CurrentBackupId = r.CurrentBackupId,
        HasDrift = r.HasDrift,
        AddedLinesCount = r.AddedLinesCount,
        RemovedLinesCount = r.RemovedLinesCount,
        ModifiedLinesCount = r.ModifiedLinesCount,
        DifferencesJson = r.DifferencesJson,
        DetectedAtUtc = r.DetectedAtUtc,
        Severity = r.Severity,
        IsAcknowledged = r.IsAcknowledged,
        AcknowledgmentNotes = r.AcknowledgmentNotes
    };
}