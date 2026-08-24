IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260729121611_InitialCreate'
)
BEGIN
    CREATE TABLE [AuditLogs] (
        [Id] bigint NOT NULL IDENTITY,
        [TenantId] uniqueidentifier NOT NULL,
        [UserId] uniqueidentifier NOT NULL,
        [Action] varchar(100) NOT NULL,
        [OldValuesJson] nvarchar(max) NULL,
        [NewValuesJson] nvarchar(max) NULL,
        [TimestampUtc] datetime2 NOT NULL DEFAULT (SYSUTCDATETIME()),
        CONSTRAINT [PK_AuditLogs] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260729121611_InitialCreate'
)
BEGIN
    CREATE TABLE [Permissions] (
        [Id] int NOT NULL IDENTITY,
        [PermissionKey] varchar(100) NOT NULL,
        [Description] nvarchar(250) NULL,
        CONSTRAINT [PK_Permissions] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260729121611_InitialCreate'
)
BEGIN
    CREATE TABLE [Tenants] (
        [Id] uniqueidentifier NOT NULL DEFAULT (NEWSEQUENTIALID()),
        [Name] nvarchar(100) NOT NULL,
        [IsActive] bit NOT NULL DEFAULT CAST(1 AS bit),
        [CreatedAtUtc] datetime2 NOT NULL DEFAULT (SYSUTCDATETIME()),
        CONSTRAINT [PK_Tenants] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260729121611_InitialCreate'
)
BEGIN
    CREATE TABLE [Devices] (
        [Id] uniqueidentifier NOT NULL DEFAULT (NEWSEQUENTIALID()),
        [TenantId] uniqueidentifier NOT NULL,
        [Name] nvarchar(150) NOT NULL,
        [IpAddress] varchar(45) NOT NULL,
        [DeviceType] int NOT NULL,
        [SnmpPort] int NOT NULL DEFAULT 161,
        [SnmpV3User] nvarchar(100) NULL,
        [SnmpV3AuthKeyEncrypted] nvarchar(max) NULL,
        [SnmpV3PrivKeyEncrypted] nvarchar(max) NULL,
        [Status] int NOT NULL DEFAULT 0,
        [LastSeenUtc] datetime2 NULL,
        [CreatedAtUtc] datetime2 NOT NULL,
        [CreatedBy] nvarchar(max) NULL,
        [LastModifiedAtUtc] datetime2 NULL,
        [LastModifiedBy] nvarchar(max) NULL,
        CONSTRAINT [PK_Devices] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Devices_Tenants_TenantId] FOREIGN KEY ([TenantId]) REFERENCES [Tenants] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260729121611_InitialCreate'
)
BEGIN
    CREATE TABLE [Roles] (
        [Id] uniqueidentifier NOT NULL DEFAULT (NEWSEQUENTIALID()),
        [TenantId] uniqueidentifier NOT NULL,
        [Name] nvarchar(50) NOT NULL,
        [IsSystemDefault] bit NOT NULL DEFAULT CAST(0 AS bit),
        CONSTRAINT [PK_Roles] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Roles_Tenants_TenantId] FOREIGN KEY ([TenantId]) REFERENCES [Tenants] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260729121611_InitialCreate'
)
BEGIN
    CREATE TABLE [Users] (
        [Id] uniqueidentifier NOT NULL DEFAULT (NEWSEQUENTIALID()),
        [TenantId] uniqueidentifier NOT NULL,
        [Email] nvarchar(256) NOT NULL,
        [PasswordHash] nvarchar(max) NOT NULL,
        [IsActive] bit NOT NULL DEFAULT CAST(1 AS bit),
        [CreatedAtUtc] datetime2 NOT NULL,
        [CreatedBy] nvarchar(max) NULL,
        [LastModifiedAtUtc] datetime2 NULL,
        [LastModifiedBy] nvarchar(max) NULL,
        CONSTRAINT [PK_Users] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Users_Tenants_TenantId] FOREIGN KEY ([TenantId]) REFERENCES [Tenants] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260729121611_InitialCreate'
)
BEGIN
    CREATE TABLE [DeviceMetricsRaw] (
        [Id] bigint NOT NULL IDENTITY,
        [DeviceId] uniqueidentifier NOT NULL,
        [CpuUtilization] decimal(5,2) NOT NULL,
        [RamUtilization] decimal(5,2) NOT NULL,
        [LatencyMs] int NOT NULL,
        [TimestampUtc] datetime2 NOT NULL,
        CONSTRAINT [PK_DeviceMetricsRaw] PRIMARY KEY NONCLUSTERED ([Id]),
        CONSTRAINT [FK_DeviceMetricsRaw_Devices_DeviceId] FOREIGN KEY ([DeviceId]) REFERENCES [Devices] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260729121611_InitialCreate'
)
BEGIN
    CREATE TABLE [RolePermissions] (
        [RoleId] uniqueidentifier NOT NULL,
        [PermissionId] int NOT NULL,
        CONSTRAINT [PK_RolePermissions] PRIMARY KEY ([RoleId], [PermissionId]),
        CONSTRAINT [FK_RolePermissions_Permissions_PermissionId] FOREIGN KEY ([PermissionId]) REFERENCES [Permissions] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_RolePermissions_Roles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [Roles] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260729121611_InitialCreate'
)
BEGIN
    CREATE TABLE [RefreshTokens] (
        [Id] uniqueidentifier NOT NULL DEFAULT (NEWSEQUENTIALID()),
        [UserId] uniqueidentifier NOT NULL,
        [TokenHash] nvarchar(256) NOT NULL,
        [ExpiresAtUtc] datetime2 NOT NULL,
        [IsRevoked] bit NOT NULL DEFAULT CAST(0 AS bit),
        [CreatedAtUtc] datetime2 NOT NULL DEFAULT (SYSUTCDATETIME()),
        CONSTRAINT [PK_RefreshTokens] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_RefreshTokens_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260729121611_InitialCreate'
)
BEGIN
    CREATE TABLE [UserRoles] (
        [UserId] uniqueidentifier NOT NULL,
        [RoleId] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_UserRoles] PRIMARY KEY ([UserId], [RoleId]),
        CONSTRAINT [FK_UserRoles_Roles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [Roles] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_UserRoles_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260729121611_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_AuditLogs_TenantId] ON [AuditLogs] ([TenantId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260729121611_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_AuditLogs_TimestampUtc] ON [AuditLogs] ([TimestampUtc]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260729121611_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_DeviceMetricsRaw_DeviceId] ON [DeviceMetricsRaw] ([DeviceId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260729121611_InitialCreate'
)
BEGIN
    CREATE CLUSTERED INDEX [IX_DeviceMetricsRaw_TimestampUtc] ON [DeviceMetricsRaw] ([TimestampUtc]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260729121611_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Devices_TenantId_Status] ON [Devices] ([TenantId], [Status]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260729121611_InitialCreate'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Permissions_PermissionKey] ON [Permissions] ([PermissionKey]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260729121611_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_RefreshTokens_UserId] ON [RefreshTokens] ([UserId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260729121611_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_RolePermissions_PermissionId] ON [RolePermissions] ([PermissionId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260729121611_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Roles_TenantId] ON [Roles] ([TenantId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260729121611_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_UserRoles_RoleId] ON [UserRoles] ([RoleId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260729121611_InitialCreate'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Users_Email] ON [Users] ([Email]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260729121611_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Users_TenantId] ON [Users] ([TenantId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260729121611_InitialCreate'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260729121611_InitialCreate', N'10.0.11');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260801074617_SeedPermissionsTaxonomy'
)
BEGIN
    ALTER TABLE [DeviceMetricsRaw] DROP CONSTRAINT [FK_DeviceMetricsRaw_Devices_DeviceId];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260801074617_SeedPermissionsTaxonomy'
)
BEGIN
    ALTER TABLE [DeviceMetricsRaw] DROP CONSTRAINT [PK_DeviceMetricsRaw];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260801074617_SeedPermissionsTaxonomy'
)
BEGIN
    ALTER TABLE [DeviceMetricsRaw] ADD CONSTRAINT [PK_DeviceMetricsRaw] PRIMARY KEY NONCLUSTERED ([Id]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260801074617_SeedPermissionsTaxonomy'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Description', N'PermissionKey') AND [object_id] = OBJECT_ID(N'[Permissions]'))
        SET IDENTITY_INSERT [Permissions] ON;
    EXEC(N'INSERT INTO [Permissions] ([Id], [Description], [PermissionKey])
    VALUES (1, N''View user details and lists'', ''Users.View''),
    (2, N''Create new user accounts'', ''Users.Create''),
    (3, N''Update existing user profiles'', ''Users.Update''),
    (4, N''Delete user accounts'', ''Users.Delete''),
    (5, N''Assign or update user roles'', ''Users.ManageRoles''),
    (6, N''View existing roles and mappings'', ''Roles.View''),
    (7, N''Create new security roles'', ''Roles.Create''),
    (8, N''Update existing role names'', ''Roles.Update''),
    (9, N''Delete custom security roles'', ''Roles.Delete''),
    (10, N''Assign permissions to roles'', ''Roles.AssignPermissions''),
    (11, N''View inventory devices and status'', ''Devices.View''),
    (12, N''Add new network devices'', ''Devices.Create''),
    (13, N''Update network device configurations'', ''Devices.Update''),
    (14, N''Remove devices from inventory'', ''Devices.Delete''),
    (15, N''Execute management commands on devices'', ''Devices.Control''),
    (16, N''View raw and processed device telemetry'', ''Telemetry.View''),
    (17, N''Trigger manual SNMP device polling'', ''Telemetry.Poll''),
    (18, N''Export telemetry reports and metric logs'', ''Telemetry.Export'')');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Description', N'PermissionKey') AND [object_id] = OBJECT_ID(N'[Permissions]'))
        SET IDENTITY_INSERT [Permissions] OFF;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260801074617_SeedPermissionsTaxonomy'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260801074617_SeedPermissionsTaxonomy', N'10.0.11');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260806102602_AddReachabilityHistoryTable'
)
BEGIN
    ALTER TABLE [Devices] ADD [FirmwareVersion] nvarchar(100) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260806102602_AddReachabilityHistoryTable'
)
BEGIN
    ALTER TABLE [Devices] ADD [Hostname] nvarchar(255) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260806102602_AddReachabilityHistoryTable'
)
BEGIN
    ALTER TABLE [Devices] ADD [Location] nvarchar(200) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260806102602_AddReachabilityHistoryTable'
)
BEGIN
    ALTER TABLE [Devices] ADD [MacAddress] varchar(17) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260806102602_AddReachabilityHistoryTable'
)
BEGIN
    ALTER TABLE [Devices] ADD [Model] nvarchar(100) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260806102602_AddReachabilityHistoryTable'
)
BEGIN
    ALTER TABLE [Devices] ADD [SerialNumber] nvarchar(100) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260806102602_AddReachabilityHistoryTable'
)
BEGIN
    ALTER TABLE [Devices] ADD [Site] nvarchar(100) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260806102602_AddReachabilityHistoryTable'
)
BEGIN
    ALTER TABLE [Devices] ADD [Vendor] nvarchar(100) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260806102602_AddReachabilityHistoryTable'
)
BEGIN
    CREATE TABLE [DeviceReachabilityHistories] (
        [Id] uniqueidentifier NOT NULL,
        [DeviceId] uniqueidentifier NOT NULL,
        [TenantId] uniqueidentifier NOT NULL,
        [Status] int NOT NULL,
        [MinLatencyMs] float NOT NULL,
        [MaxLatencyMs] float NOT NULL,
        [AvgLatencyMs] float NOT NULL,
        [CurrentLatencyMs] float NOT NULL,
        [PacketsSent] int NOT NULL,
        [PacketsReceived] int NOT NULL,
        [PacketLossPercentage] float NOT NULL,
        [TimestampUtc] datetime2 NOT NULL,
        CONSTRAINT [PK_DeviceReachabilityHistories] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_DeviceReachabilityHistories_Devices_DeviceId] FOREIGN KEY ([DeviceId]) REFERENCES [Devices] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260806102602_AddReachabilityHistoryTable'
)
BEGIN
    CREATE INDEX [IX_Tenants_IsActive] ON [Tenants] ([IsActive]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260806102602_AddReachabilityHistoryTable'
)
BEGIN
    CREATE INDEX [IX_Devices_TenantId_DeviceType] ON [Devices] ([TenantId], [DeviceType]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260806102602_AddReachabilityHistoryTable'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Devices_TenantId_IpAddress] ON [Devices] ([TenantId], [IpAddress]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260806102602_AddReachabilityHistoryTable'
)
BEGIN
    CREATE INDEX [IX_Devices_TenantId_SerialNumber] ON [Devices] ([TenantId], [SerialNumber]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260806102602_AddReachabilityHistoryTable'
)
BEGIN
    CREATE INDEX [IX_DeviceReachabilityHistories_DeviceId_TimestampUtc] ON [DeviceReachabilityHistories] ([DeviceId], [TimestampUtc]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260806102602_AddReachabilityHistoryTable'
)
BEGIN
    CREATE INDEX [IX_DeviceReachabilityHistories_TenantId] ON [DeviceReachabilityHistories] ([TenantId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260806102602_AddReachabilityHistoryTable'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260806102602_AddReachabilityHistoryTable', N'10.0.11');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260807114943_ExpandDeviceMetricRawTable'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260807114943_ExpandDeviceMetricRawTable', N'10.0.11');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260808063106_AddPollProfilesTable'
)
BEGIN
    DROP INDEX [IX_DeviceMetricsRaw_DeviceId] ON [DeviceMetricsRaw];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260808063106_AddPollProfilesTable'
)
BEGIN
    ALTER TABLE [Devices] ADD [PollProfileId] uniqueidentifier NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260808063106_AddPollProfilesTable'
)
BEGIN
    ALTER TABLE [DeviceMetricsRaw] ADD [DiskUtilization] decimal(5,2) NOT NULL DEFAULT 0.0;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260808063106_AddPollProfilesTable'
)
BEGIN
    ALTER TABLE [DeviceMetricsRaw] ADD [FanStatus] int NOT NULL DEFAULT 0;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260808063106_AddPollProfilesTable'
)
BEGIN
    ALTER TABLE [DeviceMetricsRaw] ADD [InterfaceUtilization] decimal(5,2) NOT NULL DEFAULT 0.0;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260808063106_AddPollProfilesTable'
)
BEGIN
    ALTER TABLE [DeviceMetricsRaw] ADD [PowerSupplyStatus] int NOT NULL DEFAULT 0;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260808063106_AddPollProfilesTable'
)
BEGIN
    ALTER TABLE [DeviceMetricsRaw] ADD [Temperature] decimal(5,2) NOT NULL DEFAULT 0.0;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260808063106_AddPollProfilesTable'
)
BEGIN
    ALTER TABLE [DeviceMetricsRaw] ADD [TenantId] uniqueidentifier NOT NULL DEFAULT '00000000-0000-0000-0000-000000000000';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260808063106_AddPollProfilesTable'
)
BEGIN
    CREATE TABLE [PollProfiles] (
        [Id] uniqueidentifier NOT NULL,
        [TenantId] uniqueidentifier NOT NULL,
        [Name] nvarchar(100) NOT NULL,
        [Description] nvarchar(500) NULL,
        [IntervalSeconds] int NOT NULL DEFAULT 60,
        [TimeoutSeconds] int NOT NULL DEFAULT 5,
        [RetryCount] int NOT NULL DEFAULT 3,
        [IsDefault] bit NOT NULL DEFAULT CAST(0 AS bit),
        [IsEnabled] bit NOT NULL DEFAULT CAST(1 AS bit),
        [CreatedAtUtc] datetime2 NOT NULL,
        [CreatedBy] nvarchar(max) NULL,
        [LastModifiedAtUtc] datetime2 NULL,
        [LastModifiedBy] nvarchar(max) NULL,
        CONSTRAINT [PK_PollProfiles] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260808063106_AddPollProfilesTable'
)
BEGIN
    CREATE INDEX [IX_Devices_PollProfileId] ON [Devices] ([PollProfileId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260808063106_AddPollProfilesTable'
)
BEGIN
    CREATE INDEX [IX_DeviceMetricsRaw_TenantId_DeviceId_TimestampUtc] ON [DeviceMetricsRaw] ([TenantId], [DeviceId], [TimestampUtc]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260808063106_AddPollProfilesTable'
)
BEGIN
    CREATE INDEX [IX_PollProfiles_TenantId_IsEnabled] ON [PollProfiles] ([TenantId], [IsEnabled]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260808063106_AddPollProfilesTable'
)
BEGIN
    ALTER TABLE [Devices] ADD CONSTRAINT [FK_Devices_PollProfiles_PollProfileId] FOREIGN KEY ([PollProfileId]) REFERENCES [PollProfiles] ([Id]) ON DELETE SET NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260808063106_AddPollProfilesTable'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260808063106_AddPollProfilesTable', N'10.0.11');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260811070512_AddDiscoveryTables'
)
BEGIN
    CREATE TABLE [DiscoveryJobs] (
        [Id] uniqueidentifier NOT NULL,
        [TenantId] uniqueidentifier NOT NULL,
        [Name] nvarchar(100) NOT NULL,
        [IpRange] nvarchar(100) NOT NULL,
        [SnmpCommunity] nvarchar(100) NULL,
        [SnmpPort] int NOT NULL,
        [Status] int NOT NULL,
        [TotalTargets] int NOT NULL,
        [ProcessedTargets] int NOT NULL,
        [DiscoveredCount] int NOT NULL,
        [StartedAtUtc] datetime2 NULL,
        [CompletedAtUtc] datetime2 NULL,
        [FailureReason] nvarchar(500) NULL,
        [CreatedAtUtc] datetime2 NOT NULL,
        [CreatedBy] nvarchar(max) NULL,
        [LastModifiedAtUtc] datetime2 NULL,
        [LastModifiedBy] nvarchar(max) NULL,
        CONSTRAINT [PK_DiscoveryJobs] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260811070512_AddDiscoveryTables'
)
BEGIN
    CREATE TABLE [DiscoveredDeviceCandidates] (
        [Id] uniqueidentifier NOT NULL,
        [TenantId] uniqueidentifier NOT NULL,
        [DiscoveryJobId] uniqueidentifier NOT NULL,
        [IpAddress] nvarchar(45) NOT NULL,
        [IsIcmpReachable] bit NOT NULL,
        [ResponseTimeMs] float NULL,
        [IsSnmpReachable] bit NOT NULL,
        [SysDescr] nvarchar(500) NULL,
        [SysObjectId] nvarchar(100) NULL,
        [SysName] nvarchar(200) NULL,
        [MacAddress] nvarchar(50) NULL,
        [FingerprintedType] int NOT NULL,
        [IdentifiedVendor] nvarchar(100) NOT NULL,
        [IsDuplicate] bit NOT NULL,
        [ExistingDeviceId] uniqueidentifier NULL,
        [IsImported] bit NOT NULL,
        [ImportedDeviceId] uniqueidentifier NULL,
        CONSTRAINT [PK_DiscoveredDeviceCandidates] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_DiscoveredDeviceCandidates_DiscoveryJobs_DiscoveryJobId] FOREIGN KEY ([DiscoveryJobId]) REFERENCES [DiscoveryJobs] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260811070512_AddDiscoveryTables'
)
BEGIN
    CREATE INDEX [IX_DiscoveredDeviceCandidates_DiscoveryJobId] ON [DiscoveredDeviceCandidates] ([DiscoveryJobId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260811070512_AddDiscoveryTables'
)
BEGIN
    CREATE INDEX [IX_DiscoveredDeviceCandidates_TenantId_IpAddress] ON [DiscoveredDeviceCandidates] ([TenantId], [IpAddress]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260811070512_AddDiscoveryTables'
)
BEGIN
    CREATE INDEX [IX_DiscoveryJobs_TenantId_Status] ON [DiscoveryJobs] ([TenantId], [Status]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260811070512_AddDiscoveryTables'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260811070512_AddDiscoveryTables', N'10.0.11');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260811093018_AddNetworkInterfaceTables'
)
BEGIN
    CREATE TABLE [NetworkInterfaces] (
        [Id] uniqueidentifier NOT NULL,
        [TenantId] uniqueidentifier NOT NULL,
        [DeviceId] uniqueidentifier NOT NULL,
        [IfIndex] int NOT NULL,
        [Name] nvarchar(128) NOT NULL,
        [Description] nvarchar(256) NULL,
        [InterfaceType] nvarchar(64) NULL,
        [MacAddress] nvarchar(64) NULL,
        [SpeedBps] bigint NOT NULL,
        [AdminStatus] int NOT NULL,
        [OperStatus] int NOT NULL,
        [Duplex] int NOT NULL,
        [InOctets] bigint NOT NULL,
        [OutOctets] bigint NOT NULL,
        [InErrors] bigint NOT NULL,
        [OutErrors] bigint NOT NULL,
        [InDiscards] bigint NOT NULL,
        [OutDiscards] bigint NOT NULL,
        [UtilizationPercent] float NOT NULL,
        [LastPolledUtc] datetime2 NULL,
        [CreatedAtUtc] datetime2 NOT NULL,
        [CreatedBy] nvarchar(max) NULL,
        [LastModifiedAtUtc] datetime2 NULL,
        [LastModifiedBy] nvarchar(max) NULL,
        CONSTRAINT [PK_NetworkInterfaces] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_NetworkInterfaces_Devices_DeviceId] FOREIGN KEY ([DeviceId]) REFERENCES [Devices] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260811093018_AddNetworkInterfaceTables'
)
BEGIN
    CREATE TABLE [NetworkInterfaceHistory] (
        [Id] uniqueidentifier NOT NULL,
        [TenantId] uniqueidentifier NOT NULL,
        [DeviceId] uniqueidentifier NOT NULL,
        [NetworkInterfaceId] uniqueidentifier NOT NULL,
        [IfIndex] int NOT NULL,
        [AdminStatus] int NOT NULL,
        [OperStatus] int NOT NULL,
        [SpeedBps] bigint NOT NULL,
        [InOctets] bigint NOT NULL,
        [OutOctets] bigint NOT NULL,
        [InErrors] bigint NOT NULL,
        [OutErrors] bigint NOT NULL,
        [InDiscards] bigint NOT NULL,
        [OutDiscards] bigint NOT NULL,
        [UtilizationPercent] float NOT NULL,
        [TimestampUtc] datetime2 NOT NULL,
        CONSTRAINT [PK_NetworkInterfaceHistory] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_NetworkInterfaceHistory_NetworkInterfaces_NetworkInterfaceId] FOREIGN KEY ([NetworkInterfaceId]) REFERENCES [NetworkInterfaces] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260811093018_AddNetworkInterfaceTables'
)
BEGIN
    CREATE INDEX [IX_NetworkInterfaceHistory_NetworkInterfaceId_TimestampUtc] ON [NetworkInterfaceHistory] ([NetworkInterfaceId], [TimestampUtc]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260811093018_AddNetworkInterfaceTables'
)
BEGIN
    CREATE INDEX [IX_NetworkInterfaceHistory_TenantId_DeviceId_TimestampUtc] ON [NetworkInterfaceHistory] ([TenantId], [DeviceId], [TimestampUtc]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260811093018_AddNetworkInterfaceTables'
)
BEGIN
    CREATE INDEX [IX_NetworkInterfaces_DeviceId] ON [NetworkInterfaces] ([DeviceId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260811093018_AddNetworkInterfaceTables'
)
BEGIN
    CREATE INDEX [IX_NetworkInterfaces_TenantId_DeviceId] ON [NetworkInterfaces] ([TenantId], [DeviceId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260811093018_AddNetworkInterfaceTables'
)
BEGIN
    CREATE UNIQUE INDEX [IX_NetworkInterfaces_TenantId_DeviceId_IfIndex] ON [NetworkInterfaces] ([TenantId], [DeviceId], [IfIndex]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260811093018_AddNetworkInterfaceTables'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260811093018_AddNetworkInterfaceTables', N'10.0.11');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260811113152_AddDeviceHealthHistoryTable'
)
BEGIN
    CREATE TABLE [DeviceHealthHistory] (
        [Id] uniqueidentifier NOT NULL,
        [TenantId] uniqueidentifier NOT NULL,
        [DeviceId] uniqueidentifier NOT NULL,
        [HealthScore] float(5) NOT NULL,
        [Status] int NOT NULL,
        [Reason] nvarchar(1000) NOT NULL,
        [TimestampUtc] datetime2 NOT NULL,
        CONSTRAINT [PK_DeviceHealthHistory] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_DeviceHealthHistory_Devices_DeviceId] FOREIGN KEY ([DeviceId]) REFERENCES [Devices] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260811113152_AddDeviceHealthHistoryTable'
)
BEGIN
    CREATE INDEX [IX_DeviceHealthHistory_DeviceId] ON [DeviceHealthHistory] ([DeviceId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260811113152_AddDeviceHealthHistoryTable'
)
BEGIN
    CREATE INDEX [IX_DeviceHealthHistory_Tenant_Device_Timestamp] ON [DeviceHealthHistory] ([TenantId], [DeviceId], [TimestampUtc]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260811113152_AddDeviceHealthHistoryTable'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260811113152_AddDeviceHealthHistoryTable', N'10.0.11');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260811123940_AddAlertEngineTables'
)
BEGIN
    CREATE TABLE [AlertRules] (
        [Id] uniqueidentifier NOT NULL,
        [TenantId] uniqueidentifier NOT NULL,
        [Name] nvarchar(200) NOT NULL,
        [Description] nvarchar(1000) NOT NULL,
        [MetricType] int NOT NULL,
        [Operator] int NOT NULL,
        [ThresholdValue] decimal(18,4) NOT NULL,
        [Severity] int NOT NULL,
        [IsEnabled] bit NOT NULL,
        [DeviceId] uniqueidentifier NULL,
        [CreatedAtUtc] datetime2 NOT NULL,
        [CreatedBy] nvarchar(max) NULL,
        [LastModifiedAtUtc] datetime2 NULL,
        [LastModifiedBy] nvarchar(max) NULL,
        CONSTRAINT [PK_AlertRules] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260811123940_AddAlertEngineTables'
)
BEGIN
    CREATE TABLE [Alerts] (
        [Id] uniqueidentifier NOT NULL,
        [TenantId] uniqueidentifier NOT NULL,
        [AlertRuleId] uniqueidentifier NOT NULL,
        [DeviceId] uniqueidentifier NOT NULL,
        [MetricType] int NOT NULL,
        [Severity] int NOT NULL,
        [State] int NOT NULL,
        [MetricValue] decimal(18,4) NOT NULL,
        [ThresholdValue] decimal(18,4) NOT NULL,
        [Message] nvarchar(1000) NOT NULL,
        [TriggeredAtUtc] datetime2 NOT NULL,
        [LastOccurredAtUtc] datetime2 NULL,
        [AcknowledgedAtUtc] datetime2 NULL,
        [AcknowledgedBy] nvarchar(200) NULL,
        [SuppressedAtUtc] datetime2 NULL,
        [SuppressedBy] nvarchar(200) NULL,
        [ResolvedAtUtc] datetime2 NULL,
        [CreatedAtUtc] datetime2 NOT NULL,
        [CreatedBy] nvarchar(max) NULL,
        [LastModifiedAtUtc] datetime2 NULL,
        [LastModifiedBy] nvarchar(max) NULL,
        CONSTRAINT [PK_Alerts] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Alerts_AlertRules_AlertRuleId] FOREIGN KEY ([AlertRuleId]) REFERENCES [AlertRules] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Alerts_Devices_DeviceId] FOREIGN KEY ([DeviceId]) REFERENCES [Devices] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260811123940_AddAlertEngineTables'
)
BEGIN
    CREATE TABLE [AlertHistories] (
        [Id] uniqueidentifier NOT NULL,
        [TenantId] uniqueidentifier NOT NULL,
        [AlertId] uniqueidentifier NOT NULL,
        [OldState] int NOT NULL,
        [NewState] int NOT NULL,
        [ChangedBy] nvarchar(200) NOT NULL,
        [Note] nvarchar(1000) NOT NULL,
        [TimestampUtc] datetime2 NOT NULL,
        CONSTRAINT [PK_AlertHistories] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_AlertHistories_Alerts_AlertId] FOREIGN KEY ([AlertId]) REFERENCES [Alerts] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260811123940_AddAlertEngineTables'
)
BEGIN
    CREATE INDEX [IX_AlertHistories_AlertId] ON [AlertHistories] ([AlertId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260811123940_AddAlertEngineTables'
)
BEGIN
    CREATE INDEX [IX_AlertHistories_TenantId_AlertId_TimestampUtc] ON [AlertHistories] ([TenantId], [AlertId], [TimestampUtc]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260811123940_AddAlertEngineTables'
)
BEGIN
    CREATE INDEX [IX_AlertRules_TenantId_DeviceId] ON [AlertRules] ([TenantId], [DeviceId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260811123940_AddAlertEngineTables'
)
BEGIN
    CREATE INDEX [IX_AlertRules_TenantId_IsEnabled] ON [AlertRules] ([TenantId], [IsEnabled]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260811123940_AddAlertEngineTables'
)
BEGIN
    CREATE INDEX [IX_Alerts_AlertRuleId] ON [Alerts] ([AlertRuleId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260811123940_AddAlertEngineTables'
)
BEGIN
    CREATE INDEX [IX_Alerts_DeviceId] ON [Alerts] ([DeviceId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260811123940_AddAlertEngineTables'
)
BEGIN
    CREATE INDEX [IX_Alerts_TenantId_AlertRuleId_DeviceId_State] ON [Alerts] ([TenantId], [AlertRuleId], [DeviceId], [State]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260811123940_AddAlertEngineTables'
)
BEGIN
    CREATE INDEX [IX_Alerts_TenantId_DeviceId_State] ON [Alerts] ([TenantId], [DeviceId], [State]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260811123940_AddAlertEngineTables'
)
BEGIN
    CREATE INDEX [IX_Alerts_TenantId_TriggeredAtUtc] ON [Alerts] ([TenantId], [TriggeredAtUtc]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260811123940_AddAlertEngineTables'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260811123940_AddAlertEngineTables', N'10.0.11');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260812083222_AddNotificationTables'
)
BEGIN
    CREATE TABLE [NotificationLogs] (
        [Id] uniqueidentifier NOT NULL,
        [TenantId] uniqueidentifier NOT NULL,
        [AlertId] uniqueidentifier NULL,
        [Channel] int NOT NULL,
        [RecipientTarget] nvarchar(500) NOT NULL,
        [Subject] nvarchar(500) NOT NULL,
        [Message] nvarchar(max) NOT NULL,
        [Status] int NOT NULL,
        [ErrorMessage] nvarchar(max) NULL,
        [SentAtUtc] datetime2 NOT NULL,
        [CreatedAtUtc] datetime2 NOT NULL,
        [CreatedBy] nvarchar(max) NULL,
        [LastModifiedAtUtc] datetime2 NULL,
        [LastModifiedBy] nvarchar(max) NULL,
        CONSTRAINT [PK_NotificationLogs] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_NotificationLogs_Alerts_AlertId] FOREIGN KEY ([AlertId]) REFERENCES [Alerts] ([Id]) ON DELETE SET NULL
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260812083222_AddNotificationTables'
)
BEGIN
    CREATE TABLE [NotificationTemplates] (
        [Id] uniqueidentifier NOT NULL,
        [TenantId] uniqueidentifier NOT NULL,
        [Name] nvarchar(200) NOT NULL,
        [Channel] int NOT NULL,
        [SubjectTemplate] nvarchar(500) NOT NULL,
        [BodyTemplate] nvarchar(max) NOT NULL,
        [RecipientTarget] nvarchar(500) NOT NULL,
        [IsEnabled] bit NOT NULL,
        [CreatedAtUtc] datetime2 NOT NULL,
        [CreatedBy] nvarchar(max) NULL,
        [LastModifiedAtUtc] datetime2 NULL,
        [LastModifiedBy] nvarchar(max) NULL,
        CONSTRAINT [PK_NotificationTemplates] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260812083222_AddNotificationTables'
)
BEGIN
    CREATE INDEX [IX_NotificationLogs_AlertId] ON [NotificationLogs] ([AlertId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260812083222_AddNotificationTables'
)
BEGIN
    CREATE INDEX [IX_NotificationLogs_TenantId_AlertId_SentAtUtc] ON [NotificationLogs] ([TenantId], [AlertId], [SentAtUtc]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260812083222_AddNotificationTables'
)
BEGIN
    CREATE INDEX [IX_NotificationTemplates_TenantId_Channel_IsEnabled] ON [NotificationTemplates] ([TenantId], [Channel], [IsEnabled]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260812083222_AddNotificationTables'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260812083222_AddNotificationTables', N'10.0.11');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260812100035_AddDeviceEventsTable'
)
BEGIN
    CREATE TABLE [DeviceEvents] (
        [Id] uniqueidentifier NOT NULL,
        [TenantId] uniqueidentifier NOT NULL,
        [DeviceId] uniqueidentifier NULL,
        [Category] int NOT NULL,
        [Severity] int NOT NULL,
        [Source] nvarchar(200) NOT NULL,
        [Message] nvarchar(2000) NOT NULL,
        [CorrelationId] nvarchar(100) NULL,
        [ParentEventId] uniqueidentifier NULL,
        [MetadataJson] nvarchar(max) NULL,
        [CreatedAtUtc] datetime2 NOT NULL,
        [CreatedBy] nvarchar(max) NULL,
        [LastModifiedAtUtc] datetime2 NULL,
        [LastModifiedBy] nvarchar(max) NULL,
        CONSTRAINT [PK_DeviceEvents] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_DeviceEvents_Devices_DeviceId] FOREIGN KEY ([DeviceId]) REFERENCES [Devices] ([Id]) ON DELETE SET NULL
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260812100035_AddDeviceEventsTable'
)
BEGIN
    CREATE INDEX [IX_DeviceEvents_DeviceId] ON [DeviceEvents] ([DeviceId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260812100035_AddDeviceEventsTable'
)
BEGIN
    CREATE INDEX [IX_DeviceEvents_TenantId] ON [DeviceEvents] ([TenantId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260812100035_AddDeviceEventsTable'
)
BEGIN
    CREATE INDEX [IX_DeviceEvents_TenantId_CorrelationId] ON [DeviceEvents] ([TenantId], [CorrelationId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260812100035_AddDeviceEventsTable'
)
BEGIN
    CREATE INDEX [IX_DeviceEvents_TenantId_CreatedAtUtc] ON [DeviceEvents] ([TenantId], [CreatedAtUtc]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260812100035_AddDeviceEventsTable'
)
BEGIN
    CREATE INDEX [IX_DeviceEvents_TenantId_DeviceId] ON [DeviceEvents] ([TenantId], [DeviceId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260812100035_AddDeviceEventsTable'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260812100035_AddDeviceEventsTable', N'10.0.11');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260812121310_AddSyslogTables'
)
BEGIN
    CREATE TABLE [SyslogMessages] (
        [Id] uniqueidentifier NOT NULL,
        [TenantId] uniqueidentifier NOT NULL,
        [DeviceId] uniqueidentifier NULL,
        [Facility] int NOT NULL,
        [Severity] int NOT NULL,
        [FacilityName] nvarchar(50) NOT NULL,
        [SeverityName] nvarchar(50) NOT NULL,
        [TimestampUtc] datetime2 NOT NULL,
        [Hostname] nvarchar(255) NULL,
        [AppTag] nvarchar(100) NULL,
        [ProcessId] nvarchar(50) NULL,
        [MessageId] nvarchar(100) NULL,
        [Message] nvarchar(max) NOT NULL,
        [RawMessage] nvarchar(max) NULL,
        [SourceIpAddress] nvarchar(45) NOT NULL,
        [IsMalformed] bit NOT NULL,
        [CreatedAtUtc] datetime2 NOT NULL,
        [CreatedBy] nvarchar(max) NULL,
        [LastModifiedAtUtc] datetime2 NULL,
        [LastModifiedBy] nvarchar(max) NULL,
        CONSTRAINT [PK_SyslogMessages] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_SyslogMessages_Devices_DeviceId] FOREIGN KEY ([DeviceId]) REFERENCES [Devices] ([Id]) ON DELETE SET NULL
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260812121310_AddSyslogTables'
)
BEGIN
    CREATE INDEX [DeviceId] ON [SyslogMessages] ([TenantId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260812121310_AddSyslogTables'
)
BEGIN
    CREATE INDEX [Facility] ON [SyslogMessages] ([TenantId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260812121310_AddSyslogTables'
)
BEGIN
    CREATE INDEX [IX_SyslogMessages_DeviceId] ON [SyslogMessages] ([DeviceId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260812121310_AddSyslogTables'
)
BEGIN
    CREATE INDEX [IX_SyslogMessages_TenantId] ON [SyslogMessages] ([TenantId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260812121310_AddSyslogTables'
)
BEGIN
    CREATE INDEX [Severity] ON [SyslogMessages] ([TenantId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260812121310_AddSyslogTables'
)
BEGIN
    CREATE INDEX [SourceIpAddress] ON [SyslogMessages] ([TenantId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260812121310_AddSyslogTables'
)
BEGIN
    CREATE INDEX [TimestampUtc] ON [SyslogMessages] ([TenantId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260812121310_AddSyslogTables'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260812121310_AddSyslogTables', N'10.0.11');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260813085848_AddSnmpTrapTables'
)
BEGIN
    CREATE TABLE [SnmpTrapMessages] (
        [Id] uniqueidentifier NOT NULL,
        [TenantId] uniqueidentifier NOT NULL,
        [DeviceId] uniqueidentifier NULL,
        [SnmpVersion] int NOT NULL,
        [Community] nvarchar(100) NULL,
        [EnterpriseOid] nvarchar(255) NOT NULL,
        [GenericTrap] int NULL,
        [SpecificTrap] int NULL,
        [TrapOid] nvarchar(255) NULL,
        [AgentAddress] nvarchar(45) NULL,
        [SourceIpAddress] nvarchar(45) NOT NULL,
        [Severity] int NOT NULL,
        [VarbindsJson] nvarchar(max) NOT NULL,
        [RawPayloadHex] nvarchar(max) NULL,
        [IsMalformed] bit NOT NULL,
        [TimestampUtc] datetime2 NOT NULL,
        [CreatedAtUtc] datetime2 NOT NULL,
        [CreatedBy] nvarchar(max) NULL,
        [LastModifiedAtUtc] datetime2 NULL,
        [LastModifiedBy] nvarchar(max) NULL,
        CONSTRAINT [PK_SnmpTrapMessages] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_SnmpTrapMessages_Devices_DeviceId] FOREIGN KEY ([DeviceId]) REFERENCES [Devices] ([Id]) ON DELETE SET NULL
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260813085848_AddSnmpTrapTables'
)
BEGIN
    CREATE INDEX [IX_SnmpTrapMessages_DeviceId] ON [SnmpTrapMessages] ([DeviceId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260813085848_AddSnmpTrapTables'
)
BEGIN
    CREATE INDEX [IX_SnmpTrapMessages_TenantId] ON [SnmpTrapMessages] ([TenantId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260813085848_AddSnmpTrapTables'
)
BEGIN
    CREATE INDEX [IX_SnmpTrapMessages_TenantId_DeviceId] ON [SnmpTrapMessages] ([TenantId], [DeviceId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260813085848_AddSnmpTrapTables'
)
BEGIN
    CREATE INDEX [IX_SnmpTrapMessages_TenantId_EnterpriseOid] ON [SnmpTrapMessages] ([TenantId], [EnterpriseOid]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260813085848_AddSnmpTrapTables'
)
BEGIN
    CREATE INDEX [IX_SnmpTrapMessages_TenantId_Severity] ON [SnmpTrapMessages] ([TenantId], [Severity]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260813085848_AddSnmpTrapTables'
)
BEGIN
    CREATE INDEX [IX_SnmpTrapMessages_TenantId_SourceIpAddress] ON [SnmpTrapMessages] ([TenantId], [SourceIpAddress]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260813085848_AddSnmpTrapTables'
)
BEGIN
    CREATE INDEX [IX_SnmpTrapMessages_TenantId_TimestampUtc] ON [SnmpTrapMessages] ([TenantId], [TimestampUtc]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260813085848_AddSnmpTrapTables'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260813085848_AddSnmpTrapTables', N'10.0.11');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260813103011_AddConfigurationBackupTables'
)
BEGIN
    CREATE TABLE [BackupSchedules] (
        [Id] uniqueidentifier NOT NULL,
        [TenantId] uniqueidentifier NOT NULL,
        [Name] nvarchar(150) NOT NULL,
        [Description] nvarchar(500) NULL,
        [DeviceId] uniqueidentifier NULL,
        [IntervalMinutes] int NOT NULL,
        [IsEnabled] bit NOT NULL,
        [LastRunUtc] datetime2 NULL,
        [NextRunUtc] datetime2 NOT NULL,
        [CreatedAtUtc] datetime2 NOT NULL,
        [CreatedBy] nvarchar(max) NULL,
        [LastModifiedAtUtc] datetime2 NULL,
        [LastModifiedBy] nvarchar(max) NULL,
        CONSTRAINT [PK_BackupSchedules] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_BackupSchedules_Devices_DeviceId] FOREIGN KEY ([DeviceId]) REFERENCES [Devices] ([Id]) ON DELETE SET NULL
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260813103011_AddConfigurationBackupTables'
)
BEGIN
    CREATE TABLE [ConfigurationBackups] (
        [Id] uniqueidentifier NOT NULL,
        [TenantId] uniqueidentifier NOT NULL,
        [DeviceId] uniqueidentifier NOT NULL,
        [VersionNumber] int NOT NULL,
        [StoragePath] nvarchar(500) NOT NULL,
        [ChecksumSha256] nvarchar(64) NOT NULL,
        [FileSizeBytes] bigint NOT NULL,
        [Status] int NOT NULL,
        [TriggerType] int NOT NULL,
        [FailureReason] nvarchar(1000) NULL,
        [TimestampUtc] datetime2 NOT NULL,
        [IsEligibleForRestore] bit NOT NULL,
        [RestorePreparationNotes] nvarchar(1000) NULL,
        [CreatedAtUtc] datetime2 NOT NULL,
        [CreatedBy] nvarchar(max) NULL,
        [LastModifiedAtUtc] datetime2 NULL,
        [LastModifiedBy] nvarchar(max) NULL,
        CONSTRAINT [PK_ConfigurationBackups] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_ConfigurationBackups_Devices_DeviceId] FOREIGN KEY ([DeviceId]) REFERENCES [Devices] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260813103011_AddConfigurationBackupTables'
)
BEGIN
    CREATE INDEX [IX_BackupSchedules_DeviceId] ON [BackupSchedules] ([DeviceId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260813103011_AddConfigurationBackupTables'
)
BEGIN
    CREATE INDEX [IX_BackupSchedules_IsEnabled] ON [BackupSchedules] ([IsEnabled]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260813103011_AddConfigurationBackupTables'
)
BEGIN
    CREATE INDEX [IX_BackupSchedules_NextRunUtc] ON [BackupSchedules] ([NextRunUtc]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260813103011_AddConfigurationBackupTables'
)
BEGIN
    CREATE INDEX [IX_BackupSchedules_TenantId] ON [BackupSchedules] ([TenantId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260813103011_AddConfigurationBackupTables'
)
BEGIN
    CREATE INDEX [IX_ConfigurationBackups_DeviceId] ON [ConfigurationBackups] ([DeviceId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260813103011_AddConfigurationBackupTables'
)
BEGIN
    CREATE INDEX [IX_ConfigurationBackups_TenantId] ON [ConfigurationBackups] ([TenantId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260813103011_AddConfigurationBackupTables'
)
BEGIN
    CREATE INDEX [IX_ConfigurationBackups_TenantId_DeviceId] ON [ConfigurationBackups] ([TenantId], [DeviceId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260813103011_AddConfigurationBackupTables'
)
BEGIN
    CREATE UNIQUE INDEX [IX_ConfigurationBackups_TenantId_DeviceId_VersionNumber] ON [ConfigurationBackups] ([TenantId], [DeviceId], [VersionNumber]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260813103011_AddConfigurationBackupTables'
)
BEGIN
    CREATE INDEX [IX_ConfigurationBackups_TenantId_TimestampUtc] ON [ConfigurationBackups] ([TenantId], [TimestampUtc]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260813103011_AddConfigurationBackupTables'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260813103011_AddConfigurationBackupTables', N'10.0.11');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260813125621_AddConfigurationRestoreLogTable'
)
BEGIN
    CREATE TABLE [ConfigurationRestoreLogs] (
        [Id] uniqueidentifier NOT NULL,
        [TenantId] uniqueidentifier NOT NULL,
        [DeviceId] uniqueidentifier NOT NULL,
        [TargetBackupId] uniqueidentifier NOT NULL,
        [PreRestoreBackupId] uniqueidentifier NULL,
        [Status] int NOT NULL,
        [InitiatedBy] nvarchar(200) NOT NULL,
        [StartTimeUtc] datetime2 NOT NULL,
        [EndTimeUtc] datetime2 NULL,
        [FailureReason] nvarchar(2000) NULL,
        [RollbackReason] nvarchar(2000) NULL,
        [AuditNotes] nvarchar(1000) NULL,
        [CreatedAtUtc] datetime2 NOT NULL,
        [CreatedBy] nvarchar(max) NULL,
        [LastModifiedAtUtc] datetime2 NULL,
        [LastModifiedBy] nvarchar(max) NULL,
        CONSTRAINT [PK_ConfigurationRestoreLogs] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_ConfigurationRestoreLogs_ConfigurationBackups_PreRestoreBackupId] FOREIGN KEY ([PreRestoreBackupId]) REFERENCES [ConfigurationBackups] ([Id]) ON DELETE SET NULL,
        CONSTRAINT [FK_ConfigurationRestoreLogs_ConfigurationBackups_TargetBackupId] FOREIGN KEY ([TargetBackupId]) REFERENCES [ConfigurationBackups] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_ConfigurationRestoreLogs_Devices_DeviceId] FOREIGN KEY ([DeviceId]) REFERENCES [Devices] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260813125621_AddConfigurationRestoreLogTable'
)
BEGIN
    CREATE INDEX [IX_ConfigurationRestoreLogs_DeviceId] ON [ConfigurationRestoreLogs] ([DeviceId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260813125621_AddConfigurationRestoreLogTable'
)
BEGIN
    CREATE INDEX [IX_ConfigurationRestoreLogs_PreRestoreBackupId] ON [ConfigurationRestoreLogs] ([PreRestoreBackupId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260813125621_AddConfigurationRestoreLogTable'
)
BEGIN
    CREATE INDEX [IX_ConfigurationRestoreLogs_TargetBackupId] ON [ConfigurationRestoreLogs] ([TargetBackupId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260813125621_AddConfigurationRestoreLogTable'
)
BEGIN
    CREATE INDEX [IX_ConfigurationRestoreLogs_TenantId] ON [ConfigurationRestoreLogs] ([TenantId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260813125621_AddConfigurationRestoreLogTable'
)
BEGIN
    CREATE INDEX [IX_ConfigurationRestoreLogs_TenantId_DeviceId] ON [ConfigurationRestoreLogs] ([TenantId], [DeviceId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260813125621_AddConfigurationRestoreLogTable'
)
BEGIN
    CREATE INDEX [IX_ConfigurationRestoreLogs_TenantId_Status] ON [ConfigurationRestoreLogs] ([TenantId], [Status]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260813125621_AddConfigurationRestoreLogTable'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260813125621_AddConfigurationRestoreLogTable', N'10.0.11');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260813155049_AddFirmwareAndOsManagementTables'
)
BEGIN
    CREATE TABLE [FirmwareBaselines] (
        [Id] uniqueidentifier NOT NULL,
        [TenantId] uniqueidentifier NOT NULL,
        [Vendor] nvarchar(100) NOT NULL,
        [Model] nvarchar(100) NOT NULL,
        [TargetVersion] nvarchar(100) NOT NULL,
        [IsActive] bit NOT NULL DEFAULT CAST(1 AS bit),
        [Notes] nvarchar(1000) NULL,
        [CreatedAtUtc] datetime2 NOT NULL,
        [CreatedBy] nvarchar(max) NULL,
        [LastModifiedAtUtc] datetime2 NULL,
        [LastModifiedBy] nvarchar(max) NULL,
        CONSTRAINT [PK_FirmwareBaselines] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260813155049_AddFirmwareAndOsManagementTables'
)
BEGIN
    CREATE TABLE [FirmwareUpgradePlans] (
        [Id] uniqueidentifier NOT NULL,
        [TenantId] uniqueidentifier NOT NULL,
        [DeviceId] uniqueidentifier NOT NULL,
        [TargetVersion] nvarchar(100) NOT NULL,
        [PlannedDateUtc] datetime2 NOT NULL,
        [Status] int NOT NULL,
        [Notes] nvarchar(1000) NULL,
        [CreatedAtUtc] datetime2 NOT NULL,
        [CreatedBy] nvarchar(max) NULL,
        [LastModifiedAtUtc] datetime2 NULL,
        [LastModifiedBy] nvarchar(max) NULL,
        CONSTRAINT [PK_FirmwareUpgradePlans] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_FirmwareUpgradePlans_Devices_DeviceId] FOREIGN KEY ([DeviceId]) REFERENCES [Devices] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260813155049_AddFirmwareAndOsManagementTables'
)
BEGIN
    CREATE UNIQUE INDEX [IX_FirmwareBaselines_TenantId_Vendor_Model] ON [FirmwareBaselines] ([TenantId], [Vendor], [Model]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260813155049_AddFirmwareAndOsManagementTables'
)
BEGIN
    CREATE INDEX [IX_FirmwareUpgradePlans_DeviceId] ON [FirmwareUpgradePlans] ([DeviceId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260813155049_AddFirmwareAndOsManagementTables'
)
BEGIN
    CREATE INDEX [IX_FirmwareUpgradePlans_TenantId_DeviceId] ON [FirmwareUpgradePlans] ([TenantId], [DeviceId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260813155049_AddFirmwareAndOsManagementTables'
)
BEGIN
    CREATE INDEX [IX_FirmwareUpgradePlans_TenantId_Status] ON [FirmwareUpgradePlans] ([TenantId], [Status]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260813155049_AddFirmwareAndOsManagementTables'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260813155049_AddFirmwareAndOsManagementTables', N'10.0.11');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260814074318_AddNetworkTopologyTables'
)
BEGIN
    CREATE TABLE [TopologyLinks] (
        [Id] uniqueidentifier NOT NULL,
        [TenantId] uniqueidentifier NOT NULL,
        [SourceDeviceId] uniqueidentifier NOT NULL,
        [SourceInterfaceId] uniqueidentifier NULL,
        [TargetDeviceId] uniqueidentifier NOT NULL,
        [TargetInterfaceId] uniqueidentifier NULL,
        [LayerType] int NOT NULL,
        [Protocol] int NOT NULL,
        [Status] int NOT NULL,
        [SpeedBps] bigint NOT NULL DEFAULT CAST(0 AS bigint),
        [LastDiscoveredUtc] datetime2 NOT NULL,
        [MetadataJson] nvarchar(4000) NULL,
        [CreatedAtUtc] datetime2 NOT NULL,
        [CreatedBy] nvarchar(max) NULL,
        [LastModifiedAtUtc] datetime2 NULL,
        [LastModifiedBy] nvarchar(max) NULL,
        CONSTRAINT [PK_TopologyLinks] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_TopologyLinks_Devices_SourceDeviceId] FOREIGN KEY ([SourceDeviceId]) REFERENCES [Devices] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_TopologyLinks_Devices_TargetDeviceId] FOREIGN KEY ([TargetDeviceId]) REFERENCES [Devices] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_TopologyLinks_NetworkInterfaces_SourceInterfaceId] FOREIGN KEY ([SourceInterfaceId]) REFERENCES [NetworkInterfaces] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_TopologyLinks_NetworkInterfaces_TargetInterfaceId] FOREIGN KEY ([TargetInterfaceId]) REFERENCES [NetworkInterfaces] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260814074318_AddNetworkTopologyTables'
)
BEGIN
    CREATE INDEX [IX_TopologyLinks_SourceDeviceId] ON [TopologyLinks] ([SourceDeviceId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260814074318_AddNetworkTopologyTables'
)
BEGIN
    CREATE INDEX [IX_TopologyLinks_SourceInterfaceId] ON [TopologyLinks] ([SourceInterfaceId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260814074318_AddNetworkTopologyTables'
)
BEGIN
    CREATE INDEX [IX_TopologyLinks_TargetDeviceId] ON [TopologyLinks] ([TargetDeviceId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260814074318_AddNetworkTopologyTables'
)
BEGIN
    CREATE INDEX [IX_TopologyLinks_TargetInterfaceId] ON [TopologyLinks] ([TargetInterfaceId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260814074318_AddNetworkTopologyTables'
)
BEGIN
    CREATE INDEX [IX_TopologyLinks_TenantId] ON [TopologyLinks] ([TenantId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260814074318_AddNetworkTopologyTables'
)
BEGIN
    CREATE INDEX [IX_TopologyLinks_TenantId_LayerType_Status] ON [TopologyLinks] ([TenantId], [LayerType], [Status]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260814074318_AddNetworkTopologyTables'
)
BEGIN
    CREATE INDEX [IX_TopologyLinks_TenantId_SourceDeviceId] ON [TopologyLinks] ([TenantId], [SourceDeviceId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260814074318_AddNetworkTopologyTables'
)
BEGIN
    CREATE INDEX [IX_TopologyLinks_TenantId_Status] ON [TopologyLinks] ([TenantId], [Status]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260814074318_AddNetworkTopologyTables'
)
BEGIN
    CREATE INDEX [IX_TopologyLinks_TenantId_TargetDeviceId] ON [TopologyLinks] ([TenantId], [TargetDeviceId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260814074318_AddNetworkTopologyTables'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260814074318_AddNetworkTopologyTables', N'10.0.11');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260814160916_AddReportingEngineTables'
)
BEGIN
    CREATE TABLE [ScheduledReports] (
        [Id] uniqueidentifier NOT NULL,
        [TenantId] uniqueidentifier NOT NULL,
        [Name] nvarchar(150) NOT NULL,
        [ReportType] nvarchar(50) NOT NULL,
        [ScheduleFrequency] nvarchar(50) NOT NULL,
        [OutputFormat] nvarchar(20) NOT NULL,
        [FilterJson] nvarchar(4000) NULL,
        [RecipientEmail] nvarchar(256) NOT NULL,
        [IsActive] bit NOT NULL,
        [LastRunUtc] datetime2 NULL,
        [NextRunUtc] datetime2 NULL,
        [CreatedAtUtc] datetime2 NOT NULL,
        [CreatedBy] nvarchar(max) NULL,
        [LastModifiedAtUtc] datetime2 NULL,
        [LastModifiedBy] nvarchar(max) NULL,
        CONSTRAINT [PK_ScheduledReports] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260814160916_AddReportingEngineTables'
)
BEGIN
    CREATE TABLE [ScheduledReportExecutionLogs] (
        [Id] uniqueidentifier NOT NULL,
        [TenantId] uniqueidentifier NOT NULL,
        [ScheduledReportId] uniqueidentifier NOT NULL,
        [ExecutedAtUtc] datetime2 NOT NULL,
        [Status] nvarchar(30) NOT NULL,
        [RecordCount] int NOT NULL,
        [ErrorMessage] nvarchar(2000) NULL,
        [CreatedAtUtc] datetime2 NOT NULL,
        [CreatedBy] nvarchar(max) NULL,
        [LastModifiedAtUtc] datetime2 NULL,
        [LastModifiedBy] nvarchar(max) NULL,
        CONSTRAINT [PK_ScheduledReportExecutionLogs] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_ScheduledReportExecutionLogs_ScheduledReports_ScheduledReportId] FOREIGN KEY ([ScheduledReportId]) REFERENCES [ScheduledReports] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260814160916_AddReportingEngineTables'
)
BEGIN
    CREATE INDEX [IX_ScheduledReportExecutionLogs_ExecutedAtUtc] ON [ScheduledReportExecutionLogs] ([ExecutedAtUtc]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260814160916_AddReportingEngineTables'
)
BEGIN
    CREATE INDEX [IX_ScheduledReportExecutionLogs_ScheduledReportId] ON [ScheduledReportExecutionLogs] ([ScheduledReportId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260814160916_AddReportingEngineTables'
)
BEGIN
    CREATE INDEX [IX_ScheduledReportExecutionLogs_TenantId] ON [ScheduledReportExecutionLogs] ([TenantId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260814160916_AddReportingEngineTables'
)
BEGIN
    CREATE INDEX [IX_ScheduledReports_IsActive_NextRunUtc] ON [ScheduledReports] ([IsActive], [NextRunUtc]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260814160916_AddReportingEngineTables'
)
BEGIN
    CREATE INDEX [IX_ScheduledReports_TenantId] ON [ScheduledReports] ([TenantId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260814160916_AddReportingEngineTables'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260814160916_AddReportingEngineTables', N'10.0.11');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260815090416_AddAuditAndComplianceEnhancements'
)
BEGIN
    ALTER TABLE [AuditLogs] ADD [Category] varchar(50) NOT NULL DEFAULT '';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260815090416_AddAuditAndComplianceEnhancements'
)
BEGIN
    ALTER TABLE [AuditLogs] ADD [Details] nvarchar(500) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260815090416_AddAuditAndComplianceEnhancements'
)
BEGIN
    ALTER TABLE [AuditLogs] ADD [EntityId] nvarchar(100) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260815090416_AddAuditAndComplianceEnhancements'
)
BEGIN
    ALTER TABLE [AuditLogs] ADD [EntityName] nvarchar(100) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260815090416_AddAuditAndComplianceEnhancements'
)
BEGIN
    ALTER TABLE [AuditLogs] ADD [IpAddress] varchar(45) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260815090416_AddAuditAndComplianceEnhancements'
)
BEGIN
    ALTER TABLE [AuditLogs] ADD [Status] varchar(20) NOT NULL DEFAULT '';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260815090416_AddAuditAndComplianceEnhancements'
)
BEGIN
    ALTER TABLE [AuditLogs] ADD [Username] nvarchar(100) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260815090416_AddAuditAndComplianceEnhancements'
)
BEGIN
    CREATE INDEX [IX_AuditLogs_TenantId_Category_TimestampUtc] ON [AuditLogs] ([TenantId], [Category], [TimestampUtc]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260815090416_AddAuditAndComplianceEnhancements'
)
BEGIN
    CREATE INDEX [IX_AuditLogs_TenantId_TimestampUtc] ON [AuditLogs] ([TenantId], [TimestampUtc]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260815090416_AddAuditAndComplianceEnhancements'
)
BEGIN
    CREATE INDEX [IX_AuditLogs_TenantId_UserId] ON [AuditLogs] ([TenantId], [UserId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260815090416_AddAuditAndComplianceEnhancements'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260815090416_AddAuditAndComplianceEnhancements', N'10.0.11');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260817045825_AddAssetManagementTables'
)
BEGIN
    CREATE TABLE [Assets] (
        [Id] uniqueidentifier NOT NULL,
        [TenantId] uniqueidentifier NOT NULL,
        [AssetTag] nvarchar(100) NOT NULL,
        [Name] nvarchar(200) NOT NULL,
        [SerialNumber] nvarchar(100) NULL,
        [Vendor] nvarchar(100) NULL,
        [Model] nvarchar(100) NULL,
        [Category] nvarchar(100) NULL,
        [LifecycleState] nvarchar(50) NOT NULL,
        [LifecycleNotes] nvarchar(1000) NULL,
        [LifecycleChangedAtUtc] datetime2 NULL,
        [WarrantyProvider] nvarchar(200) NULL,
        [WarrantyStartDateUtc] datetime2 NULL,
        [WarrantyEndDateUtc] datetime2 NULL,
        [WarrantyStatus] nvarchar(50) NOT NULL,
        [WarrantyContractNumber] nvarchar(100) NULL,
        [PurchaseDateUtc] datetime2 NULL,
        [PurchaseOrderNumber] nvarchar(100) NULL,
        [PurchasePrice] decimal(18,2) NULL,
        [Currency] nvarchar(10) NULL,
        [SiteOrLocation] nvarchar(200) NULL,
        [RackIdentifier] nvarchar(100) NULL,
        [RackUnitPosition] nvarchar(50) NULL,
        [Department] nvarchar(100) NULL,
        [AssignedToUserId] uniqueidentifier NULL,
        [DeviceId] uniqueidentifier NULL,
        [CreatedAtUtc] datetime2 NOT NULL,
        [CreatedBy] nvarchar(200) NULL,
        [LastModifiedAtUtc] datetime2 NULL,
        [LastModifiedBy] nvarchar(200) NULL,
        CONSTRAINT [PK_Assets] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Assets_Devices_DeviceId] FOREIGN KEY ([DeviceId]) REFERENCES [Devices] ([Id]) ON DELETE SET NULL,
        CONSTRAINT [FK_Assets_Users_AssignedToUserId] FOREIGN KEY ([AssignedToUserId]) REFERENCES [Users] ([Id]) ON DELETE SET NULL
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260817045825_AddAssetManagementTables'
)
BEGIN
    CREATE INDEX [IX_Assets_AssignedToUserId] ON [Assets] ([AssignedToUserId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260817045825_AddAssetManagementTables'
)
BEGIN
    CREATE INDEX [IX_Assets_DeviceId] ON [Assets] ([DeviceId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260817045825_AddAssetManagementTables'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Assets_TenantId_AssetTag] ON [Assets] ([TenantId], [AssetTag]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260817045825_AddAssetManagementTables'
)
BEGIN
    CREATE INDEX [IX_Assets_TenantId_DeviceId] ON [Assets] ([TenantId], [DeviceId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260817045825_AddAssetManagementTables'
)
BEGIN
    CREATE INDEX [IX_Assets_TenantId_LifecycleState] ON [Assets] ([TenantId], [LifecycleState]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260817045825_AddAssetManagementTables'
)
BEGIN
    CREATE INDEX [IX_Assets_TenantId_SerialNumber] ON [Assets] ([TenantId], [SerialNumber]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260817045825_AddAssetManagementTables'
)
BEGIN
    CREATE INDEX [IX_Assets_TenantId_WarrantyStatus] ON [Assets] ([TenantId], [WarrantyStatus]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260817045825_AddAssetManagementTables'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260817045825_AddAssetManagementTables', N'10.0.11');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260817095217_AddSiteAndLocationManagementTables'
)
BEGIN
    CREATE TABLE [Sites] (
        [Id] uniqueidentifier NOT NULL,
        [TenantId] uniqueidentifier NOT NULL,
        [Name] nvarchar(200) NOT NULL,
        [Code] nvarchar(50) NOT NULL,
        [Description] nvarchar(500) NULL,
        [Address] nvarchar(500) NULL,
        [City] nvarchar(100) NULL,
        [StateOrProvince] nvarchar(100) NULL,
        [PostalCode] nvarchar(30) NULL,
        [Country] nvarchar(100) NULL,
        [Latitude] float NULL,
        [Longitude] float NULL,
        [TimeZone] nvarchar(100) NULL,
        [IsActive] bit NOT NULL,
        [CreatedAtUtc] datetime2 NOT NULL,
        [CreatedBy] nvarchar(200) NULL,
        [LastModifiedAtUtc] datetime2 NULL,
        [LastModifiedBy] nvarchar(200) NULL,
        CONSTRAINT [PK_Sites] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260817095217_AddSiteAndLocationManagementTables'
)
BEGIN
    CREATE TABLE [Buildings] (
        [Id] uniqueidentifier NOT NULL,
        [TenantId] uniqueidentifier NOT NULL,
        [SiteId] uniqueidentifier NOT NULL,
        [Name] nvarchar(200) NOT NULL,
        [Code] nvarchar(50) NOT NULL,
        [Description] nvarchar(500) NULL,
        [Address] nvarchar(500) NULL,
        [IsActive] bit NOT NULL,
        [CreatedAtUtc] datetime2 NOT NULL,
        [CreatedBy] nvarchar(200) NULL,
        [LastModifiedAtUtc] datetime2 NULL,
        [LastModifiedBy] nvarchar(200) NULL,
        CONSTRAINT [PK_Buildings] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Buildings_Sites_SiteId] FOREIGN KEY ([SiteId]) REFERENCES [Sites] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260817095217_AddSiteAndLocationManagementTables'
)
BEGIN
    CREATE TABLE [Floors] (
        [Id] uniqueidentifier NOT NULL,
        [TenantId] uniqueidentifier NOT NULL,
        [BuildingId] uniqueidentifier NOT NULL,
        [Name] nvarchar(100) NOT NULL,
        [FloorNumber] int NOT NULL,
        [Description] nvarchar(500) NULL,
        [IsActive] bit NOT NULL,
        [CreatedAtUtc] datetime2 NOT NULL,
        [CreatedBy] nvarchar(200) NULL,
        [LastModifiedAtUtc] datetime2 NULL,
        [LastModifiedBy] nvarchar(200) NULL,
        CONSTRAINT [PK_Floors] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Floors_Buildings_BuildingId] FOREIGN KEY ([BuildingId]) REFERENCES [Buildings] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260817095217_AddSiteAndLocationManagementTables'
)
BEGIN
    CREATE TABLE [Rooms] (
        [Id] uniqueidentifier NOT NULL,
        [TenantId] uniqueidentifier NOT NULL,
        [FloorId] uniqueidentifier NOT NULL,
        [Name] nvarchar(150) NOT NULL,
        [Code] nvarchar(50) NOT NULL,
        [RoomType] nvarchar(100) NULL,
        [Description] nvarchar(500) NULL,
        [IsActive] bit NOT NULL,
        [CreatedAtUtc] datetime2 NOT NULL,
        [CreatedBy] nvarchar(200) NULL,
        [LastModifiedAtUtc] datetime2 NULL,
        [LastModifiedBy] nvarchar(200) NULL,
        CONSTRAINT [PK_Rooms] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Rooms_Floors_FloorId] FOREIGN KEY ([FloorId]) REFERENCES [Floors] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260817095217_AddSiteAndLocationManagementTables'
)
BEGIN
    CREATE TABLE [Racks] (
        [Id] uniqueidentifier NOT NULL,
        [TenantId] uniqueidentifier NOT NULL,
        [RoomId] uniqueidentifier NOT NULL,
        [Name] nvarchar(150) NOT NULL,
        [Identifier] nvarchar(50) NOT NULL,
        [HeightInUnits] int NOT NULL,
        [WidthInInches] int NULL,
        [DepthInMm] int NULL,
        [MaxPowerWatts] decimal(18,2) NULL,
        [MaxWeightKg] decimal(18,2) NULL,
        [Notes] nvarchar(1000) NULL,
        [IsActive] bit NOT NULL,
        [CreatedAtUtc] datetime2 NOT NULL,
        [CreatedBy] nvarchar(200) NULL,
        [LastModifiedAtUtc] datetime2 NULL,
        [LastModifiedBy] nvarchar(200) NULL,
        CONSTRAINT [PK_Racks] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Racks_Rooms_RoomId] FOREIGN KEY ([RoomId]) REFERENCES [Rooms] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260817095217_AddSiteAndLocationManagementTables'
)
BEGIN
    CREATE INDEX [IX_Buildings_SiteId] ON [Buildings] ([SiteId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260817095217_AddSiteAndLocationManagementTables'
)
BEGIN
    CREATE INDEX [IX_Buildings_TenantId_IsActive] ON [Buildings] ([TenantId], [IsActive]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260817095217_AddSiteAndLocationManagementTables'
)
BEGIN
    CREATE INDEX [IX_Buildings_TenantId_SiteId] ON [Buildings] ([TenantId], [SiteId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260817095217_AddSiteAndLocationManagementTables'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Buildings_TenantId_SiteId_Code] ON [Buildings] ([TenantId], [SiteId], [Code]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260817095217_AddSiteAndLocationManagementTables'
)
BEGIN
    CREATE INDEX [IX_Floors_BuildingId] ON [Floors] ([BuildingId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260817095217_AddSiteAndLocationManagementTables'
)
BEGIN
    CREATE INDEX [IX_Floors_TenantId_BuildingId] ON [Floors] ([TenantId], [BuildingId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260817095217_AddSiteAndLocationManagementTables'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Floors_TenantId_BuildingId_FloorNumber] ON [Floors] ([TenantId], [BuildingId], [FloorNumber]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260817095217_AddSiteAndLocationManagementTables'
)
BEGIN
    CREATE INDEX [IX_Floors_TenantId_IsActive] ON [Floors] ([TenantId], [IsActive]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260817095217_AddSiteAndLocationManagementTables'
)
BEGIN
    CREATE INDEX [IX_Racks_RoomId] ON [Racks] ([RoomId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260817095217_AddSiteAndLocationManagementTables'
)
BEGIN
    CREATE INDEX [IX_Racks_TenantId_IsActive] ON [Racks] ([TenantId], [IsActive]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260817095217_AddSiteAndLocationManagementTables'
)
BEGIN
    CREATE INDEX [IX_Racks_TenantId_RoomId] ON [Racks] ([TenantId], [RoomId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260817095217_AddSiteAndLocationManagementTables'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Racks_TenantId_RoomId_Identifier] ON [Racks] ([TenantId], [RoomId], [Identifier]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260817095217_AddSiteAndLocationManagementTables'
)
BEGIN
    CREATE INDEX [IX_Rooms_FloorId] ON [Rooms] ([FloorId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260817095217_AddSiteAndLocationManagementTables'
)
BEGIN
    CREATE INDEX [IX_Rooms_TenantId_FloorId] ON [Rooms] ([TenantId], [FloorId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260817095217_AddSiteAndLocationManagementTables'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Rooms_TenantId_FloorId_Code] ON [Rooms] ([TenantId], [FloorId], [Code]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260817095217_AddSiteAndLocationManagementTables'
)
BEGIN
    CREATE INDEX [IX_Rooms_TenantId_IsActive] ON [Rooms] ([TenantId], [IsActive]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260817095217_AddSiteAndLocationManagementTables'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Sites_TenantId_Code] ON [Sites] ([TenantId], [Code]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260817095217_AddSiteAndLocationManagementTables'
)
BEGIN
    CREATE INDEX [IX_Sites_TenantId_IsActive] ON [Sites] ([TenantId], [IsActive]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260817095217_AddSiteAndLocationManagementTables'
)
BEGIN
    CREATE INDEX [IX_Sites_TenantId_Name] ON [Sites] ([TenantId], [Name]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260817095217_AddSiteAndLocationManagementTables'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260817095217_AddSiteAndLocationManagementTables', N'10.0.11');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260817112720_AddCustomerManagementTables'
)
BEGIN
    CREATE TABLE [Customers] (
        [Id] uniqueidentifier NOT NULL,
        [TenantId] uniqueidentifier NOT NULL,
        [Name] nvarchar(200) NOT NULL,
        [Code] nvarchar(50) NOT NULL,
        [AccountNumber] nvarchar(100) NULL,
        [Description] nvarchar(500) NULL,
        [Status] int NOT NULL,
        [Tier] int NOT NULL,
        [MetadataJson] nvarchar(max) NULL,
        [ParentCustomerId] uniqueidentifier NULL,
        [CreatedAtUtc] datetime2 NOT NULL,
        [CreatedBy] nvarchar(200) NULL,
        [LastModifiedAtUtc] datetime2 NULL,
        [LastModifiedBy] nvarchar(200) NULL,
        CONSTRAINT [PK_Customers] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Customers_Customers_ParentCustomerId] FOREIGN KEY ([ParentCustomerId]) REFERENCES [Customers] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260817112720_AddCustomerManagementTables'
)
BEGIN
    CREATE TABLE [CustomerContacts] (
        [Id] uniqueidentifier NOT NULL,
        [TenantId] uniqueidentifier NOT NULL,
        [CustomerId] uniqueidentifier NOT NULL,
        [FirstName] nvarchar(100) NOT NULL,
        [LastName] nvarchar(100) NOT NULL,
        [Email] nvarchar(200) NOT NULL,
        [PhoneNumber] nvarchar(50) NULL,
        [JobTitle] nvarchar(100) NULL,
        [ContactType] int NOT NULL,
        [IsPrimary] bit NOT NULL,
        [CreatedAtUtc] datetime2 NOT NULL,
        [CreatedBy] nvarchar(200) NULL,
        [LastModifiedAtUtc] datetime2 NULL,
        [LastModifiedBy] nvarchar(200) NULL,
        CONSTRAINT [PK_CustomerContacts] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_CustomerContacts_Customers_CustomerId] FOREIGN KEY ([CustomerId]) REFERENCES [Customers] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260817112720_AddCustomerManagementTables'
)
BEGIN
    CREATE INDEX [IX_CustomerContacts_CustomerId] ON [CustomerContacts] ([CustomerId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260817112720_AddCustomerManagementTables'
)
BEGIN
    CREATE INDEX [IX_CustomerContacts_TenantId_CustomerId] ON [CustomerContacts] ([TenantId], [CustomerId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260817112720_AddCustomerManagementTables'
)
BEGIN
    CREATE UNIQUE INDEX [IX_CustomerContacts_TenantId_CustomerId_Email] ON [CustomerContacts] ([TenantId], [CustomerId], [Email]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260817112720_AddCustomerManagementTables'
)
BEGIN
    CREATE INDEX [IX_CustomerContacts_TenantId_Email] ON [CustomerContacts] ([TenantId], [Email]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260817112720_AddCustomerManagementTables'
)
BEGIN
    CREATE INDEX [IX_CustomerContacts_TenantId_IsPrimary] ON [CustomerContacts] ([TenantId], [IsPrimary]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260817112720_AddCustomerManagementTables'
)
BEGIN
    CREATE INDEX [IX_Customers_ParentCustomerId] ON [Customers] ([ParentCustomerId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260817112720_AddCustomerManagementTables'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Customers_TenantId_Code] ON [Customers] ([TenantId], [Code]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260817112720_AddCustomerManagementTables'
)
BEGIN
    CREATE INDEX [IX_Customers_TenantId_Name] ON [Customers] ([TenantId], [Name]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260817112720_AddCustomerManagementTables'
)
BEGIN
    CREATE INDEX [IX_Customers_TenantId_ParentCustomerId] ON [Customers] ([TenantId], [ParentCustomerId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260817112720_AddCustomerManagementTables'
)
BEGIN
    CREATE INDEX [IX_Customers_TenantId_Status] ON [Customers] ([TenantId], [Status]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260817112720_AddCustomerManagementTables'
)
BEGIN
    CREATE INDEX [IX_Customers_TenantId_Tier] ON [Customers] ([TenantId], [Tier]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260817112720_AddCustomerManagementTables'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260817112720_AddCustomerManagementTables', N'10.0.11');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260817155337_AddTicketingIntegrationTables'
)
BEGIN
    CREATE TABLE [Tickets] (
        [Id] uniqueidentifier NOT NULL,
        [TenantId] uniqueidentifier NOT NULL,
        [Title] nvarchar(200) NOT NULL,
        [Description] nvarchar(4000) NOT NULL,
        [Priority] int NOT NULL,
        [Status] int NOT NULL,
        [ProviderType] int NOT NULL,
        [ExternalTicketId] nvarchar(100) NULL,
        [ExternalTicketKey] nvarchar(100) NULL,
        [ExternalStatus] nvarchar(100) NULL,
        [ExternalUrl] nvarchar(1000) NULL,
        [LastSyncedAtUtc] datetime2 NULL,
        [DeviceId] uniqueidentifier NULL,
        [AlertId] uniqueidentifier NULL,
        [CustomerId] uniqueidentifier NULL,
        [MetadataJson] nvarchar(4000) NULL,
        [CreatedAtUtc] datetime2 NOT NULL,
        [CreatedBy] nvarchar(max) NULL,
        [LastModifiedAtUtc] datetime2 NULL,
        [LastModifiedBy] nvarchar(max) NULL,
        CONSTRAINT [PK_Tickets] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Tickets_Alerts_AlertId] FOREIGN KEY ([AlertId]) REFERENCES [Alerts] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Tickets_Customers_CustomerId] FOREIGN KEY ([CustomerId]) REFERENCES [Customers] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Tickets_Devices_DeviceId] FOREIGN KEY ([DeviceId]) REFERENCES [Devices] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260817155337_AddTicketingIntegrationTables'
)
BEGIN
    CREATE TABLE [TicketSyncLogs] (
        [Id] uniqueidentifier NOT NULL,
        [TenantId] uniqueidentifier NOT NULL,
        [TicketId] uniqueidentifier NOT NULL,
        [ProviderType] int NOT NULL,
        [Direction] int NOT NULL,
        [Status] int NOT NULL,
        [RequestPayload] nvarchar(4000) NULL,
        [ResponsePayload] nvarchar(4000) NULL,
        [ErrorMessage] nvarchar(4000) NULL,
        [TimestampUtc] datetime2 NOT NULL,
        CONSTRAINT [PK_TicketSyncLogs] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_TicketSyncLogs_Tickets_TicketId] FOREIGN KEY ([TicketId]) REFERENCES [Tickets] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260817155337_AddTicketingIntegrationTables'
)
BEGIN
    CREATE INDEX [IX_Tickets_AlertId] ON [Tickets] ([AlertId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260817155337_AddTicketingIntegrationTables'
)
BEGIN
    CREATE INDEX [IX_Tickets_CustomerId] ON [Tickets] ([CustomerId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260817155337_AddTicketingIntegrationTables'
)
BEGIN
    CREATE INDEX [IX_Tickets_DeviceId] ON [Tickets] ([DeviceId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260817155337_AddTicketingIntegrationTables'
)
BEGIN
    CREATE INDEX [IX_Tickets_TenantId_CreatedAtUtc] ON [Tickets] ([TenantId], [CreatedAtUtc]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260817155337_AddTicketingIntegrationTables'
)
BEGIN
    CREATE INDEX [IX_Tickets_TenantId_ExternalTicketId] ON [Tickets] ([TenantId], [ExternalTicketId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260817155337_AddTicketingIntegrationTables'
)
BEGIN
    CREATE INDEX [IX_Tickets_TenantId_Priority] ON [Tickets] ([TenantId], [Priority]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260817155337_AddTicketingIntegrationTables'
)
BEGIN
    CREATE INDEX [IX_Tickets_TenantId_ProviderType] ON [Tickets] ([TenantId], [ProviderType]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260817155337_AddTicketingIntegrationTables'
)
BEGIN
    CREATE INDEX [IX_Tickets_TenantId_Status] ON [Tickets] ([TenantId], [Status]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260817155337_AddTicketingIntegrationTables'
)
BEGIN
    CREATE INDEX [IX_TicketSyncLogs_TenantId_TicketId] ON [TicketSyncLogs] ([TenantId], [TicketId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260817155337_AddTicketingIntegrationTables'
)
BEGIN
    CREATE INDEX [IX_TicketSyncLogs_TenantId_TimestampUtc] ON [TicketSyncLogs] ([TenantId], [TimestampUtc]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260817155337_AddTicketingIntegrationTables'
)
BEGIN
    CREATE INDEX [IX_TicketSyncLogs_TicketId] ON [TicketSyncLogs] ([TicketId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260817155337_AddTicketingIntegrationTables'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260817155337_AddTicketingIntegrationTables', N'10.0.11');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260818071205_20260818123812_AddCybersecurityComplianceTables'
)
BEGIN
    CREATE TABLE [CompliancePolicies] (
        [Id] uniqueidentifier NOT NULL,
        [TenantId] uniqueidentifier NOT NULL,
        [Name] nvarchar(200) NOT NULL,
        [Description] nvarchar(1000) NOT NULL,
        [Category] int NOT NULL,
        [CheckType] int NOT NULL,
        [Severity] int NOT NULL,
        [IsActive] bit NOT NULL,
        [TargetVendor] nvarchar(100) NULL,
        [TargetDeviceType] int NULL,
        [RuleConfigurationJson] nvarchar(4000) NULL,
        [CreatedAtUtc] datetime2 NOT NULL,
        [CreatedBy] nvarchar(100) NULL,
        [LastModifiedAtUtc] datetime2 NULL,
        [LastModifiedBy] nvarchar(100) NULL,
        CONSTRAINT [PK_CompliancePolicies] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260818071205_20260818123812_AddCybersecurityComplianceTables'
)
BEGIN
    CREATE TABLE [DeviceComplianceScans] (
        [Id] uniqueidentifier NOT NULL,
        [TenantId] uniqueidentifier NOT NULL,
        [DeviceId] uniqueidentifier NOT NULL,
        [ScannedAtUtc] datetime2 NOT NULL,
        [OverallStatus] int NOT NULL,
        [PassedChecks] int NOT NULL,
        [FailedChecks] int NOT NULL,
        [WarningChecks] int NOT NULL,
        [NotApplicableChecks] int NOT NULL,
        [TotalChecks] int NOT NULL,
        [EvaluationNotes] nvarchar(1000) NULL,
        [CreatedAtUtc] datetime2 NOT NULL,
        [CreatedBy] nvarchar(100) NULL,
        [LastModifiedAtUtc] datetime2 NULL,
        [LastModifiedBy] nvarchar(100) NULL,
        CONSTRAINT [PK_DeviceComplianceScans] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_DeviceComplianceScans_Devices_DeviceId] FOREIGN KEY ([DeviceId]) REFERENCES [Devices] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260818071205_20260818123812_AddCybersecurityComplianceTables'
)
BEGIN
    CREATE TABLE [DeviceComplianceResults] (
        [Id] uniqueidentifier NOT NULL,
        [TenantId] uniqueidentifier NOT NULL,
        [ScanId] uniqueidentifier NOT NULL,
        [PolicyId] uniqueidentifier NOT NULL,
        [CheckType] int NOT NULL,
        [Category] int NOT NULL,
        [Severity] int NOT NULL,
        [Status] int NOT NULL,
        [Summary] nvarchar(500) NOT NULL,
        [Details] nvarchar(2000) NULL,
        [RemediationGuidance] nvarchar(2000) NULL,
        [EvaluatedAtUtc] datetime2 NOT NULL,
        CONSTRAINT [PK_DeviceComplianceResults] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_DeviceComplianceResults_CompliancePolicies_PolicyId] FOREIGN KEY ([PolicyId]) REFERENCES [CompliancePolicies] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_DeviceComplianceResults_DeviceComplianceScans_ScanId] FOREIGN KEY ([ScanId]) REFERENCES [DeviceComplianceScans] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260818071205_20260818123812_AddCybersecurityComplianceTables'
)
BEGIN
    CREATE INDEX [IX_CompliancePolicies_TenantId_Category] ON [CompliancePolicies] ([TenantId], [Category]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260818071205_20260818123812_AddCybersecurityComplianceTables'
)
BEGIN
    CREATE INDEX [IX_CompliancePolicies_TenantId_CheckType] ON [CompliancePolicies] ([TenantId], [CheckType]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260818071205_20260818123812_AddCybersecurityComplianceTables'
)
BEGIN
    CREATE INDEX [IX_CompliancePolicies_TenantId_IsActive] ON [CompliancePolicies] ([TenantId], [IsActive]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260818071205_20260818123812_AddCybersecurityComplianceTables'
)
BEGIN
    CREATE INDEX [IX_DeviceComplianceResults_PolicyId] ON [DeviceComplianceResults] ([PolicyId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260818071205_20260818123812_AddCybersecurityComplianceTables'
)
BEGIN
    CREATE INDEX [IX_DeviceComplianceResults_ScanId] ON [DeviceComplianceResults] ([ScanId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260818071205_20260818123812_AddCybersecurityComplianceTables'
)
BEGIN
    CREATE INDEX [IX_DeviceComplianceResults_TenantId_PolicyId] ON [DeviceComplianceResults] ([TenantId], [PolicyId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260818071205_20260818123812_AddCybersecurityComplianceTables'
)
BEGIN
    CREATE INDEX [IX_DeviceComplianceResults_TenantId_ScanId] ON [DeviceComplianceResults] ([TenantId], [ScanId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260818071205_20260818123812_AddCybersecurityComplianceTables'
)
BEGIN
    CREATE INDEX [IX_DeviceComplianceResults_TenantId_Status] ON [DeviceComplianceResults] ([TenantId], [Status]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260818071205_20260818123812_AddCybersecurityComplianceTables'
)
BEGIN
    CREATE INDEX [IX_DeviceComplianceScans_DeviceId] ON [DeviceComplianceScans] ([DeviceId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260818071205_20260818123812_AddCybersecurityComplianceTables'
)
BEGIN
    CREATE INDEX [IX_DeviceComplianceScans_TenantId_DeviceId] ON [DeviceComplianceScans] ([TenantId], [DeviceId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260818071205_20260818123812_AddCybersecurityComplianceTables'
)
BEGIN
    CREATE INDEX [IX_DeviceComplianceScans_TenantId_OverallStatus] ON [DeviceComplianceScans] ([TenantId], [OverallStatus]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260818071205_20260818123812_AddCybersecurityComplianceTables'
)
BEGIN
    CREATE INDEX [IX_DeviceComplianceScans_TenantId_ScannedAtUtc] ON [DeviceComplianceScans] ([TenantId], [ScannedAtUtc]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260818071205_20260818123812_AddCybersecurityComplianceTables'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260818071205_20260818123812_AddCybersecurityComplianceTables', N'10.0.11');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260818093007_AddVulnerabilityAndFirmwareAnalysisTables'
)
BEGIN
    CREATE TABLE [FirmwareUpgradeRecommendations] (
        [Id] uniqueidentifier NOT NULL,
        [TenantId] uniqueidentifier NOT NULL,
        [DeviceId] uniqueidentifier NOT NULL,
        [CurrentVersion] nvarchar(100) NOT NULL,
        [RecommendedVersion] nvarchar(100) NOT NULL,
        [Priority] int NOT NULL,
        [Reasoning] nvarchar(2000) NOT NULL,
        [AssociatedVulnerabilityCount] int NOT NULL,
        [CriticalVulnerabilityCount] int NOT NULL,
        [IsApplied] bit NOT NULL,
        [CreatedAtUtc] datetime2 NOT NULL,
        [CreatedBy] nvarchar(max) NULL,
        [LastModifiedAtUtc] datetime2 NULL,
        [LastModifiedBy] nvarchar(max) NULL,
        CONSTRAINT [PK_FirmwareUpgradeRecommendations] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_FirmwareUpgradeRecommendations_Devices_DeviceId] FOREIGN KEY ([DeviceId]) REFERENCES [Devices] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260818093007_AddVulnerabilityAndFirmwareAnalysisTables'
)
BEGIN
    CREATE TABLE [SecurityAdvisories] (
        [Id] uniqueidentifier NOT NULL,
        [TenantId] uniqueidentifier NOT NULL,
        [AdvisoryId] nvarchar(100) NOT NULL,
        [Vendor] nvarchar(100) NOT NULL,
        [Title] nvarchar(250) NOT NULL,
        [Summary] nvarchar(4000) NOT NULL,
        [Severity] int NOT NULL,
        [RemediationGuidance] nvarchar(4000) NULL,
        [ReferenceUrl] nvarchar(500) NULL,
        [PublishedAtUtc] datetime2 NULL,
        [CreatedAtUtc] datetime2 NOT NULL,
        [CreatedBy] nvarchar(max) NULL,
        [LastModifiedAtUtc] datetime2 NULL,
        [LastModifiedBy] nvarchar(max) NULL,
        CONSTRAINT [PK_SecurityAdvisories] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260818093007_AddVulnerabilityAndFirmwareAnalysisTables'
)
BEGIN
    CREATE TABLE [Vulnerabilities] (
        [Id] uniqueidentifier NOT NULL,
        [TenantId] uniqueidentifier NOT NULL,
        [CveId] nvarchar(50) NOT NULL,
        [Title] nvarchar(250) NOT NULL,
        [Description] nvarchar(4000) NOT NULL,
        [Severity] int NOT NULL,
        [CvssScore] decimal(3,1) NOT NULL,
        [AffectedVendor] nvarchar(100) NULL,
        [AffectedModel] nvarchar(100) NULL,
        [AffectedVersionMin] nvarchar(100) NULL,
        [AffectedVersionMax] nvarchar(100) NULL,
        [PatchedVersion] nvarchar(100) NULL,
        [PublishedAtUtc] datetime2 NULL,
        [CreatedAtUtc] datetime2 NOT NULL,
        [CreatedBy] nvarchar(max) NULL,
        [LastModifiedAtUtc] datetime2 NULL,
        [LastModifiedBy] nvarchar(max) NULL,
        CONSTRAINT [PK_Vulnerabilities] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260818093007_AddVulnerabilityAndFirmwareAnalysisTables'
)
BEGIN
    CREATE TABLE [DeviceVulnerabilityMatches] (
        [Id] uniqueidentifier NOT NULL,
        [TenantId] uniqueidentifier NOT NULL,
        [DeviceId] uniqueidentifier NOT NULL,
        [VulnerabilityId] uniqueidentifier NOT NULL,
        [SecurityAdvisoryId] uniqueidentifier NULL,
        [DetectedFirmwareVersion] nvarchar(100) NOT NULL,
        [MatchStatus] int NOT NULL,
        [DetectedAtUtc] datetime2 NOT NULL,
        [ResolutionNotes] nvarchar(2000) NULL,
        [CreatedAtUtc] datetime2 NOT NULL,
        [CreatedBy] nvarchar(max) NULL,
        [LastModifiedAtUtc] datetime2 NULL,
        [LastModifiedBy] nvarchar(max) NULL,
        CONSTRAINT [PK_DeviceVulnerabilityMatches] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_DeviceVulnerabilityMatches_Devices_DeviceId] FOREIGN KEY ([DeviceId]) REFERENCES [Devices] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_DeviceVulnerabilityMatches_SecurityAdvisories_SecurityAdvisoryId] FOREIGN KEY ([SecurityAdvisoryId]) REFERENCES [SecurityAdvisories] ([Id]) ON DELETE SET NULL,
        CONSTRAINT [FK_DeviceVulnerabilityMatches_Vulnerabilities_VulnerabilityId] FOREIGN KEY ([VulnerabilityId]) REFERENCES [Vulnerabilities] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260818093007_AddVulnerabilityAndFirmwareAnalysisTables'
)
BEGIN
    CREATE INDEX [IX_DeviceVulnerabilityMatches_DeviceId] ON [DeviceVulnerabilityMatches] ([DeviceId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260818093007_AddVulnerabilityAndFirmwareAnalysisTables'
)
BEGIN
    CREATE INDEX [IX_DeviceVulnerabilityMatches_SecurityAdvisoryId] ON [DeviceVulnerabilityMatches] ([SecurityAdvisoryId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260818093007_AddVulnerabilityAndFirmwareAnalysisTables'
)
BEGIN
    CREATE INDEX [IX_DeviceVulnerabilityMatches_TenantId_DetectedAtUtc] ON [DeviceVulnerabilityMatches] ([TenantId], [DetectedAtUtc]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260818093007_AddVulnerabilityAndFirmwareAnalysisTables'
)
BEGIN
    CREATE INDEX [IX_DeviceVulnerabilityMatches_TenantId_DeviceId_MatchStatus] ON [DeviceVulnerabilityMatches] ([TenantId], [DeviceId], [MatchStatus]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260818093007_AddVulnerabilityAndFirmwareAnalysisTables'
)
BEGIN
    CREATE UNIQUE INDEX [IX_DeviceVulnerabilityMatches_TenantId_DeviceId_VulnerabilityId] ON [DeviceVulnerabilityMatches] ([TenantId], [DeviceId], [VulnerabilityId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260818093007_AddVulnerabilityAndFirmwareAnalysisTables'
)
BEGIN
    CREATE INDEX [IX_DeviceVulnerabilityMatches_VulnerabilityId] ON [DeviceVulnerabilityMatches] ([VulnerabilityId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260818093007_AddVulnerabilityAndFirmwareAnalysisTables'
)
BEGIN
    CREATE INDEX [IX_FirmwareUpgradeRecommendations_DeviceId] ON [FirmwareUpgradeRecommendations] ([DeviceId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260818093007_AddVulnerabilityAndFirmwareAnalysisTables'
)
BEGIN
    CREATE INDEX [IX_FirmwareUpgradeRecommendations_TenantId_DeviceId_IsApplied] ON [FirmwareUpgradeRecommendations] ([TenantId], [DeviceId], [IsApplied]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260818093007_AddVulnerabilityAndFirmwareAnalysisTables'
)
BEGIN
    CREATE INDEX [IX_FirmwareUpgradeRecommendations_TenantId_Priority] ON [FirmwareUpgradeRecommendations] ([TenantId], [Priority]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260818093007_AddVulnerabilityAndFirmwareAnalysisTables'
)
BEGIN
    CREATE UNIQUE INDEX [IX_SecurityAdvisories_TenantId_AdvisoryId] ON [SecurityAdvisories] ([TenantId], [AdvisoryId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260818093007_AddVulnerabilityAndFirmwareAnalysisTables'
)
BEGIN
    CREATE INDEX [IX_SecurityAdvisories_TenantId_Severity] ON [SecurityAdvisories] ([TenantId], [Severity]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260818093007_AddVulnerabilityAndFirmwareAnalysisTables'
)
BEGIN
    CREATE INDEX [IX_SecurityAdvisories_TenantId_Vendor] ON [SecurityAdvisories] ([TenantId], [Vendor]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260818093007_AddVulnerabilityAndFirmwareAnalysisTables'
)
BEGIN
    CREATE INDEX [IX_Vulnerabilities_TenantId_AffectedVendor_AffectedModel] ON [Vulnerabilities] ([TenantId], [AffectedVendor], [AffectedModel]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260818093007_AddVulnerabilityAndFirmwareAnalysisTables'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Vulnerabilities_TenantId_CveId] ON [Vulnerabilities] ([TenantId], [CveId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260818093007_AddVulnerabilityAndFirmwareAnalysisTables'
)
BEGIN
    CREATE INDEX [IX_Vulnerabilities_TenantId_Severity] ON [Vulnerabilities] ([TenantId], [Severity]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260818093007_AddVulnerabilityAndFirmwareAnalysisTables'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260818093007_AddVulnerabilityAndFirmwareAnalysisTables', N'10.0.11');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260818132003_AddThreatDetectionTables'
)
BEGIN
    CREATE TABLE [ConfigurationDriftRecords] (
        [Id] uniqueidentifier NOT NULL,
        [TenantId] uniqueidentifier NOT NULL,
        [DeviceId] uniqueidentifier NOT NULL,
        [BaselineBackupId] uniqueidentifier NOT NULL,
        [CurrentBackupId] uniqueidentifier NOT NULL,
        [HasDrift] bit NOT NULL,
        [AddedLinesCount] int NOT NULL,
        [RemovedLinesCount] int NOT NULL,
        [ModifiedLinesCount] int NOT NULL,
        [DifferencesJson] nvarchar(max) NULL,
        [DetectedAtUtc] datetime2 NOT NULL,
        [Severity] int NOT NULL,
        [IsAcknowledged] bit NOT NULL,
        [AcknowledgmentNotes] nvarchar(1000) NULL,
        [CreatedAtUtc] datetime2 NOT NULL,
        [CreatedBy] nvarchar(max) NULL,
        [LastModifiedAtUtc] datetime2 NULL,
        [LastModifiedBy] nvarchar(max) NULL,
        CONSTRAINT [PK_ConfigurationDriftRecords] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_ConfigurationDriftRecords_ConfigurationBackups_BaselineBackupId] FOREIGN KEY ([BaselineBackupId]) REFERENCES [ConfigurationBackups] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_ConfigurationDriftRecords_ConfigurationBackups_CurrentBackupId] FOREIGN KEY ([CurrentBackupId]) REFERENCES [ConfigurationBackups] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_ConfigurationDriftRecords_Devices_DeviceId] FOREIGN KEY ([DeviceId]) REFERENCES [Devices] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260818132003_AddThreatDetectionTables'
)
BEGIN
    CREATE TABLE [ThreatDetectionRules] (
        [Id] uniqueidentifier NOT NULL,
        [TenantId] uniqueidentifier NOT NULL,
        [RuleName] nvarchar(150) NOT NULL,
        [ThreatType] int NOT NULL,
        [DefaultSeverity] int NOT NULL,
        [FailureThreshold] int NOT NULL,
        [TimeWindowMinutes] int NOT NULL,
        [IsEnabled] bit NOT NULL,
        [Description] nvarchar(500) NULL,
        [CreatedAtUtc] datetime2 NOT NULL,
        [CreatedBy] nvarchar(max) NULL,
        [LastModifiedAtUtc] datetime2 NULL,
        [LastModifiedBy] nvarchar(max) NULL,
        CONSTRAINT [PK_ThreatDetectionRules] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260818132003_AddThreatDetectionTables'
)
BEGIN
    CREATE TABLE [ThreatIndicators] (
        [Id] uniqueidentifier NOT NULL,
        [TenantId] uniqueidentifier NOT NULL,
        [ThreatType] int NOT NULL,
        [Severity] int NOT NULL,
        [Status] int NOT NULL,
        [Title] nvarchar(200) NOT NULL,
        [Description] nvarchar(2000) NOT NULL,
        [SourceIp] nvarchar(100) NOT NULL,
        [TargetDeviceId] uniqueidentifier NULL,
        [TargetUser] nvarchar(150) NOT NULL,
        [AttemptCount] int NOT NULL,
        [FirstDetectedAtUtc] datetime2 NOT NULL,
        [LastDetectedAtUtc] datetime2 NOT NULL,
        [ResolutionNotes] nvarchar(2000) NULL,
        [IndicatorMetadataJson] nvarchar(max) NULL,
        [CreatedAtUtc] datetime2 NOT NULL,
        [CreatedBy] nvarchar(max) NULL,
        [LastModifiedAtUtc] datetime2 NULL,
        [LastModifiedBy] nvarchar(max) NULL,
        CONSTRAINT [PK_ThreatIndicators] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_ThreatIndicators_Devices_TargetDeviceId] FOREIGN KEY ([TargetDeviceId]) REFERENCES [Devices] ([Id]) ON DELETE SET NULL
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260818132003_AddThreatDetectionTables'
)
BEGIN
    CREATE INDEX [IX_ConfigurationDriftRecords_BaselineBackupId] ON [ConfigurationDriftRecords] ([BaselineBackupId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260818132003_AddThreatDetectionTables'
)
BEGIN
    CREATE INDEX [IX_ConfigurationDriftRecords_CurrentBackupId] ON [ConfigurationDriftRecords] ([CurrentBackupId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260818132003_AddThreatDetectionTables'
)
BEGIN
    CREATE INDEX [IX_ConfigurationDriftRecords_DeviceId] ON [ConfigurationDriftRecords] ([DeviceId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260818132003_AddThreatDetectionTables'
)
BEGIN
    CREATE INDEX [IX_ConfigurationDriftRecords_TenantId_DeviceId_DetectedAtUtc] ON [ConfigurationDriftRecords] ([TenantId], [DeviceId], [DetectedAtUtc]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260818132003_AddThreatDetectionTables'
)
BEGIN
    CREATE INDEX [IX_ConfigurationDriftRecords_TenantId_HasDrift] ON [ConfigurationDriftRecords] ([TenantId], [HasDrift]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260818132003_AddThreatDetectionTables'
)
BEGIN
    CREATE INDEX [IX_ThreatDetectionRules_TenantId_IsEnabled] ON [ThreatDetectionRules] ([TenantId], [IsEnabled]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260818132003_AddThreatDetectionTables'
)
BEGIN
    CREATE UNIQUE INDEX [IX_ThreatDetectionRules_TenantId_ThreatType] ON [ThreatDetectionRules] ([TenantId], [ThreatType]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260818132003_AddThreatDetectionTables'
)
BEGIN
    CREATE INDEX [IX_ThreatIndicators_TargetDeviceId] ON [ThreatIndicators] ([TargetDeviceId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260818132003_AddThreatDetectionTables'
)
BEGIN
    CREATE INDEX [IX_ThreatIndicators_TenantId_LastDetectedAtUtc] ON [ThreatIndicators] ([TenantId], [LastDetectedAtUtc]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260818132003_AddThreatDetectionTables'
)
BEGIN
    CREATE INDEX [IX_ThreatIndicators_TenantId_Severity] ON [ThreatIndicators] ([TenantId], [Severity]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260818132003_AddThreatDetectionTables'
)
BEGIN
    CREATE INDEX [IX_ThreatIndicators_TenantId_SourceIp] ON [ThreatIndicators] ([TenantId], [SourceIp]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260818132003_AddThreatDetectionTables'
)
BEGIN
    CREATE INDEX [IX_ThreatIndicators_TenantId_TargetDeviceId] ON [ThreatIndicators] ([TenantId], [TargetDeviceId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260818132003_AddThreatDetectionTables'
)
BEGIN
    CREATE INDEX [IX_ThreatIndicators_TenantId_ThreatType_Status] ON [ThreatIndicators] ([TenantId], [ThreatType], [Status]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260818132003_AddThreatDetectionTables'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260818132003_AddThreatDetectionTables', N'10.0.11');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260819132225_AddPerformanceOptimizationIndexes'
)
BEGIN
    DROP INDEX [DeviceId] ON [SyslogMessages];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260819132225_AddPerformanceOptimizationIndexes'
)
BEGIN
    DROP INDEX [Facility] ON [SyslogMessages];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260819132225_AddPerformanceOptimizationIndexes'
)
BEGIN
    DROP INDEX [Severity] ON [SyslogMessages];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260819132225_AddPerformanceOptimizationIndexes'
)
BEGIN
    DROP INDEX [SourceIpAddress] ON [SyslogMessages];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260819132225_AddPerformanceOptimizationIndexes'
)
BEGIN
    DROP INDEX [TimestampUtc] ON [SyslogMessages];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260819132225_AddPerformanceOptimizationIndexes'
)
BEGIN
    CREATE INDEX [IX_SyslogMessages_TenantId_DeviceId] ON [SyslogMessages] ([TenantId], [DeviceId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260819132225_AddPerformanceOptimizationIndexes'
)
BEGIN
    CREATE INDEX [IX_SyslogMessages_TenantId_DeviceId_TimestampUtc] ON [SyslogMessages] ([TenantId], [DeviceId], [TimestampUtc]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260819132225_AddPerformanceOptimizationIndexes'
)
BEGIN
    CREATE INDEX [IX_SyslogMessages_TenantId_Facility] ON [SyslogMessages] ([TenantId], [Facility]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260819132225_AddPerformanceOptimizationIndexes'
)
BEGIN
    CREATE INDEX [IX_SyslogMessages_TenantId_Severity] ON [SyslogMessages] ([TenantId], [Severity]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260819132225_AddPerformanceOptimizationIndexes'
)
BEGIN
    CREATE INDEX [IX_SyslogMessages_TenantId_Severity_TimestampUtc] ON [SyslogMessages] ([TenantId], [Severity], [TimestampUtc]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260819132225_AddPerformanceOptimizationIndexes'
)
BEGIN
    CREATE INDEX [IX_SyslogMessages_TenantId_SourceIpAddress] ON [SyslogMessages] ([TenantId], [SourceIpAddress]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260819132225_AddPerformanceOptimizationIndexes'
)
BEGIN
    CREATE INDEX [IX_SyslogMessages_TenantId_TimestampUtc] ON [SyslogMessages] ([TenantId], [TimestampUtc]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260819132225_AddPerformanceOptimizationIndexes'
)
BEGIN
    CREATE INDEX [IX_Devices_TenantId_CreatedAtUtc] ON [Devices] ([TenantId], [CreatedAtUtc]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260819132225_AddPerformanceOptimizationIndexes'
)
BEGIN
    CREATE INDEX [IX_Devices_TenantId_Status_DeviceType_CreatedAtUtc] ON [Devices] ([TenantId], [Status], [DeviceType], [CreatedAtUtc]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260819132225_AddPerformanceOptimizationIndexes'
)
BEGIN
    CREATE INDEX [IX_DeviceMetricsRaw_TenantId_TimestampUtc] ON [DeviceMetricsRaw] ([TenantId], [TimestampUtc]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260819132225_AddPerformanceOptimizationIndexes'
)
BEGIN
    CREATE INDEX [IX_AuditLogs_TenantId_UserId_TimestampUtc] ON [AuditLogs] ([TenantId], [UserId], [TimestampUtc]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260819132225_AddPerformanceOptimizationIndexes'
)
BEGIN
    CREATE INDEX [IX_Alerts_TenantId_State_Severity_TriggeredAtUtc] ON [Alerts] ([TenantId], [State], [Severity], [TriggeredAtUtc]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260819132225_AddPerformanceOptimizationIndexes'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260819132225_AddPerformanceOptimizationIndexes', N'10.0.11');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260824110429_UpdateNmsSchema'
)
BEGIN
    CREATE INDEX [IX_ThreatDetectionRules_TenantId_IsEnabled_RuleName] ON [ThreatDetectionRules] ([TenantId], [IsEnabled], [RuleName]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260824110429_UpdateNmsSchema'
)
BEGIN
    CREATE INDEX [IX_PollProfiles_TenantId_IsDefault_IsEnabled] ON [PollProfiles] ([TenantId], [IsDefault], [IsEnabled]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260824110429_UpdateNmsSchema'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260824110429_UpdateNmsSchema', N'10.0.11');
END;

COMMIT;
GO

