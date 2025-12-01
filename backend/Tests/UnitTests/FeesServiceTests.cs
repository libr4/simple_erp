using Backend.Services;
using Backend.DTOs;
using FluentAssertions;
using System;
using Xunit;

namespace Backend.Tests.UnitTests
{
    public class FeesServiceTests
    {
        private readonly IFeesService _svc;

        public FeesServiceTests()
        {
            _svc = new FeesService();
        }

        [Fact]
        public void CalculateFees_NoDelay_ReturnsZeroJuros()
        {
            var today = DateTime.UtcNow.Date;
            var res = _svc.CalculateFees(today, 1000m, 0.025m);
            
            res.DiasAtraso.Should().Be(0);
            res.Juros.Should().Be(0);
            res.ValorComJuros.Should().Be(1000m);
            res.ValorOriginal.Should().Be(1000m);
        }

        [Fact]
        public void CalculateFees_WithDelay_ComputesSimpleInterest()
        {
            var venc = DateTime.UtcNow.Date.AddDays(-10);
            var res = _svc.CalculateFees(venc, 1000m, 0.025m);
            
            res.DiasAtraso.Should().Be(10);
            res.Juros.Should().Be(1000m * 0.025m * 10);  // 250
            res.ValorComJuros.Should().Be(1250m);
            res.ValorOriginal.Should().Be(1000m);
        }

        [Fact]
        public void CalculateFees_FutureDate_ReturnsZeroJuros()
        {
            var futureDate = DateTime.UtcNow.Date.AddDays(5);
            var res = _svc.CalculateFees(futureDate, 500m, 0.025m);
            
            res.DiasAtraso.Should().Be(0);
            res.Juros.Should().Be(0);
            res.ValorComJuros.Should().Be(500m);
        }

        [Fact]
        public void CalculateFees_ZeroDelay_ReturnsZeroJuros()
        {
            var today = DateTime.UtcNow.Date;
            var res = _svc.CalculateFees(today, 1000m, 0.025m);
            
            res.DiasAtraso.Should().Be(0);
            res.Juros.Should().Be(0);
        }

        [Fact]
        public void CalculateFees_DifferentAmounts_ComputesCorrectly()
        {
            var venc = DateTime.UtcNow.Date.AddDays(-5);
            var res = _svc.CalculateFees(500m, 500m, 0.025m);
            
            res.ValorOriginal.Should().Be(500m);
            res.DiasAtraso.Should().Be(5);
            res.Juros.Should().Be(500m * 0.025m * 5);
            res.ValorComJuros.Should().Be(500m + res.Juros);
        }
    }
}
