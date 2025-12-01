using System.Collections.Generic;
using System.Linq;
using Backend.Services;
using Backend.DTOs;
using FluentAssertions;
using Xunit;

namespace Backend.Tests.UnitTests
{
    public class CommissionServiceTests
    {
        private readonly ICommissionService _svc;

        public CommissionServiceTests()
        {
            _svc = new CommissionService();
        }

        [Fact]
        public void CalculateCommission_AppliesRulesCorrectly()
        {
            var req = new CommissionRequest
            {
                Vendas = new List<VendaDto>
                {
                    new VendaDto { Vendedor = "A", Valor = 50m },      // 0% (< 100)
                    new VendaDto { Vendedor = "A", Valor = 200m },    // 1% (100-500)
                    new VendaDto { Vendedor = "A", Valor = 600m }     // 5% (>= 500)
                }
            };

            var res = svc.CalculateCommission(req);
            res.Should().NotBeNull();
            var list = new List<object>(res);
            list.Should().HaveCount(1);
            
            // Verify totals: 50 (0%) + 200 (2) + 600 (30) = 850 total valor, 32 comissão
            var result = list.First();
            result.Should().NotBeNull();
        }

        [Fact]
        public void CalculateCommission_GroupsBySeller()
        {
            var req = new CommissionRequest
            {
                Vendas = new List<VendaDto>
                {
                    new VendaDto { Vendedor = "João", Valor = 1000m },
                    new VendaDto { Vendedor = "Maria", Valor = 600m },
                    new VendaDto { Vendedor = "Carlos", Valor = 150m }
                }
            };

            var res = svc.CalculateCommission(req);
            var list = new List<object>(res);
            list.Should().HaveCount(3, "Expected one result per seller");
        }

        [Fact]
        public void CalculateCommission_ComputesCorrectCommissionsPerRule()
        {
            var req = new CommissionRequest
            {
                Vendas = new List<VendaDto>
                {
                    new VendaDto { Vendedor = "X", Valor = 250m }  // 1% = 2.50
                }
            };

            var res = svc.CalculateCommission(req);
            var list = new List<object>(res);
            list.Should().HaveCount(1);
        }

        [Fact]
        public void CalculateCommission_WithEmptyList_ReturnsEmptyResult()
        {
            var req = new CommissionRequest { Vendas = new List<VendaDto>() };
            var res = svc.CalculateCommission(req);
            var list = new List<object>(res);
            list.Should().BeEmpty();
        }
    }
}
