namespace Nms.Application.Common.Interfaces;

public interface IServiceNowClient
{
    Task<(string SysId, string Number, string? Url)> CreateIncidentAsync(string shortDescription, string description, string urgency, CancellationToken cancellationToken = default);
    Task UpdateIncidentAsync(string sysId, string status, string? comments, CancellationToken cancellationToken = default);
    Task<string?> GetIncidentStatusAsync(string sysId, CancellationToken cancellationToken = default);
}