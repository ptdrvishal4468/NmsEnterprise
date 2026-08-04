using System.Net;
using System.Net.Http.Json;
using Nms.Application.Common.Models;
using Nms.Application.Devices.Commands.CreateDevice;
using Nms.Application.Devices.Dtos;
using Nms.Application.Tenants.Commands.CreateTenant;
using Nms.Application.Tenants.Dtos;
using Nms.Domain.Enums;
using Xunit;

namespace Nms.IntegrationTests;

public class DeviceEndpointTests : IClassFixture<TestWebApplicationFactory>
{
    private readonly HttpClient _client;

    public DeviceEndpointTests(TestWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    private async Task EnsureTenantContextSetAsync()
    {
        if (!_client.DefaultRequestHeaders.Contains("X-Tenant-Id"))
        {
            // First create a tenant to get a valid tenant ID
            var createTenantCommand = new CreateTenantCommand("Device Test Tenant");
            var tenantResponse = await _client.PostAsJsonAsync("/api/v1/tenants", createTenantCommand);
            tenantResponse.EnsureSuccessStatusCode();

            var tenant = await tenantResponse.Content.ReadFromJsonAsync<TenantDto>();
            Assert.NotNull(tenant);

            // Set the X-Tenant-Id header on the HttpClient for all subsequent device requests
            _client.DefaultRequestHeaders.Add("X-Tenant-Id", tenant.Id.ToString());
        }
    }

    [Fact]
    public async Task CreateAndGetDevice_ShouldReturnCreatedAndOk()
    {
        // Arrange
        await EnsureTenantContextSetAsync();

        var createCommand = new CreateDeviceCommand(
            Name: "Core-Router-01",
            IpAddress: "10.0.10.1",
            DeviceType: DeviceType.Router,
            SnmpPort: 161,
            Hostname: "rtr01.internal",
            Vendor: "Cisco",
            Model: "ISR4451",
            SerialNumber: "SN-RTR-99",
            MacAddress: "00:1A:2B:3C:4D:5E",
            Site: "DataCenter-A",
            Location: "Rack-01"
        );

        // Act - 1. Create Device
        var createResponse = await _client.PostAsJsonAsync("/api/v1/devices", createCommand);

        // Assert - Creation
        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);
        var createdDevice = await createResponse.Content.ReadFromJsonAsync<DeviceDto>();
        Assert.NotNull(createdDevice);
        Assert.Equal("Core-Router-01", createdDevice.Name);
        Assert.Equal("10.0.10.1", createdDevice.IpAddress);
        Assert.Equal(DeviceType.Router, createdDevice.DeviceType);
        Assert.Equal("Cisco", createdDevice.Vendor);

        // Act - 2. Get Device By ID
        var getResponse = await _client.GetAsync($"/api/v1/devices/{createdDevice.Id}");

        // Assert - Retrieval
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);
        var fetchedDevice = await getResponse.Content.ReadFromJsonAsync<DeviceDto>();
        Assert.NotNull(fetchedDevice);
        Assert.Equal(createdDevice.Id, fetchedDevice.Id);
        Assert.Equal("Core-Router-01", fetchedDevice.Name);
    }

    [Fact]
    public async Task GetDevices_ShouldReturnPagedResult()
    {
        // Arrange
        await EnsureTenantContextSetAsync();

        // Act
        var response = await _client.GetAsync("/api/v1/devices?pageIndex=1&pageSize=10");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var result = await response.Content.ReadFromJsonAsync<PagedResult<DeviceDto>>();
        Assert.NotNull(result);
    }

    [Fact]
    public async Task CreateDevice_WithInvalidIp_ShouldReturnBadRequest()
    {
        // Arrange
        await EnsureTenantContextSetAsync();

        var invalidCommand = new CreateDeviceCommand(
            Name: "Invalid-Device",
            IpAddress: "999.999.999.999",
            DeviceType: DeviceType.Switch
        );

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/devices", invalidCommand);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}