using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Backend.DTOs;
using FluentAssertions;
using Xunit;

namespace Backend.Tests.IntegrationTests
{
    public class CommissionEndpointTests : IAsyncLifetime
    {
        private readonly TestWebApplicationFactory _factory;
        private HttpClient _client = null!;

        public CommissionEndpointTests()
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
        public async Task PostCommission_WithValidSales_ReturnsOkWithResults()
        {
            var req = new CommissionRequest
            {
                Vendas = new List<VendaDto>
                {
                    new VendaDto { Vendedor = "João", Valor = 50m },      // no commission
                    new VendaDto { Vendedor = "João", Valor = 200m },    // 1%
                    new VendaDto { Vendedor = "João", Valor = 600m }     // 5%
                }
            };

            var json = JsonSerializer.Serialize(req);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _client.PostAsync("/api/v1/comissao", content);

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var responseContent = await response.Content.ReadAsStringAsync();
            responseContent.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public async Task PostCommission_WithMultipleSellers_ReturnsGroupedResults()
        {
            var req = new CommissionRequest
            {
                Vendas = new List<VendaDto>
                {
                    new VendaDto { Vendedor = "Alice", Valor = 1000m },
                    new VendaDto { Vendedor = "Bob", Valor = 250m }
                }
            };

            var json = JsonSerializer.Serialize(req);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _client.PostAsync("/api/v1/comissao", content);

            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task PostCommission_WithEmptyVendas_ReturnsBadRequest()
        {
            var req = new CommissionRequest { Vendas = new List<VendaDto>() };
            var json = JsonSerializer.Serialize(req);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _client.PostAsync("/api/v1/comissao", content);

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task PostCommission_WithNullBody_ReturnsBadRequest()
        {
            var content = new StringContent("{}", Encoding.UTF8, "application/json");
            var response = await _client.PostAsync("/api/v1/comissao", content);
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }
    }
}
