using Backend.Infrastructure;
using Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace Backend.Repositories;

/// <summary>
/// Implementação do repositório MovimentacaoEstoque com EF Core.
/// </summary>
public class MovimentacaoRepository : IMovimentacaoRepository
{
    private readonly ApplicationDbContext _context;

    public MovimentacaoRepository(ApplicationDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task AddAsync(MovimentacaoEstoque movimentacao, CancellationToken ct = default)
    {
        await _context.Movimentacoes.AddAsync(movimentacao, ct);
    }

    public async Task<IEnumerable<MovimentacaoEstoque>> GetRecentMovimentacoes(
        long codigoProduto, 
        int limit = 10, 
        CancellationToken ct = default)
    {
        return await _context.Movimentacoes
            .AsNoTracking()
            .Where(m => m.CodigoProduto == codigoProduto)
            .OrderByDescending(m => m.DataHora)
            .Take(limit)
            .ToListAsync(ct);
    }

    public async Task<IEnumerable<MovimentacaoEstoque>> GetByCodigoProduto(
        long codigoProduto, 
        CancellationToken ct = default)
    {
        return await _context.Movimentacoes
            .AsNoTracking()
            .Where(m => m.CodigoProduto == codigoProduto)
            .OrderByDescending(m => m.DataHora)
            .ToListAsync(ct);
    }
}
