namespace Backend.Models;

/// <summary>
/// Representa uma movimentação de estoque de um produto.
/// </summary>
public class MovimentacaoEstoque
{
    /// <summary>
    /// Identificador interno único da movimentação (identity).
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// Identificador público único da movimentação (GUID, exposto na API).
    /// </summary>
    public Guid PublicId { get; set; } = Guid.NewGuid();

    /// <summary>
    /// Código do produto movimentado (foreign key).
    /// </summary>
    public long CodigoProduto { get; set; }

    /// <summary>
    /// Referência navegacional para o produto.
    /// </summary>
    public Produto Produto { get; set; } = null!;

    /// <summary>
    /// Tipo de movimentação (ENTRADA, SAIDA, INVENTARIO).
    /// </summary>
    public TipoMovimentacao Tipo { get; set; }

    /// <summary>
    /// Quantidade movimentada.
    /// </summary>
    public int Quantidade { get; set; }

    /// <summary>
    /// Descrição opcional da movimentação.
    /// </summary>
    public string? Descricao { get; set; }

    /// <summary>
    /// Data e hora da movimentação em UTC.
    /// </summary>
    public DateTime DataHora { get; set; }

    /// <summary>
    /// Saldo do produto ANTES da movimentação.
    /// </summary>
    public int SaldoAntes { get; set; }

    /// <summary>
    /// Saldo do produto DEPOIS da movimentação.
    /// </summary>
    public int SaldoDepois { get; set; }
}
