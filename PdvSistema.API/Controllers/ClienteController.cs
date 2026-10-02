using Microsoft.AspNetCore.Mvc;
using PdvSistema.Application.DTOs.Cliente;
using PdvSistema.Application.Interfaces;

namespace PdvSistema.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ClienteController : ControllerBase
{
    private readonly IClienteService _service;

    public ClienteController(IClienteService service) => _service = service;

    [HttpGet]
    public async Task<ActionResult<List<ClienteResponse>>> Listar() =>
        Ok(await _service.ListarAsync());

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ClienteResponse>> ObterPorId(int id)
    {
        var cliente = await _service.ObterPorIdAsync(id);
        return cliente is null ? NotFound() : Ok(cliente);
    }

    [HttpPost]
    public async Task<IActionResult> Criar(CriarClienteRequest request)
    {
        try
        {
            var cliente = await _service.CriarAsync(request);
            return CreatedAtAction(nameof(ObterPorId), new { id = cliente.Id }, cliente);
        }
        catch (ArgumentException exception)
        {
            return BadRequest(new { message = exception.Message });
        }
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Atualizar(int id, CriarClienteRequest request)
    {
        try
        {
            return await _service.AtualizarAsync(id, request) ? NoContent() : NotFound();
        }
        catch (ArgumentException exception)
        {
            return BadRequest(new { message = exception.Message });
        }
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Remover(int id)
    {
        var removido = await _service.RemoverAsync(id);
        return removido ? NoContent() : NotFound();
    }
}
