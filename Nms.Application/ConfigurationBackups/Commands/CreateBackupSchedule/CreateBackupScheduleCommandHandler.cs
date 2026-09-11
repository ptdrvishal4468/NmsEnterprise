using MediatR;
using Nms.Application.ConfigurationBackups.Dtos;
using Nms.Domain.Entities;
using Nms.Domain.Interfaces;

namespace Nms.Application.ConfigurationBackups.Commands.CreateBackupSchedule;

public class CreateBackupScheduleCommandHandler : IRequestHandler<CreateBackupScheduleCommand, BackupScheduleDto>
{
    private readonly IUnitOfWork _unitOfWork;

    public CreateBackupScheduleCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<BackupScheduleDto> Handle(CreateBackupScheduleCommand request, CancellationToken cancellationToken)
    {
        if (request.DeviceId.HasValue)
        {
            var device = await _unitOfWork.Devices.GetByIdAsync(request.DeviceId.Value, cancellationToken);
            if (device == null || device.TenantId != request.TenantId)
            {
                throw new InvalidOperationException($"Target device '{request.DeviceId.Value}' not found for tenant '{request.TenantId}'.");
            }
        }

        var schedule = new BackupSchedule(
            Guid.NewGuid(),
            request.TenantId,
            request.Name,
            request.IntervalMinutes,
            request.DeviceId,
            request.Description);

        await _unitOfWork.BackupSchedules.AddAsync(schedule, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new BackupScheduleDto
        {
            Id = schedule.Id,
            TenantId = schedule.TenantId,
            Name = schedule.Name,
            Description = schedule.Description,
            DeviceId = schedule.DeviceId,
            DeviceName = schedule.Device?.Name,
            IntervalMinutes = schedule.IntervalMinutes,
            IsEnabled = schedule.IsEnabled,
            LastRunUtc = schedule.LastRunUtc,
            NextRunUtc = schedule.NextRunUtc,
            CreatedAtUtc = schedule.CreatedAtUtc
        };
    }
}