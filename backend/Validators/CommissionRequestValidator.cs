using Backend.DTOs;
using FluentValidation;

namespace Backend.Validators;

/// <summary>
/// Validador para CommissionRequest usando FluentValidation.
/// </summary>
public class CommissionRequestValidator : AbstractValidator<CommissionRequest>
{
    public CommissionRequestValidator()
    {
        RuleFor(x => x.Vendas)
            .NotNull()
            .WithMessage("Vendas é obrigatório.")
            .NotEmpty()
            .WithMessage("Lista de vendas não pode estar vazia.");

        RuleForEach(x => x.Vendas)
            .ChildRules(vendas =>
            {
                vendas.RuleFor(v => v.Vendedor)
                    .NotEmpty()
                    .WithMessage("Vendedor é obrigatório.")
                    .MaximumLength(200)
                    .WithMessage("Nome do vendedor não pode exceder 200 caracteres.");

                vendas.RuleFor(v => v.Valor)
                    .GreaterThanOrEqualTo(0)
                    .WithMessage("Valor da venda deve ser maior ou igual a 0.")
                    .LessThanOrEqualTo(decimal.MaxValue)
                    .WithMessage("Valor da venda excede o limite permitido.");
            });
    }
}
