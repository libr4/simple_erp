using System.Globalization;
using FluentValidation;

namespace Backend.Validators;

/// <summary>
/// Validador para request de cálculo de fees (multa e juros).
/// Valida data de vencimento e valor.
/// </summary>
public class FeesRequestValidator : AbstractValidator<FeesQueryRequest>
{
    private static readonly TimeZoneInfo BrazilTZ =
        TimeZoneInfo.FindSystemTimeZoneById("America/Sao_Paulo");

    public FeesRequestValidator()
    {
        RuleFor(x => x.Vencimento)
            .NotEmpty()
            .WithMessage("Data de vencimento é obrigatória.")
            .Must(BeValidDateFormat)
            .WithMessage("Data de vencimento inválida. Formato esperado: dd/MM/yyyy");

        RuleFor(x => x.Valor)
            .GreaterThan(0)
            .WithMessage("Valor deve ser maior que 0.")
            .LessThanOrEqualTo(decimal.MaxValue)
            .WithMessage("Valor excede o limite permitido.");
    }

    private static bool BeValidDateFormat(string dateString)
    {
        if (string.IsNullOrWhiteSpace(dateString))
            return false;

        var formats = new[] { "dd/MM/yyyy" };
        return DateTime.TryParseExact(
            dateString.Trim(),
            formats,
            new CultureInfo("pt-BR"),
            DateTimeStyles.None,
            out _);
    }
}

/// <summary>
/// DTO para query parameters de fees.
/// </summary>
public class FeesQueryRequest
{
    public string Vencimento { get; set; } = string.Empty;
    public decimal Valor { get; set; }
}
