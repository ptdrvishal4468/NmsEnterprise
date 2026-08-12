using MediatR;
using Nms.Application.Common.Models;
using Nms.Application.Notifications.Dtos;

namespace Nms.Application.Notifications.Queries.GetNotificationLogsPaged;

public record GetNotificationLogsPagedQuery(int PageIndex = 1, int PageSize = 10) : IRequest<PagedResult<NotificationLogDto>>;