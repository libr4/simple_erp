using Microsoft.AspNetCore.Mvc;
using Backend.Services;
using Backend.DTOs;
using Microsoft.Extensions.Logging;

namespace Backend.Controllers;

/// <summary>
/// Controller para gerenciar movimentações de estoque.
/// </summary>
[ApiController]
[Route("api/v1/estoque")]
public class EstoqueController : ControllerBase
{
    private readonly IMovimentacaoService _movimentacaoService;

    public EstoqueController(
        IMovimentacaoService movimentacaoService)
    {
        _movimentacaoService = movimentacaoService ?? throw new ArgumentNullException(nameof(movimentacaoService));
    }

    /// <summary>
    /// Registra uma movimentação de estoque (ENTRADA, SAIDA ou INVENTARIO).
    /// Retorna o produto atualizado e as últimas 10 movimentações.
    /// </summary>
    /// <param name="request">Request contendo código do produto, tipo, quantidade e descrição.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>MovimentacaoResultDto com produto e últimas movimentações.</returns>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<MovimentacaoResultDto>> PostMovimentacao(
        [FromBody] MovimentacaoCreateRequest request,
        CancellationToken ct)
    {
        var result = await _movimentacaoService.ProcessMovementAsync(request, ct);
        return CreatedAtAction(nameof(PostMovimentacao), result);
    }
}
