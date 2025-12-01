using Backend.DTOs;
using Backend.Exceptions;
using Backend.Infrastructure;
using Backend.Models;
using Backend.Repositories;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Backend.Services;

/// <summary>
/// Interface do serviço de movimentação de estoque.
/// </summary>
public interface IMovimentacaoService
{
    /// <summary>
    /// Processa uma movimentação de estoque de forma atômica.
    /// Atualiza o produto, insere a movimentação e retorna o produto com últimas 10 movimentações.
    /// </summary>
    Task<MovimentacaoResultDto> ProcessMovementAsync(
        MovimentacaoCreateRequest request,
        CancellationToken ct = default);
}

/// <summary>
/// Implementação do serviço de movimentação de estoque.
/// Orquestra os serviços de validação e cálculo, com suporte a transações e concorrência otimista.
/// </summary>
public class MovimentacaoService : IMovimentacaoService
{
    private readonly ApplicationDbContext _context;
    private readonly IProdutoRepository _produtoRepository;
    private readonly IMovimentacaoRepository _movimentacaoRepository;
    private readonly IMovimentacaoParser _parser;
    private readonly IMovimentacaoCalculationService _calculationService;
    private readonly IMapper _mapper;
    private readonly ILogger<MovimentacaoService> _logger;

    public MovimentacaoService(
        ApplicationDbContext context,
        IProdutoRepository produtoRepository,
        IMovimentacaoRepository movimentacaoRepository,
        IMovimentacaoParser parser,
        IMovimentacaoCalculationService calculationService,
        IMapper mapper,
        ILogger<MovimentacaoService> logger)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _produtoRepository = produtoRepository ?? throw new ArgumentNullException(nameof(produtoRepository));
        _movimentacaoRepository = movimentacaoRepository ?? throw new ArgumentNullException(nameof(movimentacaoRepository));
        _parser = parser ?? throw new ArgumentNullException(nameof(parser));
        _calculationService = calculationService ?? throw new ArgumentNullException(nameof(calculationService));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<MovimentacaoResultDto> ProcessMovementAsync(
        MovimentacaoCreateRequest request,
        CancellationToken ct = default)
    {
        _logger.LogInformation(
            "Processando movimentação: Código={Codigo}, Tipo={Tipo}, Quantidade={Quantidade}",
            request.CodigoProduto, request.Tipo, request.Quantidade);

        // 1. Parse e normaliza o tipo de movimentação (sem acesso ao BD)
        var tipo = _parser.Parse(request.Tipo);

        // Inicia uma transação para atomicidade
        using var transaction = await _context.Database.BeginTransactionAsync(ct);

        try
        {
            // 2. Buscar o produto
            var produto = await _produtoRepository.GetByCodigo(request.CodigoProduto, ct);
            if (produto == null)
            {
                _logger.LogWarning("Produto não encontrado: {Codigo}", request.CodigoProduto);
                throw new ProductNotFoundException(request.CodigoProduto);
            }

            // 3. Calcular novo estoque (lógica de negócio pura)
            int saldoAntes = produto.Estoque;
            int saldoDepois = _calculationService.CalculateNewStock(
                tipo, saldoAntes, request.Quantidade, request.CodigoProduto);

            // 4. Criar registro de movimentação
            var movimentacao = new MovimentacaoEstoque
            {
                CodigoProduto = request.CodigoProduto,
                Tipo = tipo,
                Quantidade = request.Quantidade,
                Descricao = request.Descricao,
                DataHora = DateTime.UtcNow,
                SaldoAntes = saldoAntes,
                SaldoDepois = saldoDepois,
                PublicId = Guid.NewGuid()
            };

            // 5. Atualizar estoque do produto
            produto.Estoque = saldoDepois;

            // 6. Persistir (com concorrência otimista via RowVersion)
            await _movimentacaoRepository.AddAsync(movimentacao, ct);
            await _produtoRepository.UpdateAsync(produto, ct);

            // Commit da transação
            await transaction.CommitAsync(ct);

            _logger.LogInformation(
                "Movimentação processada com sucesso: PublicId={PublicId}, Código={Codigo}",
                movimentacao.PublicId, request.CodigoProduto);

            // 7. Buscar as últimas 10 movimentações
            var movimentacoesRecentes = await _movimentacaoRepository
                .GetRecentMovimentacoes(request.CodigoProduto, limit: 10, ct);

            // 8. Mapear e retornar result DTO
            var produtoDto = _mapper.Map<ProdutoDto>(produto);
            var movimentacoesDto = _mapper.Map<List<MovimentacaoDto>>(movimentacoesRecentes);

            return new MovimentacaoResultDto
            {
                Produto = produtoDto,
                MovimentacoesRecentes = movimentacoesDto
            };
        }
        catch (DbUpdateConcurrencyException ex)
        {
            await transaction.RollbackAsync(ct);
            _logger.LogError(ex, "Conflito de concorrência ao processar movimentação do produto {Codigo}",
                request.CodigoProduto);
            throw new ConcurrencyException(request.CodigoProduto);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(ct);
            _logger.LogError(ex, "Erro ao processar movimentação: Código={Codigo}", request.CodigoProduto);
            throw;
        }
    }
}
