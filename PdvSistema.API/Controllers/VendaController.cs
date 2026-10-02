using Microsoft.AspNetCore.Mvc;
using PdvSistema.Application.DTOs.Venda;
using PdvSistema.Application.Interfaces;

namespace PdvSistema.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class VendaController : ControllerBase
{
    private readonly IVendaService _service;

    public VendaController(IVendaService service) => _service = service;

    [HttpGet]
    public async Task<ActionResult<List<VendaResponse>>> Listar() =>
        Ok(await _service.ListarAsync());

    [HttpGet("{id:int}")]
    public async Task<ActionResult<VendaResponse>> ObterPorId(int id)
    {
        var venda = await _service.ObterPorIdAsync(id);
        return venda is null ? NotFound() : Ok(venda);
    }

    [HttpPost]
    public async Task<IActionResult> Criar(CriarVendaRequest request)
    {
        try
        {
            var venda = await _service.CriarAsync(request);
            return CreatedAtAction(nameof(ObterPorId), new { id = venda.Id }, venda);
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

    [HttpPut("{id:int}/cancelar")]
    public async Task<IActionResult> Cancelar(int id)
    {
        var cancelada = await _service.CancelarAsync(id);
        return cancelada ? NoContent() : NotFound();
    }
}
