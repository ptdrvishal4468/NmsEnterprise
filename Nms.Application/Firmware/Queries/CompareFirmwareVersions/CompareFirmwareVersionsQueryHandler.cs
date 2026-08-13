using MediatR;
using Nms.Application.Common.Interfaces;
using Nms.Application.Firmware.Dtos;

namespace Nms.Application.Firmware.Queries.CompareFirmwareVersions;

public class CompareFirmwareVersionsQueryHandler : IRequestHandler<CompareFirmwareVersionsQuery, FirmwareVersionComparisonDto>
{
    private readonly IFirmwareVersionComparator _versionComparator;

    public CompareFirmwareVersionsQueryHandler(IFirmwareVersionComparator versionComparator)
    {
        _versionComparator = versionComparator;
    }

    public Task<FirmwareVersionComparisonDto> Handle(CompareFirmwareVersionsQuery request, CancellationToken cancellationToken)
    {
        var cmp = _versionComparator.Compare(request.Version1, request.Version2);
        var compliance = _versionComparator.EvaluateCompliance(request.Version1, request.Version2);

        string relation = cmp switch
        {
            < 0 => $"{request.Version1} is OLDER than {request.Version2}",
            0 => $"{request.Version1} is EQUAL to {request.Version2}",
            > 0 => $"{request.Version1} is NEWER than {request.Version2}"
        };

        var result = new FirmwareVersionComparisonDto(
            request.Version1,
            request.Version2,
            cmp,
            relation,
            compliance);

        return Task.FromResult(result);
    }
}