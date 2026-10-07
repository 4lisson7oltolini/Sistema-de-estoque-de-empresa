using System.Net.Http.Json;
using PdvSistema.Web.Models;

namespace PdvSistema.Web.Services;

public sealed class EstoqueApiClient(HttpClient http, PdvAuthenticationStateProvider authState)
{
    public async Task<List<ProdutoDto>> ListarProdutosAsync() =>
        await ObterAsync<List<ProdutoDto>>("api/Produto") ?? [];

    public async Task<List<CategoriaDto>> ListarCategoriasAsync() =>
        await ObterAsync<List<CategoriaDto>>("api/Categoria") ?? [];

    public async Task CriarProdutoAsync(ProdutoFormModel model) =>
        await EnviarSemRespostaAsync(HttpMethod.Post, "api/Produto", model);

    public async Task AtualizarProdutoAsync(ProdutoFormModel model) =>
        await EnviarSemRespostaAsync(HttpMethod.Put, $"api/Produto/{model.Id}", model);

    public async Task RemoverProdutoAsync(int id) =>
        await EnviarSemRespostaAsync(HttpMethod.Delete, $"api/Produto/{id}");

    public async Task CriarCategoriaAsync(CategoriaFormModel model) =>
        await EnviarSemRespostaAsync(HttpMethod.Post, "api/Categoria", model);

    public async Task AtualizarCategoriaAsync(int id, CategoriaFormModel model) =>
        await EnviarSemRespostaAsync(HttpMethod.Put, $"api/Categoria/{id}", model);

    public async Task RemoverCategoriaAsync(int id) =>
        await EnviarSemRespostaAsync(HttpMethod.Delete, $"api/Categoria/{id}");

    public Task<LoginResultDto> LoginAsync(LoginFormModel model) =>
        EnviarComRespostaAsync<LoginResultDto>(HttpMethod.Post, "api/Auth/login", model);

    public Task ConfigurarAdministradorAsync(BootstrapFormModel model) =>
        EnviarSemRespostaAsync(HttpMethod.Post, "api/Auth/bootstrap", model);

    private async Task<T?> ObterAsync<T>(string caminho)
    {
        using var response = await EnviarAsync(HttpMethod.Get, caminho);
        return await response.Content.ReadFromJsonAsync<T>();
    }

    private async Task EnviarSemRespostaAsync(HttpMethod metodo, string caminho, object? corpo = null)
    {
        using var response = await EnviarAsync(metodo, caminho, corpo);
    }

    private async Task<T> EnviarComRespostaAsync<T>(HttpMethod metodo, string caminho, object corpo)
    {
        using var response = await EnviarAsync(metodo, caminho, corpo);
        return await response.Content.ReadFromJsonAsync<T>()
            ?? throw new InvalidOperationException("A API retornou uma resposta vazia.");
    }

    private async Task<HttpResponseMessage> EnviarAsync(HttpMethod metodo, string caminho, object? corpo = null)
    {
        using var request = new HttpRequestMessage(metodo, caminho);
        if (corpo is not null)
            request.Content = JsonContent.Create(corpo);

        var token = await authState.ObterTokenAsync();
        if (!string.IsNullOrWhiteSpace(token))
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        var response = await http.SendAsync(request);
        if (response.IsSuccessStatusCode)
            return response;

        var erro = await response.Content.ReadFromJsonAsync<ApiError>();
        var mensagem = erro?.ObterMensagem() ?? $"A API retornou o erro {(int)response.StatusCode}.";
        var statusCode = response.StatusCode;
        response.Dispose();
        throw new HttpRequestException(mensagem, null, statusCode);
    }

    private sealed class ApiError
    {
        public string? Title { get; set; }
        public string? Detail { get; set; }
        public string? Message { get; set; }
        public Dictionary<string, string[]>? Errors { get; set; }

        public string? ObterMensagem() =>
            Errors?.Values.SelectMany(mensagens => mensagens).FirstOrDefault()
            ?? Message
            ?? Detail
            ?? Title;
    }
}