using Microsoft.AspNetCore.Mvc;
using PdvSistema.Application.DTOs.Usuario;
using PdvSistema.Application.Interfaces;

namespace PdvSistema.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsuarioController : ControllerBase
{
    private readonly IUsuarioService _service;

    public UsuarioController(IUsuarioService service) => _service = service;

    [HttpGet]
    public async Task<ActionResult<List<UsuarioResponse>>> Listar() =>
        Ok(await _service.ListarAsync());

    [HttpGet("{id:int}")]
    public async Task<ActionResult<UsuarioResponse>> ObterPorId(int id)
    {
        var usuario = await _service.ObterPorIdAsync(id);
        return usuario is null ? NotFound() : Ok(usuario);
    }

    [HttpPost]
    public async Task<IActionResult> Criar(CriarUsuarioRequest request)
    {
        try
        {
            var usuario = await _service.CriarAsync(request);
            return CreatedAtAction(nameof(ObterPorId), new { id = usuario.Id }, usuario);
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

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Atualizar(int id, CriarUsuarioRequest request)
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
