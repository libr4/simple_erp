namespace Backend.Models;

/// <summary>
/// Representa um produto no estoque.
/// </summary>
public class Produto
{
    /// <summary>
    /// Código único do produto (primary key).
    /// </summary>
    public long Codigo { get; set; }

    /// <summary>
    /// Descrição do produto.
    /// </summary>
    public string Descricao { get; set; } = null!;

    /// <summary>
    /// Quantidade atual em estoque.
    /// </summary>
    public int Estoque { get; set; }

    /// <summary>
    /// Token de concorrência otimista (RowVersion).
    /// </summary>
    public byte[] RowVersion { get; set; } = null!;

    /// <summary>
    /// Coleção de movimentações de estoque associadas ao produto.
    /// </summary>
    public ICollection<MovimentacaoEstoque> Movimentacoes { get; set; } = new List<MovimentacaoEstoque>();
}
