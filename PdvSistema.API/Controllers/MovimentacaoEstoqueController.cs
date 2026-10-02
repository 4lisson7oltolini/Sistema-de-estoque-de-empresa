using Microsoft.AspNetCore.Mvc;
using PdvSistema.Application.DTOs.MovimentacaoEstoque;
using PdvSistema.Application.Interfaces;

namespace PdvSistema.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MovimentacaoEstoqueController : ControllerBase
{
    private readonly IMovimentacaoEstoqueService _service;

    public MovimentacaoEstoqueController(IMovimentacaoEstoqueService service) => _service = service;

    [HttpGet]
    public async Task<ActionResult<List<MovimentacaoEstoqueResponse>>> Listar(
        [FromQuery] int? produtoId) =>
        Ok(await _service.ListarAsync(produtoId));

    [HttpGet("{id:int}")]
    public async Task<ActionResult<MovimentacaoEstoqueResponse>> ObterPorId(int id)
    {
        var movimentacao = await _service.ObterPorIdAsync(id);
        return movimentacao is null ? NotFound() : Ok(movimentacao);
    }

    [HttpPost]
    public async Task<IActionResult> Registrar(RegistrarMovimentacaoRequest request)
    {
        try
        {
            var movimentacao = await _service.RegistrarAsync(request);
            if (movimentacao is null)
                return NotFound(new { message = "Produto não encontrado." });

            return CreatedAtAction(
                nameof(ObterPorId),
                new { id = movimentacao.Id },
                movimentacao);
        }
        catch (ArgumentException exception)
        {
            return BadRequest(new { message = exception.Message });
        }
        catch (InvalidOperationException exception)
        {
            return Conflict(new { message = exception.Message });
        }
    }
}