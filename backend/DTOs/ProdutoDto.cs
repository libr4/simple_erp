namespace Backend.DTOs;

/// <summary>
/// Response DTO para um produto.
/// </summary>
public class ProdutoDto
{
    /// <summary>
    /// Código do produto.
    /// </summary>
    public long Codigo { get; set; }

    /// <summary>
    /// Descrição do produto.
    /// </summary>
    public string Descricao { get; set; } = null!;

    /// <summary>
    /// Quantidade em estoque.
    /// </summary>
    public int Estoque { get; set; }
}
