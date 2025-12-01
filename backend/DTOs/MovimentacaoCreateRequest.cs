namespace Backend.DTOs;

/// <summary>
/// Request DTO para criar uma movimentação de estoque.
/// </summary>
public class MovimentacaoCreateRequest
{
    /// <summary>
    /// Código do produto.
    /// </summary>
    public long CodigoProduto { get; set; }

    /// <summary>
    /// Tipo de movimentação (ENTRADA, SAIDA, INVENTARIO).
    /// </summary>
    public string Tipo { get; set; } = null!;

    /// <summary>
    /// Quantidade da movimentação.
    /// </summary>
    public int Quantidade { get; set; }

    /// <summary>
    /// Descrição da movimentação (opcional).
    /// </summary>
    public string? Descricao { get; set; }
}
