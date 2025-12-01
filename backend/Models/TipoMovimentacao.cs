namespace Backend.Models;

/// <summary>
/// Enumeração dos tipos de movimentação de estoque.
/// </summary>
public enum TipoMovimentacao
{
    /// <summary>Entrada de estoque (compra, devolução, etc.)</summary>
    Entrada,

    /// <summary>Saída de estoque (venda, descarte, etc.)</summary>
    Saida,

    /// <summary>Inventário (ajuste de estoque)</summary>
    Inventario
}
