using Backend.DTOs;
using Backend.Models;
using Backend.Constants;
using System.Collections.Generic;
using System.Linq;

namespace Backend.Services
{
    public interface ICommissionService
    {
        IEnumerable<object> CalculateCommission(CommissionRequest req);
    }

    public class CommissionService : ICommissionService
    {
        // Use centralized constants to avoid magic numbers in the implementation
        private readonly decimal _rateLow = CommissionConstants.RateLow;
        private readonly decimal _rateHigh = CommissionConstants.RateHigh;
        public IEnumerable<object> CalculateCommission(CommissionRequest req)
        {
            var grouped = req.Vendas.GroupBy(v => v.Vendedor);
            var result = new List<object>();
            foreach (var g in grouped)
            {
                decimal totalVendas = 0;
                var itens = new List<object>();
                decimal totalComissaoUnrounded = 0m;
                foreach (var v in g)
                {
                    decimal com = 0m;
                    if (v.Valor < 100) com = 0m;
                    else if (v.Valor < 500) com = v.Valor * _rateLow;
                    else com = v.Valor * _rateHigh;

                    // Keep per-item commission for display rounded to 2 decimals
                    var comItemDisplay = decimal.Round(com, 2, MidpointRounding.AwayFromZero);

                    totalComissaoUnrounded += com;
                    totalVendas += v.Valor;
                    itens.Add(new { valor = v.Valor, comissao = comItemDisplay });
                }

                // Sum all unrounded item commissions, then round once for the total
                var comissaoTotalRounded = decimal.Round(totalComissaoUnrounded, 2, MidpointRounding.AwayFromZero);

                result.Add(new {
                    vendedor = g.Key,
                    totalVendas = totalVendas,
                    comissaoTotal = comissaoTotalRounded,
                    itens = itens
                });
            }
            return result;
        }
    }
}
