namespace Backend.DTOs;

/// <summary>
/// Response DTO para o resultado de uma movimentação.
/// Contém o produto atualizado e as últimas 10 movimentações.
/// </summary>
public class MovimentacaoResultDto
{
    /// <summary>
    /// Dados do produto após a movimentação.
    /// </summary>
    public ProdutoDto Produto { get; set; } = null!;

    /// <summary>
    /// Últimas 10 movimentações do produto (ordenadas por data decrescente).
    /// </summary>
    public List<MovimentacaoDto> MovimentacoesRecentes { get; set; } = new();
}
