using FluentAssertions;
using System.Net;
using System.Net.Http.Json;

namespace AgroOpsManager.Tests.Integration;

public class ApiEndpointsTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public ApiEndpointsTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task HealthCheck_ShouldReturnOk()
    {
        // Act
        var response = await _client.GetAsync("/api/health-check");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task DashboardSummary_ShouldReturnOk()
    {
        // Act
        var response = await _client.GetAsync("/api/dashboard/summary");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Fields_ShouldReturnOk()
    {
        // Act
        var response = await _client.GetAsync("/api/fields");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task LowStockInventory_ShouldReturnOk()
    {
        // Act
        var response = await _client.GetAsync("/api/inventory/low-stock");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task MachineServiceAlerts_ShouldReturnOk()
    {
        // Act
        var response = await _client.GetAsync("/api/machines/service-alerts");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task UpcomingFieldWorks_ShouldReturnOk()
    {
        // Act
        var response = await _client.GetAsync("/api/field-works/upcoming");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Fields_ShouldReturnJsonArray()
    {
        // Act
        var fields = await _client.GetFromJsonAsync<List<FieldApiResponse>>("/api/fields");

        // Assert
        fields.Should().NotBeNull();
        fields.Should().NotBeEmpty();
        fields!.First().Name.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public async Task DashboardSummary_ShouldReturnExpectedJson()
    {
        // Act
        var summary = await _client.GetFromJsonAsync<DashboardSummaryApiResponse>("/api/dashboard/summary");

        // Assert
        summary.Should().NotBeNull();
        summary!.ActiveFieldsCount.Should().BeGreaterThanOrEqualTo(0);
        summary.MachinesCount.Should().BeGreaterThanOrEqualTo(0);
        summary.PlannedFieldWorksCount.Should().BeGreaterThanOrEqualTo(0);
        summary.CompletedFieldWorksCount.Should().BeGreaterThanOrEqualTo(0);
        summary.InventoryValue.Should().BeGreaterThanOrEqualTo(0);
    }

    [Fact]
    public async Task FieldById_ShouldReturnNotFound_WhenFieldDoesNotExist()
    {
        // Act
        var response = await _client.GetAsync("/api/fields/999999");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    private class FieldApiResponse
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public decimal AreaInHectares { get; set; }

        public string Location { get; set; } = string.Empty;

        public string SoilType { get; set; } = string.Empty;

        public string CurrentCrop { get; set; } = string.Empty;

        public string Status { get; set; } = string.Empty;

        public string? Notes { get; set; }
    }

    private class DashboardSummaryApiResponse
    {
        public int ActiveFieldsCount { get; set; }

        public int MachinesCount { get; set; }

        public int MachinesRequiringServiceCount { get; set; }

        public int PlannedFieldWorksCount { get; set; }

        public int CompletedFieldWorksCount { get; set; }

        public int LowStockItemsCount { get; set; }

        public decimal TotalFieldWorksCost { get; set; }

        public decimal InventoryValue { get; set; }
    }
}