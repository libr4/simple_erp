using Backend.Models;

namespace Backend.Repositories;

/// <summary>
/// Interface do repositório para a entidade Produto.
/// </summary>
public interface IProdutoRepository
{
    /// <summary>
    /// Obtém um produto pelo código.
    /// </summary>
    Task<Produto?> GetByCodigo(long codigo, CancellationToken ct = default);

    /// <summary>
    /// Atualiza um produto existente.
    /// </summary>
    Task UpdateAsync(Produto produto, CancellationToken ct = default);

    /// <summary>
    /// Obtém todos os produtos.
    /// </summary>
    Task<IEnumerable<Produto>> GetAllAsync(CancellationToken ct = default);
}
