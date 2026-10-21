using System.Net.Http.Json;
using System.Text.Json;
using PdvSistema.Web.Models;

namespace PdvSistema.Web.Services;

public sealed class EstoqueApiClient(
    HttpClient http,
    PdvAuthenticationStateProvider authState,
    ILogger<EstoqueApiClient> logger)
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    public async Task<List<ProdutoDto>> ListarProdutosAsync() =>
        await ObterAsync<List<ProdutoDto>>("api/Produto") ?? [];

    public async Task<List<CategoriaDto>> ListarCategoriasAsync() =>
        await ObterAsync<List<CategoriaDto>>("api/Categoria") ?? [];

    public async Task<List<ClienteDto>> ListarClientesAsync() =>
        await ObterAsync<List<ClienteDto>>("api/Cliente") ?? [];

    public async Task<List<VendaDto>> ListarVendasAsync() =>
        await ObterAsync<List<VendaDto>>("api/Venda") ?? [];

    public async Task<List<MovimentacaoEstoqueDto>> ListarMovimentacoesAsync(int? produtoId = null) =>
        await ObterAsync<List<MovimentacaoEstoqueDto>>(produtoId is null
            ? "api/MovimentacaoEstoque"
            : $"api/MovimentacaoEstoque?produtoId={produtoId}") ?? [];

    public Task<ProdutoDto> CriarProdutoAsync(ProdutoFormModel model) =>
        EnviarComRespostaAsync<ProdutoDto>(HttpMethod.Post, "api/Produto", new
        {
            model.Nome,
            model.CodigoBarras,
            model.PrecoCusto,
            model.PrecoVenda,
            model.EstoqueMinimo,
            model.CategoriaId
        });

    public async Task AtualizarProdutoAsync(ProdutoFormModel model) =>
        await EnviarSemRespostaAsync(HttpMethod.Put, $"api/Produto/{model.Id}", model);

    public async Task RemoverProdutoAsync(int id) =>
        await EnviarSemRespostaAsync(HttpMethod.Delete, $"api/Produto/{id}");

    public Task RegistrarMovimentacaoAsync(MovimentacaoEstoqueFormModel model) =>
        EnviarSemRespostaAsync(HttpMethod.Post, "api/MovimentacaoEstoque", model);

    public Task<ClienteDto> CriarClienteAsync(ClienteFormModel model) =>
        EnviarComRespostaAsync<ClienteDto>(HttpMethod.Post, "api/Cliente", model);

    public Task AtualizarClienteAsync(int id, ClienteFormModel model) =>
        EnviarSemRespostaAsync(HttpMethod.Put, $"api/Cliente/{id}", model);

    public Task RemoverClienteAsync(int id) =>
        EnviarSemRespostaAsync(HttpMethod.Delete, $"api/Cliente/{id}");

    public Task<VendaDto> CriarVendaAsync(VendaRequestModel model) =>
        EnviarComRespostaAsync<VendaDto>(HttpMethod.Post, "api/Venda", model);

    public Task CancelarVendaAsync(int id) =>
        EnviarSemRespostaAsync(HttpMethod.Put, $"api/Venda/{id}/cancelar");

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

        HttpResponseMessage response;
        try
        {
            response = await http.SendAsync(request);
        }
        catch (HttpRequestException exception)
        {
            logger.LogError(exception, "Could not reach the API for {Method} {Endpoint}.", metodo, caminho);
            throw;
        }
        catch (TaskCanceledException exception)
        {
            logger.LogWarning(exception, "The API request timed out for {Method} {Endpoint}.", metodo, caminho);
            throw;
        }

        if (response.IsSuccessStatusCode)
            return response;

        var conteudo = await response.Content.ReadAsStringAsync();
        var statusCode = response.StatusCode;
        var erro = LerErro(conteudo);
        response.Dispose();

        if (statusCode == System.Net.HttpStatusCode.Unauthorized
            && !caminho.StartsWith("api/Auth/", StringComparison.OrdinalIgnoreCase))
        {
            await authState.EncerrarSessaoAsync();
            logger.LogWarning("The API rejected the session for {Method} {Endpoint} with HTTP {StatusCode}.",
                metodo, caminho, (int)statusCode);
            throw new HttpRequestException("Sua sessão expirou ou não está mais autorizada. Entre novamente.", null, statusCode);
        }

        var mensagem = erro?.ObterMensagem() ?? $"A API retornou o erro {(int)statusCode}.";
        logger.LogWarning("The API request {Method} {Endpoint} returned HTTP {StatusCode}.",
            metodo, caminho, (int)statusCode);
        throw new HttpRequestException(mensagem, null, statusCode);
    }

    private static ApiError? LerErro(string conteudo)
    {
        if (string.IsNullOrWhiteSpace(conteudo))
            return null;

        try { return JsonSerializer.Deserialize<ApiError>(conteudo, JsonOptions); }
        catch (JsonException) { return null; }
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