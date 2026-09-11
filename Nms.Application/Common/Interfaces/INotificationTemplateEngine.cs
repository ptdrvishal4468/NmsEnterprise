using Nms.Domain.Entities;

namespace Nms.Application.Common.Interfaces;

public interface INotificationTemplateEngine
{
    string Render(string templateText, Alert alert, Device? device = null);
}