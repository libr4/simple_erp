using Backend.Exceptions;
using Backend.Models;
using Microsoft.Extensions.Logging;

namespace Backend.Services;

/// <summary>
/// Interface para cálculo de estoque durante movimentações.
/// Responsável apenas pela lógica de negócio de cálculo.
/// </summary>
public interface IMovimentacaoCalculationService
{
    /// <summary>
    /// Calcula o novo saldo de estoque baseado no tipo de movimentação.
    /// </summary>
    /// <param name="tipo">Tipo de movimentação (ENTRADA, SAIDA, INVENTARIO)</param>
    /// <param name="saldoAtual">Saldo atual do produto</param>
    /// <param name="quantidade">Quantidade a movimentar</param>
    /// <param name="codigoProduto">Código do produto (para logs e exceções)</param>
    /// <returns>Novo saldo após a movimentação</returns>
    /// <exception cref="InsufficientStockException">Se SAIDA e estoque insuficiente</exception>
    int CalculateNewStock(TipoMovimentacao tipo, int saldoAtual, int quantidade, long codigoProduto);
}

/// <summary>
/// Implementação do serviço de cálculo de estoque.
/// Encapsula a lógica de cálculo para cada tipo de movimentação.
/// </summary>
public class MovimentacaoCalculationService : IMovimentacaoCalculationService
{
    private readonly ILogger<MovimentacaoCalculationService> _logger;

    public MovimentacaoCalculationService(ILogger<MovimentacaoCalculationService> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public int CalculateNewStock(TipoMovimentacao tipo, int saldoAtual, int quantidade, long codigoProduto)
    {
        int novoSaldo;

        switch (tipo)
        {
            case TipoMovimentacao.Entrada:
                novoSaldo = saldoAtual + quantidade;
                _logger.LogInformation(
                    "Movimentação ENTRADA: Produto={Codigo}, Antes={Antes}, Depois={Depois}",
                    codigoProduto, saldoAtual, novoSaldo);
                break;

            case TipoMovimentacao.Saida:
                if (quantidade > saldoAtual)
                {
                    _logger.LogWarning(
                        "Estoque insuficiente: Produto={Codigo}, Disponível={Disponível}, Solicitado={Solicitado}",
                        codigoProduto, saldoAtual, quantidade);
                    throw new InsufficientStockException(codigoProduto, saldoAtual, quantidade);
                }
                novoSaldo = saldoAtual - quantidade;
                _logger.LogInformation(
                    "Movimentação SAIDA: Produto={Codigo}, Antes={Antes}, Depois={Depois}",
                    codigoProduto, saldoAtual, novoSaldo);
                break;

            case TipoMovimentacao.Inventario:
                novoSaldo = quantidade;
                _logger.LogInformation(
                    "Movimentação INVENTARIO: Produto={Codigo}, Antes={Antes}, Depois={Depois}",
                    codigoProduto, saldoAtual, novoSaldo);
                break;

            default:
                throw new InvalidMovementTypeException(tipo.ToString());
        }

        return novoSaldo;
    }
}
