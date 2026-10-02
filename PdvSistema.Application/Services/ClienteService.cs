using PdvSistema.Application.DTOs.Cliente;
using PdvSistema.Application.Interfaces;
using PdvSistema.Domain.Entities;
using PdvSistema.Domain.Interfaces;

namespace PdvSistema.Application.Services;

public class ClienteService : IClienteService
{
    private readonly IClienteRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public ClienteService(IClienteRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<List<ClienteResponse>> ListarAsync() =>
        (await _repository.ObterTodosAsync()).Select(Mapear).ToList();

    public async Task<ClienteResponse?> ObterPorIdAsync(int id)
    {
        var cliente = await _repository.ObterPorIdAsync(id);
        return cliente is null ? null : Mapear(cliente);
    }

    public async Task<ClienteResponse> CriarAsync(CriarClienteRequest request)
    {
        var cliente = new Cliente(
            ValidarNome(request.Nome),
            NormalizarTexto(request.Documento),
            NormalizarTexto(request.Telefone),
            NormalizarTexto(request.Email));

        await _repository.AdicionarAsync(cliente);
        await _unitOfWork.SaveChangesAsync();
        return Mapear(cliente);
    }

    public async Task<bool> AtualizarAsync(int id, CriarClienteRequest request)
    {
        var cliente = await _repository.ObterPorIdAsync(id);
        if (cliente is null)
            return false;

        cliente.AtualizarDados(
            ValidarNome(request.Nome),
            NormalizarTexto(request.Telefone),
            NormalizarTexto(request.Email));

        cliente.Documento = NormalizarTexto(request.Documento);
        _repository.Atualizar(cliente);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }

    public async Task<bool> RemoverAsync(int id)
    {
        var cliente = await _repository.ObterPorIdAsync(id);
        if (cliente is null)
            return false;

        _repository.Remover(cliente);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }

    private static string ValidarNome(string nome)
    {
        var valor = nome?.Trim();
        if (string.IsNullOrWhiteSpace(valor))
            throw new ArgumentException("O nome do cliente é obrigatório.");

        return valor;
    }

    private static string? NormalizarTexto(string? valor) =>
        string.IsNullOrWhiteSpace(valor) ? null : valor.Trim();

    private static ClienteResponse Mapear(Cliente cliente) => new()
    {
        Id = cliente.Id,
        Nome = cliente.Nome,
        Documento = cliente.Documento,
        Telefone = cliente.Telefone,
        Email = cliente.Email,
        DataCadastro = cliente.DataCadastro
    };
}
