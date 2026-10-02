using Microsoft.AspNetCore.Mvc;
using PdvSistema.Application.DTOs.Produto;
using PdvSistema.Application.Interfaces;

namespace PdvSistema.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProdutoController : ControllerBase
{
    private readonly IProdutoService _produtoService;

    public ProdutoController(IProdutoService produtoService)
    {
        _produtoService = produtoService;
    }

    [HttpGet]
    public async Task<IActionResult> Listar()
    {
        var produtos = await _produtoService.ListarAsync();
        return Ok(produtos);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> ObterPorId(int id)
    {
        var produto = await _produtoService.ObterPorIdAsync(id);

        if (produto is null)
        {
            return NotFound();
        }

        return Ok(produto);
    }

    [HttpPost]
    public async Task<IActionResult> Criar(CriarProdutoRequest request)
    {
        try
        {
            var produto = await _produtoService.CriarAsync(request);
            return CreatedAtAction(nameof(ObterPorId), new { id = produto.Id }, produto);
        }
        catch (ArgumentException exception)
        {
            return BadRequest(new { message = exception.Message });
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Atualizar(
        int id,
        AtualizarProdutoRequest request)
    {
        try
        {
            var atualizado = await _produtoService.AtualizarAsync(id, request);
            return atualizado ? NoContent() : NotFound();
        }
        catch (ArgumentException exception)
        {
            return BadRequest(new { message = exception.Message });
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Remover(int id)
    {
        var removido = await _produtoService.RemoverAsync(id);
        return removido ? NoContent() : NotFound();
    }
}