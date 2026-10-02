using PdvSistema.Application.DTOs.Categoria;
using PdvSistema.Application.Interfaces;
using PdvSistema.Domain.Entities;
using PdvSistema.Domain.Interfaces;

namespace PdvSistema.Application.Services;

public class CategoriaService : ICategoriaService
{
    private readonly ICategoriaRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public CategoriaService(ICategoriaRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<List<CategoriaResponse>> ListarAsync() =>
        (await _repository.ObterTodosAsync()).Select(Mapear).ToList();

    public async Task<CategoriaResponse?> ObterPorIdAsync(int id)
    {
        var categoria = await _repository.ObterPorIdAsync(id);
        return categoria is null ? null : Mapear(categoria);
    }

    public async Task<CategoriaResponse> CriarAsync(CriarCategoriaRequest request)
    {
        var categoria = new Categoria { Nome = ValidarNome(request.Nome) };
        await _repository.AdicionarAsync(categoria);
        await _unitOfWork.SaveChangesAsync();
        return Mapear(categoria);
    }

    public async Task<bool> AtualizarAsync(int id, CriarCategoriaRequest request)
    {
        var categoria = await _repository.ObterPorIdAsync(id);
        if (categoria is null)
            return false;

        categoria.Nome = ValidarNome(request.Nome);
        _repository.Atualizar(categoria);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }

    public async Task<ResultadoRemocaoCategoria> RemoverAsync(int id)
    {
        var categoria = await _repository.ObterPorIdAsync(id);
        if (categoria is null)
            return ResultadoRemocaoCategoria.NaoEncontrada;

        if (await _repository.PossuiProdutosAsync(id))
            return ResultadoRemocaoCategoria.PossuiProdutos;

        _repository.Remover(categoria);
        await _unitOfWork.SaveChangesAsync();
        return ResultadoRemocaoCategoria.Removida;
    }

    private static string ValidarNome(string nome)
    {
        var nomeNormalizado = nome?.Trim();
        if (string.IsNullOrWhiteSpace(nomeNormalizado))
            throw new ArgumentException("O nome da categoria é obrigatório.");
        if (nomeNormalizado.Length > 100)
            throw new ArgumentException("O nome da categoria deve ter até 100 caracteres.");

        return nomeNormalizado;
    }

    private static CategoriaResponse Mapear(Categoria categoria) => new()
    {
        Id = categoria.Id,
        Nome = categoria.Nome
    };
}