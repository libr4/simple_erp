using Backend.Infrastructure;
using Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace Backend.Repositories;

/// <summary>
/// Implementação do repositório Produto com EF Core.
/// </summary>
public class ProdutoRepository : IProdutoRepository
{
    private readonly ApplicationDbContext _context;

    public ProdutoRepository(ApplicationDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<Produto?> GetByCodigo(long codigo, CancellationToken ct = default)
    {
        return await _context.Produtos
            .AsTracking()
            .FirstOrDefaultAsync(p => p.Codigo == codigo, ct);
    }

    public async Task UpdateAsync(Produto produto, CancellationToken ct = default)
    {
        _context.Produtos.Update(produto);
        await _context.SaveChangesAsync(ct);
    }

    public async Task<IEnumerable<Produto>> GetAllAsync(CancellationToken ct = default)
    {
        return await _context.Produtos
            .AsNoTracking()
            .ToListAsync(ct);
    }
}
