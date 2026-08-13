using MediatR;
using Nms.Application.Common.Interfaces;
using Nms.Application.Firmware.Dtos;
using Nms.Domain.Entities;
using Nms.Domain.Interfaces;

namespace Nms.Application.Firmware.Commands.CreateFirmwareBaseline;

public class CreateFirmwareBaselineCommandHandler : IRequestHandler<CreateFirmwareBaselineCommand, FirmwareBaselineDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITenantContext _tenantContext;

    public CreateFirmwareBaselineCommandHandler(IUnitOfWork unitOfWork, ITenantContext tenantContext)
    {
        _unitOfWork = unitOfWork;
        _tenantContext = tenantContext;
    }

    public async Task<FirmwareBaselineDto> Handle(CreateFirmwareBaselineCommand request, CancellationToken cancellationToken)
    {
        var tenantId = _tenantContext.TenantId;

        var existing = await _unitOfWork.FirmwareBaselines.GetByVendorAndModelAsync(tenantId, request.Vendor, request.Model, cancellationToken);
        if (existing != null)
        {
            existing.UpdateBaseline(request.TargetVersion, request.Notes, request.IsActive);
            _unitOfWork.FirmwareBaselines.Update(existing);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new FirmwareBaselineDto(
                existing.Id,
                existing.TenantId,
                existing.Vendor,
                existing.Model,
                existing.TargetVersion,
                existing.IsActive,
                existing.Notes,
                existing.CreatedAtUtc,
                existing.CreatedBy);
        }

        var baseline = new FirmwareBaseline(
            Guid.NewGuid(),
            tenantId,
            request.Vendor,
            request.Model,
            request.TargetVersion,
            request.Notes,
            request.IsActive);

        await _unitOfWork.FirmwareBaselines.AddAsync(baseline, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new FirmwareBaselineDto(
            baseline.Id,
            baseline.TenantId,
            baseline.Vendor,
            baseline.Model,
            baseline.TargetVersion,
            baseline.IsActive,
            baseline.Notes,
            baseline.CreatedAtUtc,
            baseline.CreatedBy);
    }
}