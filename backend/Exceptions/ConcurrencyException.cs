namespace Backend.Exceptions;

/// <summary>
/// Exception lançada quando há um conflito de concorrência na atualização.
/// </summary>
public class ConcurrencyException : Exception
{
    public ConcurrencyException(long codigoProduto)
        : base($"Conflito de concorrência ao atualizar o produto {codigoProduto}. " +
               $"O produto foi modificado por outro processo. Por favor, tente novamente.")
    {
        CodigoProduto = codigoProduto;
    }

    public long CodigoProduto { get; }
}
