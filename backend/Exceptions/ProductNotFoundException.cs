namespace Backend.Exceptions;

/// <summary>
/// Exception lançada quando um produto não é encontrado.
/// </summary>
public class ProductNotFoundException : Exception
{
    public ProductNotFoundException(long codigoProduto)
        : base($"Produto com código {codigoProduto} não encontrado.")
    {
        CodigoProduto = codigoProduto;
    }

    public long CodigoProduto { get; }
}
