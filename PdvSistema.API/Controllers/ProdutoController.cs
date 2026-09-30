using Microsoft.AspNetCore.Mvc;
using PdvSistema.Domain.Entities;
using PdvSistema.Domain.Interfaces;

namespace PdvSistema.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProdutoController : ControllerBase
{
    private readonly IProdutoRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public ProdutoController(
        IProdutoRepository repository,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    [HttpGet]
    public async Task<IActionResult> Listar()
    {
        var produtos = await _repository.ObterTodosAsync();

        return Ok(produtos);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> ObterPorId(int id)
    {
        var produto = await _repository.ObterPorIdAsync(id);

        if (produto is null)
        {
            return NotFound();
        }

        return Ok(produto);
    }

    [HttpPost]
    public async Task<IActionResult> Criar(Produto produto)
    {
        await _repository.AdicionarAsync(produto);

        await _unitOfWork.SaveChangesAsync();

        return CreatedAtAction(
            nameof(ObterPorId),
            new { id = produto.Id },
            produto
        );
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Atualizar(
        int id,
        Produto produto)
    {
        if (id != produto.Id)
        {
            return BadRequest();
        }

        var produtoExistente =
            await _repository.ObterPorIdAsync(id);

        if (produtoExistente is null)
        {
            return NotFound();
        }

        _repository.Atualizar(produto);

        await _unitOfWork.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Remover(int id)
    {
        var produto =
            await _repository.ObterPorIdAsync(id);

        if (produto is null)
        {
            return NotFound();
        }

        _repository.Remover(produto);

        await _unitOfWork.SaveChangesAsync();

        return NoContent();
    }
}