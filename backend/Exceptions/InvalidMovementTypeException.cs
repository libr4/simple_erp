namespace Backend.Exceptions;

/// <summary>
/// Exception lançada quando o tipo de movimentação é inválido.
/// </summary>
public class InvalidMovementTypeException : Exception
{
    public InvalidMovementTypeException(string tipoInvalido)
        : base($"Tipo de movimentação inválido: '{tipoInvalido}'. " +
               $"Valores permitidos: ENTRADA, SAIDA, INVENTARIO.")
    {
        TipoInvalido = tipoInvalido;
    }

    public string TipoInvalido { get; }
}
