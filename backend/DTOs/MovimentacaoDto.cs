namespace Backend.DTOs;

/// <summary>
/// Response DTO para uma movimentação de estoque.
/// </summary>
public class MovimentacaoDto
{
    /// <summary>
    /// Identificador público da movimentação.
    /// </summary>
    public Guid PublicId { get; set; }

    /// <summary>
    /// Tipo de movimentação.
    /// </summary>
    public string Tipo { get; set; } = null!;

    /// <summary>
    /// Quantidade movimentada.
    /// </summary>
    public int Quantidade { get; set; }

    /// <summary>
    /// Descrição da movimentação.
    /// </summary>
    public string? Descricao { get; set; }

    /// <summary>
    /// Data e hora da movimentação.
    /// </summary>
    public DateTime DataHora { get; set; }

    /// <summary>
    /// Saldo antes da movimentação.
    /// </summary>
    public int SaldoAntes { get; set; }

    /// <summary>
    /// Saldo depois da movimentação.
    /// </summary>
    public int SaldoDepois { get; set; }
}
