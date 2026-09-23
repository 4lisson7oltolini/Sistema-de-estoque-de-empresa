using Microsoft.AspNetCore.Mvc;
using PdvSistema.Domain.Entities;
using PdvSistema.Domain.Interfaces;

namespace PdvSistema.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProdutoController : ControllerBase
    {
        private readonly IProdutoRepository _repository;

        public ProdutoController(IProdutoRepository repository) => _repository = repository;

        [HttpGet]
        public async Task<IActionResult> Listar() =>
            Ok(await _repository.ListarTodosAsync());

        [HttpGet("{id}")]
        public async Task<IActionResult> ObterPorId(int id)
        {
            var produto = await _repository.ObterPorIdAsync(id);
            return produto is null ? NotFound() : Ok(produto);
        }

        [HttpPost]
        public async Task<IActionResult> Criar(Produto produto)
        {
            await _repository.AdicionarAsync(produto);
            return CreatedAtAction(nameof(ObterPorId), new { id = produto.Id }, produto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Atualizar(int id, Produto produto)
        {
            if (id != produto.Id) return BadRequest();
            await _repository.AtualizarAsync(produto);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Remover(int id)
        {
            await _repository.RemoverAsync(id);
            return NoContent();
        }
    }
}