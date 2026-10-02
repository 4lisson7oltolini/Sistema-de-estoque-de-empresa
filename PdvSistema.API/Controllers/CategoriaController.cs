using Microsoft.AspNetCore.Mvc;
using PdvSistema.Application.DTOs.Categoria;
using PdvSistema.Application.Interfaces;

namespace PdvSistema.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoriaController : ControllerBase
{
    private readonly ICategoriaService _service;

    public CategoriaController(ICategoriaService service) => _service = service;

    [HttpGet]
    public async Task<ActionResult<List<CategoriaResponse>>> Listar() =>
        Ok(await _service.ListarAsync());

    [HttpGet("{id:int}")]
    public async Task<ActionResult<CategoriaResponse>> ObterPorId(int id)
    {
        var categoria = await _service.ObterPorIdAsync(id);
        return categoria is null ? NotFound() : Ok(categoria);
    }

    [HttpPost]
    public async Task<IActionResult> Criar(CriarCategoriaRequest request)
    {
        try
        {
            var categoria = await _service.CriarAsync(request);
            return CreatedAtAction(nameof(ObterPorId), new { id = categoria.Id }, categoria);
        }
        catch (ArgumentException exception)
        {
            return BadRequest(new { message = exception.Message });
        }
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Atualizar(int id, CriarCategoriaRequest request)
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
        return await _service.RemoverAsync(id) switch
        {
            ResultadoRemocaoCategoria.Removida => NoContent(),
            ResultadoRemocaoCategoria.NaoEncontrada => NotFound(),
            ResultadoRemocaoCategoria.PossuiProdutos => Conflict(new
            {
                message = "A categoria possui produtos associados e não pode ser removida."
            }),
            _ => StatusCode(StatusCodes.Status500InternalServerError)
        };
    }
}