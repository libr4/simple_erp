using Backend.Models;

namespace Backend.Repositories;

/// <summary>
/// Interface do repositório para a entidade MovimentacaoEstoque.
/// </summary>
public interface IMovimentacaoRepository
{
    /// <summary>
    /// Adiciona uma nova movimentação.
    /// </summary>
    Task AddAsync(MovimentacaoEstoque movimentacao, CancellationToken ct = default);

    /// <summary>
    /// Obtém as últimas N movimentações de um produto, ordenadas por data descrescente.
    /// </summary>
    Task<IEnumerable<MovimentacaoEstoque>> GetRecentMovimentacoes(long codigoProduto, int limit = 10, CancellationToken ct = default);

    /// <summary>
    /// Obtém todas as movimentações de um produto.
    /// </summary>
    Task<IEnumerable<MovimentacaoEstoque>> GetByCodigoProduto(long codigoProduto, CancellationToken ct = default);
}
