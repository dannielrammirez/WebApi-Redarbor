using Microsoft.AspNetCore.Mvc;
using Redarbor.Domain.Interfaces;
using Redarbor.Infrastructure.Authentication;
using System.Text.Json.Serialization;

namespace Redarbor.API.Controllers;

/// <summary>
/// OAuth2 Token Endpoint - implements Resource Owner Password Credentials grant (RFC 6749 Section 4.3).
/// </summary>
[ApiController]
[Route("api/oauth")]
[Produces("application/json")]
public class OAuth2Controller : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITokenService _tokenService;
    private readonly ILogger<OAuth2Controller> _logger;

    public OAuth2Controller(
        IUnitOfWork unitOfWork,
        ITokenService tokenService,
        ILogger<OAuth2Controller> logger)
    {
        _unitOfWork = unitOfWork;
        _tokenService = tokenService;
        _logger = logger;
    }

    /// <summary>
    /// Solicita un access token mediante OAuth2 password grant.
    /// </summary>
    /// <param name="grant_type">Debe ser "password".</param>
    /// <param name="username">Username del empleado.</param>
    /// <param name="password">Password del empleado.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <response code="200">Token generado exitosamente.</response>
    /// <response code="400">grant_type no soportado o campos faltantes.</response>
    /// <response code="401">Credenciales inválidas.</response>
    [HttpPost("token")]
    [Consumes("application/x-www-form-urlencoded")]
    [ProducesResponseType(typeof(OAuth2TokenResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(OAuth2ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(OAuth2ErrorResponse), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Token(
        [FromForm] string grant_type,
        [FromForm] string username,
        [FromForm] string password,
        CancellationToken cancellationToken)
    {
        if (grant_type != "password")
        {
            return BadRequest(new OAuth2ErrorResponse
            {
                Error = "unsupported_grant_type",
                ErrorDescription = "Only 'password' grant type is supported."
            });
        }

        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
        {
            return BadRequest(new OAuth2ErrorResponse
            {
                Error = "invalid_request",
                ErrorDescription = "Username and password are required."
            });
        }

        var employee = await _unitOfWork.Employees.GetByUsernameAsync(username, cancellationToken);

        if (employee is null || !employee.VerifyPassword(password))
        {
            _logger.LogWarning("OAuth2 token request failed for user: {Username}", username);
            return Unauthorized(new OAuth2ErrorResponse
            {
                Error = "invalid_grant",
                ErrorDescription = "Invalid username or password."
            });
        }

        if (employee.IsDeleted)
        {
            return Unauthorized(new OAuth2ErrorResponse
            {
                Error = "invalid_grant",
                ErrorDescription = "Account is disabled."
            });
        }

        var expiresIn = 3600;
        var accessToken = _tokenService.GenerateAccessToken(
            employee.Id,
            employee.Username.Value,
            employee.Email.Value);

        employee.RecordLogin();
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("OAuth2 token issued for user: {Username}", username);

        return Ok(new OAuth2TokenResponse
        {
            AccessToken = accessToken,
            TokenType = "Bearer",
            ExpiresIn = expiresIn
        });
    }
}

/// <summary>
/// OAuth2 successful token response (RFC 6749 Section 5.1).
/// </summary>
public sealed record OAuth2TokenResponse
{
    [JsonPropertyName("access_token")]
    public string AccessToken { get; init; } = string.Empty;

    [JsonPropertyName("token_type")]
    public string TokenType { get; init; } = string.Empty;

    [JsonPropertyName("expires_in")]
    public int ExpiresIn { get; init; }
}

/// <summary>
/// OAuth2 error response (RFC 6749 Section 5.2).
/// </summary>
public sealed record OAuth2ErrorResponse
{
    [JsonPropertyName("error")]
    public string Error { get; init; } = string.Empty;

    [JsonPropertyName("error_description")]
    public string ErrorDescription { get; init; } = string.Empty;
}
