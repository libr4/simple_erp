using Backend.DTOs;
using FluentValidation;

namespace Backend.Validators;

/// <summary>
/// Validador para MovimentacaoCreateRequest usando FluentValidation.
/// </summary>
public class MovimentacaoCreateRequestValidator : AbstractValidator<MovimentacaoCreateRequest>
{
    private static readonly string[] TiposValidos = { "ENTRADA", "SAIDA", "INVENTARIO" };

    public MovimentacaoCreateRequestValidator()
    {
        RuleFor(x => x.CodigoProduto)
            .GreaterThan(0)
            .WithMessage("CodigoProduto deve ser maior que 0.");

        RuleFor(x => x.Tipo)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage("Tipo é obrigatório.")
            .Must(x => x != null && TiposValidos.Contains(x.Trim().ToUpperInvariant()))
            .WithMessage("Tipo deve ser 'ENTRADA', 'SAIDA' ou 'INVENTARIO'.");

        RuleFor(x => x.Quantidade)
            .GreaterThan(0)
            .WithMessage("Quantidade deve ser maior que 0.");

        RuleFor(x => x.Descricao)
            .MaximumLength(1000)
            .WithMessage("Descrição não pode exceder 1000 caracteres.");
    }
}
