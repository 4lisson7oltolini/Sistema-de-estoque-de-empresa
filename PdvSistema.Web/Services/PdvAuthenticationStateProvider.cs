using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.JSInterop;
using PdvSistema.Web.Models;

namespace PdvSistema.Web.Services;

public sealed class PdvAuthenticationStateProvider(ProtectedSessionStorage storage) : AuthenticationStateProvider
{
    private const string StorageKey = "pdv-auth-session";
    private static readonly AuthenticationState Anonymous = new(new ClaimsPrincipal(new ClaimsIdentity()));
    private ProtectedAuthSession? _session;
    private bool _loaded;

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        await CarregarSessaoAsync();
        return _session is null ? Anonymous : CriarEstado(_session.Usuario);
    }

    public async Task<string?> ObterTokenAsync()
    {
        await CarregarSessaoAsync();
        return _session?.AccessToken;
    }

    public async Task IniciarSessaoAsync(LoginResultDto resposta)
    {
        var sessao = new ProtectedAuthSession(resposta.AccessToken, resposta.ExpiresAt, resposta.Usuario);
        await storage.SetAsync(StorageKey, sessao);
        _session = sessao;
        _loaded = true;
        NotifyAuthenticationStateChanged(Task.FromResult(CriarEstado(sessao.Usuario)));
    }

    public async Task EncerrarSessaoAsync()
    {
        try
        {
            await storage.DeleteAsync(StorageKey);
        }
        catch (InvalidOperationException)
        {
        }
        catch (JSDisconnectedException)
        {
        }

        _session = null;
        _loaded = true;
        NotifyAuthenticationStateChanged(Task.FromResult(Anonymous));
    }

    private async Task CarregarSessaoAsync()
    {
        if (_loaded)
            return;

        try
        {
            var resultado = await storage.GetAsync<ProtectedAuthSession>(StorageKey);
            _session = resultado.Success ? resultado.Value : null;
            if (_session is not null && _session.ExpiresAt <= DateTimeOffset.UtcNow)
            {
                _session = null;
                await storage.DeleteAsync(StorageKey);
            }
            _loaded = true;
        }
        catch (InvalidOperationException)
        {
        }
        catch (JSDisconnectedException)
        {
        }
    }

    private static AuthenticationState CriarEstado(AuthUsuarioDto usuario)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
            new Claim(ClaimTypes.Name, usuario.Nome),
            new Claim(ClaimTypes.Email, usuario.Email),
            new Claim(ClaimTypes.Role, usuario.Perfil switch
            {
                1 => "Administrador",
                2 => "Vendedor",
                _ => "SemPerfil"
            })
        };
        return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity(claims, "Bearer", ClaimTypes.Name, ClaimTypes.Role)));
    }
}