using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nms.Application.Dashboard.Dtos;
using Nms.Application.Dashboard.Queries.GetAlertSummary;
using Nms.Application.Dashboard.Queries.GetDeviceStatistics;
using Nms.Application.Dashboard.Queries.GetExecutiveDashboard;
using Nms.Application.Dashboard.Queries.GetHealthSummary;
using Nms.Application.Dashboard.Queries.GetTenantDashboard;
using Nms.Domain.Constants;
using Nms.Infrastructure.Security;

namespace Nms.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class DashboardController : ControllerBase
{
    private readonly ISender _sender;

    public DashboardController(ISender sender)
    {
        _sender = sender;
    }

    /// <summary>
    /// Gets the executive dashboard providing high-level operational metrics across devices, alerts, health, and topology.
    /// </summary>
    [HttpGet("executive")]
    [HasPermission(Permissions.Dashboard.ViewExecutive)]
    [ProducesResponseType(typeof(ExecutiveDashboardDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<ExecutiveDashboardDto>> GetExecutiveDashboard(
        CancellationToken cancellationToken = default)
    {
        var query = new GetExecutiveDashboardQuery();
        var result = await _sender.Send(query, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Gets multi-dimensional device statistics including status, type, vendor, and site distributions.
    /// </summary>
    [HttpGet("devices")]
    [HasPermission(Permissions.Dashboard.View)]
    [ProducesResponseType(typeof(DeviceStatisticsDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<DeviceStatisticsDto>> GetDeviceStatistics(
        CancellationToken cancellationToken = default)
    {
        var query = new GetDeviceStatisticsQuery();
        var result = await _sender.Send(query, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Gets the system health summary including score distributions and top degraded devices.
    /// </summary>
    [HttpGet("health")]
    [HasPermission(Permissions.Dashboard.View)]
    [ProducesResponseType(typeof(HealthSummaryDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<HealthSummaryDto>> GetHealthSummary(
        [FromQuery] int topDegraded = 5,
        CancellationToken cancellationToken = default)
    {
        var query = new GetHealthSummaryQuery(topDegraded);
        var result = await _sender.Send(query, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Gets the operational alert summary including active/acknowledged counts, severity breakdowns, and top firing rules.
    /// </summary>
    [HttpGet("alerts")]
    [HasPermission(Permissions.Dashboard.View)]
    [ProducesResponseType(typeof(AlertSummaryDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<AlertSummaryDto>> GetAlertSummary(
        [FromQuery] int topRules = 5,
        CancellationToken cancellationToken = default)
    {
        var query = new GetAlertSummaryQuery(topRules);
        var result = await _sender.Send(query, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Gets the tenant-specific dashboard overview.
    /// </summary>
    [HttpGet("tenant")]
    [HasPermission(Permissions.Dashboard.ViewTenant)]
    [ProducesResponseType(typeof(TenantDashboardDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<TenantDashboardDto>> GetTenantDashboard(
        [FromQuery] Guid? tenantId = null,
        CancellationToken cancellationToken = default)
    {
        var query = new GetTenantDashboardQuery(tenantId);
        var result = await _sender.Send(query, cancellationToken);
        return Ok(result);
    }
}