using System.Net;
using System.Net.Http.Json;
using PdvSistema.Web.Models;

namespace PdvSistema.Web.Services;

public sealed class EstoqueApiClient(HttpClient http)
{
    public async Task<List<ProdutoDto>> ListarProdutosAsync() =>
        await http.GetFromJsonAsync<List<ProdutoDto>>("api/Produto") ?? [];

    public async Task<List<CategoriaDto>> ListarCategoriasAsync() =>
        await http.GetFromJsonAsync<List<CategoriaDto>>("api/Categoria") ?? [];

    public async Task CriarProdutoAsync(ProdutoFormModel model) =>
        await EnviarAsync(() => http.PostAsJsonAsync("api/Produto", model));

    public async Task AtualizarProdutoAsync(ProdutoFormModel model) =>
        await EnviarAsync(() => http.PutAsJsonAsync($"api/Produto/{model.Id}", model));

    public async Task RemoverProdutoAsync(int id) =>
        await EnviarAsync(() => http.DeleteAsync($"api/Produto/{id}"));

    public async Task CriarCategoriaAsync(CategoriaFormModel model) =>
        await EnviarAsync(() => http.PostAsJsonAsync("api/Categoria", model));

    public async Task AtualizarCategoriaAsync(int id, CategoriaFormModel model) =>
        await EnviarAsync(() => http.PutAsJsonAsync($"api/Categoria/{id}", model));

    public async Task RemoverCategoriaAsync(int id) =>
        await EnviarAsync(() => http.DeleteAsync($"api/Categoria/{id}"));

    private static async Task EnviarAsync(Func<Task<HttpResponseMessage>> enviar)
    {
        using var response = await enviar();
        if (response.IsSuccessStatusCode)
            return;

        var erro = await response.Content.ReadFromJsonAsync<ApiError>();
        var mensagem = erro?.Message ?? response.StatusCode switch
        {
            HttpStatusCode.Conflict => "Este registro está vinculado a outros dados e não pode ser removido.",
            HttpStatusCode.NotFound => "O registro solicitado não foi encontrado.",
            _ => $"A API retornou o erro {(int)response.StatusCode}."
        };

        throw new HttpRequestException(mensagem, null, response.StatusCode);
    }

    private sealed class ApiError
    {
        public string? Message { get; set; }
    }
}