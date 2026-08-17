using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Nms.Application.Alerts.Services;
using Nms.Application.Auth.Commands.Login;
using Nms.Application.Common.Behaviors;
using Nms.Application.Common.Interfaces;
using Nms.Application.Customers.Commands.AddCustomerContact;
using Nms.Application.Customers.Commands.CreateCustomer;
using Nms.Application.Customers.Commands.DeleteCustomer;
using Nms.Application.Customers.Commands.DeleteCustomerContact;
using Nms.Application.Customers.Commands.UpdateCustomer;
using Nms.Application.Customers.Commands.UpdateCustomerContact;
using Nms.Application.Customers.Queries.GetCustomerById;
using Nms.Application.Customers.Queries.GetCustomerContacts;
using Nms.Application.Customers.Queries.GetCustomerHierarchy;
using Nms.Application.Customers.Queries.GetCustomersPaged;
using Nms.Application.Devices.Commands.CreateDevice;
using Nms.Application.Devices.Commands.DeleteDevice;
using Nms.Application.Devices.Commands.UpdateDevice;
using Nms.Application.Devices.Queries.GetDeviceById;
using Nms.Application.Devices.Queries.GetDevicesPaged;
using Nms.Application.Discovery.Commands.ImportDiscoveredDevice;
using Nms.Application.Discovery.Commands.StartDiscoveryScan;
using Nms.Application.Discovery.Queries.GetDiscoveryJobById;
using Nms.Application.Discovery.Services;
using Nms.Application.Events.Services;
using Nms.Application.Firmware.Services;
using Nms.Application.Health.Services;
using Nms.Application.Notifications.Commands.CreateNotificationTemplate;
using Nms.Application.Notifications.Commands.DeleteNotificationTemplate;
using Nms.Application.Notifications.Commands.SendTestNotification;
using Nms.Application.Notifications.Commands.UpdateNotificationTemplate;
using Nms.Application.Notifications.Queries.GetNotificationLogsPaged;
using Nms.Application.Notifications.Queries.GetNotificationTemplatesPaged;
using Nms.Application.Notifications.Services;
using Nms.Application.Reporting.Services;
using Nms.Application.Syslog.Commands.IngestSyslogMessage;
using Nms.Application.Syslog.Queries.GetSyslogById;
using Nms.Application.Syslog.Queries.GetSyslogsPaged;
using Nms.Application.Telemetry.Commands.PollDevice;
using Nms.Application.Telemetry.Commands.ProcessTelemetryData;
using Nms.Application.Telemetry.Queries.GetDeviceMetrics;
using Nms.Application.Ticketing.Commands.CloseTicket;
using Nms.Application.Ticketing.Commands.CreateTicket;
using Nms.Application.Ticketing.Commands.SyncTicket;
using Nms.Application.Ticketing.Commands.UpdateTicket;
using Nms.Application.Ticketing.Queries.GetTicketById;
using Nms.Application.Ticketing.Queries.GetTicketsPaged;
using Nms.Application.Ticketing.Queries.GetTicketSyncLogs;
using Nms.Application.Topology.Services;
using Nms.Application.Users.Commands.CreateUser;
using Nms.Application.Users.Commands.UpdateUserRoles;
using Nms.Application.Users.Queries.GetUserById;
using Nms.Application.Users.Queries.GetUsersPaged;

