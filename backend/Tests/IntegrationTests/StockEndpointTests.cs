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
    public class StockEndpointTests : IAsyncLifetime
    {
        private readonly TestWebApplicationFactory _factory;
        private HttpClient _client = null!;

        public StockEndpointTests()
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
        public async Task PostStock_WithValidEntrada_ReturnsOkAndIncreasesStock()
        {
            var req = new MovimentacaoCreateRequest
            {
                CodigoProduto = 101,
                Tipo = "ENTRADA",
                Quantidade = 50,
                Descricao = "Entrada de teste"
            };

            var json = JsonSerializer.Serialize(req);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _client.PostAsync("/api/v1/estoque", content);

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var responseContent = await response.Content.ReadAsStringAsync();
            responseContent.Should().NotBeNullOrEmpty();
            
            // Parse and verify the returned product and movements
            var result = JsonSerializer.Deserialize<MovimentacaoResultDto>(responseContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            result.Should().NotBeNull();
            result!.Produto.Should().NotBeNull();
            result.MovimentacoesRecentes.Should().NotBeEmpty();
        }

        [Fact]
        public async Task PostStock_WithValidSaida_ReturnsOkAndDecreasesStock()
        {
            var req = new MovimentacaoCreateRequest
            {
                CodigoProduto = 101,
                Tipo = "SAIDA",
                Quantidade = 10,
                Descricao = "Saída de teste"
            };

            var json = JsonSerializer.Serialize(req);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _client.PostAsync("/api/v1/estoque", content);

            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task PostStock_WithInvalidType_ReturnsBadRequest()
        {
            var req = new MovimentacaoCreateRequest
            {
                CodigoProduto = 101,
                Tipo = "INVALIDO",
                Quantidade = 10,
                Descricao = "Test"
            };

            var json = JsonSerializer.Serialize(req);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _client.PostAsync("/api/v1/estoque", content);

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task PostStock_WithNonExistentProduct_ReturnsNotFound()
        {
            var req = new MovimentacaoCreateRequest
            {
                CodigoProduto = 999999,
                Tipo = "ENTRADA",
                Quantidade = 10,
                Descricao = "Test"
            };

            var json = JsonSerializer.Serialize(req);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _client.PostAsync("/api/v1/estoque", content);

            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task PostStock_WithInsufficientStock_ReturnsBadRequest()
        {
            var req = new MovimentacaoCreateRequest
            {
                CodigoProduto = 101,
                Tipo = "SAIDA",
                Quantidade = 10000,  // More than available
                Descricao = "Test"
            };

            var json = JsonSerializer.Serialize(req);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _client.PostAsync("/api/v1/estoque", content);

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }
    }
}
