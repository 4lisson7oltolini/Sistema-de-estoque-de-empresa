using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.IdentityModel.Tokens;
using PdvSistema.Application.DTOs.Usuario;
using PdvSistema.Application.Interfaces;
using PdvSistema.Domain.Enums;

namespace PdvSistema.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class AuthController(
    IAuthService authService,
    IConfiguration configuration,
    JwtSettings jwtSettings) : ControllerBase
{
    [AllowAnonymous]
    [EnableRateLimiting("authentication")]
    [HttpPost("login")]
    [ProducesResponseType<LoginResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status429TooManyRequests)]
    public async Task<ActionResult<LoginResponse>> Login(LoginRequest request)
    {
        var usuario = await authService.AutenticarAsync(request.Email, request.Senha);
        if (usuario is null)
            return Unauthorized(new ProblemDetails { Title = "E-mail ou senha inválidos." });

        var agora = DateTime.UtcNow;
        var expiraEm = agora.AddMinutes(jwtSettings.AccessTokenMinutes);
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, usuario.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, usuario.Email),
            new Claim(JwtRegisteredClaimNames.Name, usuario.Nome),
            new Claim(ClaimTypes.Role, usuario.Perfil.ToString())
        };
        var credentials = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SigningKey)),
            SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(
            issuer: jwtSettings.Issuer,
            audience: jwtSettings.Audience,
            claims: claims,
            notBefore: agora,
            expires: expiraEm,
            signingCredentials: credentials);

        return Ok(new LoginResponse(new JwtSecurityTokenHandler().WriteToken(token), expiraEm, usuario));
    }

    [AllowAnonymous]
    [EnableRateLimiting("authentication")]
    [HttpPost("bootstrap")]
    [ProducesResponseType<UsuarioResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Bootstrap(IniciarAdministracaoRequest request)
    {
        var chaveConfigurada = configuration["Bootstrap:SetupKey"];
        if (string.IsNullOrEmpty(chaveConfigurada))
            return NotFound();

        var chaveInformada = Encoding.UTF8.GetBytes(request.ChaveConfiguracao);
        var chaveEsperada = Encoding.UTF8.GetBytes(chaveConfigurada);
        if (!CryptographicOperations.FixedTimeEquals(chaveInformada, chaveEsperada))
            return Unauthorized(new ProblemDetails { Title = "Chave de configuração inválida." });

        try
        {
            var usuario = await authService.CriarAdministradorInicialAsync(request.Nome, request.Email, request.Senha);
            return Created($"/api/Usuario/{usuario.Id}", usuario);
        }
        catch (ArgumentException exception)
        {
            return BadRequest(new ProblemDetails { Title = exception.Message });
        }
        catch (InvalidOperationException exception)
        {
            return Conflict(new ProblemDetails { Title = exception.Message });
        }
    }
}

public sealed record LoginRequest
{
    [Required, EmailAddress, MaxLength(254)]
    public required string Email { get; init; }

    [Required, MaxLength(128)]
    public required string Senha { get; init; }
}

public sealed record IniciarAdministracaoRequest
{
    [Required, MaxLength(200)]
    public required string Nome { get; init; }

    [Required, EmailAddress, MaxLength(254)]
    public required string Email { get; init; }

    [Required, MinLength(12), MaxLength(128)]
    public required string Senha { get; init; }

    [Required, MinLength(32), MaxLength(256)]
    public required string ChaveConfiguracao { get; init; }
}

public sealed record LoginResponse(string AccessToken, DateTime ExpiresAt, UsuarioResponse Usuario);

public sealed class JwtSettings
{
    public string Issuer { get; init; } = "PdvSistema.API";
    public string Audience { get; init; } = "PdvSistema.Web";
    public string SigningKey { get; init; } = string.Empty;
    public int AccessTokenMinutes { get; init; } = 15;
}