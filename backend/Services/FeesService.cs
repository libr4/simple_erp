using Backend.DTOs;
using Backend.Validators;
using System;

namespace Backend.Services
{
    public interface IFeesService
    {
        /// <summary>
        /// Calcula fees baseado em request validado. Request contains vencimento string (dd/MM/yyyy) and valor.
        /// </summary>
        FeesResponse CalculateFees(FeesQueryRequest request);
    }

    public class FeesService : IFeesService
    {
         private static readonly TimeZoneInfo BrazilTZ =
            TimeZoneInfo.FindSystemTimeZoneById("America/Sao_Paulo");

        private const decimal DAILY_RATE = 0.025m;

        public FeesResponse CalculateFees(FeesQueryRequest request)
        {
            // Request is validated by FluentValidation; parse the date here.
            var formats = new[] { "dd/MM/yyyy" };
            DateTime.TryParseExact(
                request.Vencimento.Trim(),
                formats,
                new System.Globalization.CultureInfo("pt-BR"),
                System.Globalization.DateTimeStyles.None,
                out var vencimentoDate);

            var todayBrazil = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, BrazilTZ).Date;

            var daysLate = Math.Max((todayBrazil - vencimentoDate.Date).Days, 0);
            var juros = request.Valor * DAILY_RATE * daysLate;

            // Round juros and valorComJuros to 2 decimal places for currency representation
            var jurosRounded = decimal.Round(juros, 2, MidpointRounding.AwayFromZero);
            var valorComJurosRounded = decimal.Round(request.Valor + jurosRounded, 2, MidpointRounding.AwayFromZero);

            return new FeesResponse
            {
                ValorOriginal = request.Valor,
                DiasAtraso = daysLate,
                Juros = jurosRounded,
                ValorComJuros = valorComJurosRounded
            };
        }
    }
}
