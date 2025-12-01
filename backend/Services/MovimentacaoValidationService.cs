using Backend.Exceptions;
using Backend.Models;

namespace Backend.Services;

/// <summary>
/// Interface para parsing/normalização de tipos de movimentação.
/// Responsabilidade: converter a string de entrada em <see cref="TipoMovimentacao"/>.
/// </summary>
public interface IMovimentacaoParser
{
    /// <summary>
    /// Converte e valida a string de tipo de movimentação.
    /// </summary>
    /// <param name="tipoMovimentacao">String contendo o tipo de movimentação</param>
    /// <returns>Tipo de movimentação validado e normalizado</returns>
    /// <exception cref="InvalidMovementTypeException">Se tipo inválido</exception>
    TipoMovimentacao Parse(string tipoMovimentacao);
}

/// <summary>
/// Implementação do parser de movimentação.
/// Converte a entrada para <see cref="TipoMovimentacao"/> (case-insensitive).
/// </summary>
public class MovimentacaoParser : IMovimentacaoParser
{
    public TipoMovimentacao Parse(string tipoMovimentacao)
    {
        if (string.IsNullOrWhiteSpace(tipoMovimentacao))
        {
            throw new InvalidMovementTypeException("null");
        }

        var trimmed = tipoMovimentacao.Trim();

        // Tenta fazer parsing ignorando caixa
        if (Enum.TryParse<TipoMovimentacao>(trimmed, ignoreCase: true, out var tipo))
        {
            return tipo;
        }

        throw new InvalidMovementTypeException(tipoMovimentacao);
    }
}
