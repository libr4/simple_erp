using System;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Backend.DTOs;
using FluentAssertions;
using Xunit;

namespace Backend.Tests.IntegrationTests
{
    public class FeesEndpointTests : IAsyncLifetime
    {
        private readonly TestWebApplicationFactory _factory;
        private HttpClient _client = null!;

        public FeesEndpointTests()
        {
            _factory = new TestWebApplicationFactory();
        }

        public async Task InitializeAsync()
        {
            await _factory.InitializeContainerAsync();
            _client = _factory.CreateClient();
        }

        public async Task DisposeAsync()
        {
            await _factory.DisposeAsync();
        }

        [Fact]
        public async Task GetFees_WithValidFutureDate_ReturnsZeroJuros()
        {
            var today = DateTime.Now.Date;
            var futureDate = today.AddDays(10);
            var vencStr = futureDate.ToString("dd/MM/yyyy");

            var response = await _client.GetAsync($"/api/v1/fees?vencimento={vencStr}&valor=1000");
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var content = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<FeesResponse>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            
            result.Should().NotBeNull();
            result!.DiasAtraso.Should().Be(0);
            result.Juros.Should().Be(0);
            result.ValorComJuros.Should().Be(1000);
        }

        [Fact]
        public async Task GetFees_WithValidPastDate_ReturnsJuros()
        {
            var today = DateTime.Now.Date;
            var pastDate = today.AddDays(-10);
            var vencStr = pastDate.ToString("dd/MM/yyyy");

            var response = await _client.GetAsync($"/api/v1/fees?vencimento={vencStr}&valor=1000");
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var content = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<FeesResponse>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            
            result.Should().NotBeNull();
            result!.DiasAtraso.Should().Be(10);
            result.Juros.Should().Be(1000m * 0.025m * 10);
            result.ValorComJuros.Should().Be(1000m + result.Juros);
        }

        [Fact]
        public async Task GetFees_WithInvalidDateFormat_ReturnsBadRequest()
        {
            var response = await _client.GetAsync("/api/v1/fees?vencimento=2025-01-01&valor=1000");
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task GetFees_WithMissingVencimento_ReturnsBadRequest()
        {
            var response = await _client.GetAsync("/api/v1/fees?valor=1000");
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task GetFees_WithInvalidValor_ReturnsBadRequest()
        {
            var response = await _client.GetAsync("/api/v1/fees?vencimento=01/01/2025&valor=-100");
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }
    }
}
