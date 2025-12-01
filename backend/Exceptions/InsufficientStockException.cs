namespace Backend.Exceptions;

/// <summary>
/// Exception lançada quando há estoque insuficiente para uma saída.
/// </summary>
public class InsufficientStockException : Exception
{
    public InsufficientStockException(long codigoProduto, int quantidadeDisponivel, int quantidadeSolicitada)
        : base($"Estoque insuficiente para produto {codigoProduto}. " +
               $"Disponível: {quantidadeDisponivel}, Solicitado: {quantidadeSolicitada}.")
    {
        CodigoProduto = codigoProduto;
        QuantidadeDisponivel = quantidadeDisponivel;
        QuantidadeSolicitada = quantidadeSolicitada;
    }

    public long CodigoProduto { get; }
    public int QuantidadeDisponivel { get; }
    public int QuantidadeSolicitada { get; }
}