namespace Nms.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationLayer(this IServiceCollection services)
    {
        var assembly = typeof(DependencyInjection).Assembly;

        // Automatic FluentValidation registration
        services.AddValidatorsFromAssembly(assembly);

        // MediatR Registration with Validation Pipeline Behavior
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(assembly);
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        });

        // Auth Command Handlers
        services.AddScoped<LoginCommandHandler>();

        // User Command & Query Handlers
        services.AddScoped<CreateUserCommandHandler>();
        services.AddScoped<UpdateUserRolesCommandHandler>();
        services.AddScoped<GetUserByIdQueryHandler>();
        services.AddScoped<GetUsersPagedQueryHandler>();

        // Device Command & Query Handlers
        services.AddScoped<CreateDeviceCommandHandler>();
        services.AddScoped<UpdateDeviceCommandHandler>();
        services.AddScoped<DeleteDeviceCommandHandler>();
        services.AddScoped<GetDeviceByIdQueryHandler>();
        services.AddScoped<GetDevicesPagedQueryHandler>();

        // Telemetry Command & Query Handlers
        services.AddScoped<PollDeviceCommandHandler>();
        services.AddScoped<ProcessTelemetryDataCommandHandler>();
        services.AddScoped<GetDeviceMetricsQueryHandler>();

        // Discovery Services & Handlers
        services.AddScoped<DuplicateDetector>();
        services.AddScoped<DeviceDiscoveryEngine>();
        services.AddScoped<StartDiscoveryScanCommandHandler>();
        services.AddScoped<ImportDiscoveredDeviceCommandHandler>();
        services.AddScoped<GetDiscoveryJobByIdQueryHandler>();

        // Health & Alert Services
        services.AddScoped<IDeviceHealthCalculator, DeviceHealthCalculator>();
        services.AddScoped<IAlertEvaluationEngine, AlertEvaluationEngine>();

        // Notification Services & Handlers
        services.AddScoped<INotificationTemplateEngine, NotificationTemplateEngine>();
        services.AddScoped<INotificationDispatcher, NotificationDispatcher>();
        services.AddScoped<CreateNotificationTemplateCommandHandler>();
        services.AddScoped<UpdateNotificationTemplateCommandHandler>();
        services.AddScoped<DeleteNotificationTemplateCommandHandler>();
        services.AddScoped<SendTestNotificationCommandHandler>();
        services.AddScoped<GetNotificationTemplatesPagedQueryHandler>();
        services.AddScoped<GetNotificationLogsPagedQueryHandler>();

        // Syslog Command & Query Handlers
        services.AddScoped<IngestSyslogMessageCommandHandler>();
        services.AddScoped<GetSyslogsPagedQueryHandler>();
        services.AddScoped<GetSyslogByIdQueryHandler>();

        // Event Publisher
        services.AddScoped<IEventPublisher, EventPublisher>();

        // Firmware Version Comparator
        services.AddSingleton<IFirmwareVersionComparator, FirmwareVersionComparator>();

        // Topology Graph Builder
        services.AddScoped<ITopologyGraphBuilder, TopologyGraphBuilder>();

        // Reporting Services
        services.AddSingleton<ICsvReportFormatter, CsvReportFormatter>();

        // Customer Command & Query Handlers
        services.AddScoped<CreateCustomerCommandHandler>();
        services.AddScoped<UpdateCustomerCommandHandler>();
        services.AddScoped<DeleteCustomerCommandHandler>();
        services.AddScoped<AddCustomerContactCommandHandler>();
        services.AddScoped<UpdateCustomerContactCommandHandler>();
        services.AddScoped<DeleteCustomerContactCommandHandler>();
        services.AddScoped<GetCustomerByIdQueryHandler>();
        services.AddScoped<GetCustomerHierarchyQueryHandler>();
        services.AddScoped<GetCustomersPagedQueryHandler>();
        services.AddScoped<GetCustomerContactsQueryHandler>();

        // Ticketing Command & Query Handlers
        services.AddScoped<CreateTicketCommandHandler>();
        services.AddScoped<UpdateTicketCommandHandler>();
        services.AddScoped<CloseTicketCommandHandler>();
        services.AddScoped<SyncTicketCommandHandler>();
        services.AddScoped<GetTicketByIdQueryHandler>();
        services.AddScoped<GetTicketsPagedQueryHandler>();
        services.AddScoped<GetTicketSyncLogsQueryHandler>();

        return services;
    }
}