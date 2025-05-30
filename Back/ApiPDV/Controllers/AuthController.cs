using ApiPDV.DTOs;
using ApiPDV.DTOs.Response;
using ApiPDV.Models;
using ApiPDV.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace ApiPDV.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly ITokenService _tokenService;
    private readonly UserManager<AplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly IConfiguration _configuration;
    private readonly ILogger<AuthController> _logger;

    public AuthController(ITokenService tokenService, UserManager<AplicationUser> userManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration, ILogger<AuthController> logger)
    {
        _tokenService = tokenService;
        _userManager = userManager;
        _roleManager = roleManager;
        _configuration = configuration;
        _logger = logger;
    }

    [HttpGet("usuarios")]
    public async Task<ActionResult<IEnumerable<object>>> GetUsers()
    {
        var users = _userManager.Users.ToList();
        var userRoles = new List<object>();

        foreach (var user in users)
        {
            var roles = await _userManager.GetRolesAsync(user);
            userRoles.Add(new
            {
                user.Id,
                user.UserName,
                user.Email,
                Roles = roles
            });
        }

        return Ok(userRoles);
    }

    /// <summary>
    /// Verifica as credenciais de um usuário
    /// </summary>
    [HttpPost]
    [Route("login")]
    public async Task<IActionResult> Login([FromBody] LoginModel model)
    {
        var user = await _userManager.FindByNameAsync(model.UserName!);

        if (user is not null && await _userManager.CheckPasswordAsync(user, model.Password!))
        {
            var userRoles = await _userManager.GetRolesAsync(user);
            var authClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.UserName!),
                    new Claim(ClaimTypes.Email, user.Email!),
                    new Claim("id", user.UserName!),
                    new Claim(ClaimTypes.NameIdentifier, user.Id),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                };

            foreach (var userRole in userRoles)
            {
                authClaims.Add(new Claim(ClaimTypes.Role, userRole));

            }

            var token = _tokenService.GenerateAccessToken(authClaims, _configuration);

            var refreshToken = _tokenService.GenerateRefreshToken();

            _ = int.TryParse(_configuration["JWT:RefreshTokenValidityInMinutes"], out int refreshTokenValidityInMinutes);

            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddMinutes(refreshTokenValidityInMinutes);

            user.RefreshToken = refreshToken;

            await _userManager.UpdateAsync(user);

            return Ok(new
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                RefreshToken = refreshToken,
                Expiration = token.ValidTo
            });


        }
        return Unauthorized();
    }

    /// <summary>
    /// Registra um novo usuário
    /// </summary>
    [HttpPost]
    [Route("register")]
    public async Task<IActionResult> Register([FromBody] RegisterModel model)
    {
        var userExistes = await _userManager.FindByNameAsync(model.UserName!);

        if (userExistes is not null)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new LoginResponse { Status = "Error", Message = " User already exists!" });
        }

        AplicationUser user = new()
        {
            Email = model.Email,
            SecurityStamp = Guid.NewGuid().ToString(),
            UserName = model.UserName
        };

        var result = await _userManager.CreateAsync(user, model.Password!);

        if (!result.Succeeded)
        {
            return StatusCode(StatusCodes.Status500InternalServerError,
                new LoginResponse { Status = "Error", Message = "Falha ao criar usuario." });
        }

        return Ok(new LoginResponse { Status = "Sucess", Message = "Usuario criado!" });
    }

    [HttpPut("{id}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> EditUser([FromBody] UpdateModel model, string id)
    {

        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
        {
            return BadRequest("Usuario não encontrado");
        }
        user.Email = model.Email;
        user.UserName = model.UserName;
        var result = await _userManager.UpdateAsync(user);

        if (!result.Succeeded)
        {
            return StatusCode(StatusCodes.Status500InternalServerError,
                new LoginResponse { Status = "Error", Message = "Erro ao atualizar usuario." });
        }

        return Ok(new LoginResponse { Status = "Sucess", Message = "Usuario atualizado!" });

    }

    [HttpPut("senha/{id}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordModel model, string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
        {
            return BadRequest("Usuario não encontrado");
        }

        var result = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);

        if (!result.Succeeded)
        {
            return StatusCode(StatusCodes.Status400BadRequest,
                new LoginResponse { Status = "Error", Message = "Senha incorreta." });
        }
        return Ok(new LoginResponse { Status = "Sucess", Message = "Senha atualizada com sucesso!" });
    }

    [HttpDelete("{id}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> DeleteUser(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
        {
            return BadRequest("Usuario não encontrado");
        }

        var result = await _userManager.DeleteAsync(user);
        if (!result.Succeeded)
        {
            return StatusCode(StatusCodes.Status500InternalServerError,
                new LoginResponse { Status = "Error", Message = "Erro ao excluir usuario." });
        }
        return Ok(new LoginResponse { Status = "Sucess", Message = "Usuario excluido com sucesso!" });
    }

    /// <summary>
    /// Gera um novo token de acesso
    /// </summary>
    [HttpPost]
    [Route("refresh-token")]
    public async Task<IActionResult> RefreshToken(TokenModel tokenModel)
    {

        if (tokenModel is null)
        {
            return BadRequest("Invalid client request");
        }

        string? accessToken = tokenModel.AccessToken
                              ?? throw new ArgumentNullException(nameof(tokenModel));

        string? refreshToken = tokenModel.RefreshToken
                               ?? throw new ArgumentException(nameof(tokenModel));

        var principal = _tokenService.GetPrincipalFromExpiredToken(accessToken!, _configuration);

        if (principal == null)
        {
            return BadRequest("Invalid access token/refresh token");
        }

        string username = principal.Identity.Name;

        var user = await _userManager.FindByNameAsync(username!);

        if (user == null || user.RefreshToken != refreshToken
                         || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
        {
            return BadRequest("Invalid access token/refresh token");
        }

        var newAccessToken = _tokenService.GenerateAccessToken(
                                           principal.Claims.ToList(), _configuration);

        var newRefreshToken = _tokenService.GenerateRefreshToken();

        user.RefreshToken = newRefreshToken;

        await _userManager.UpdateAsync(user);

        return new ObjectResult(new
        {
            accessToken = new JwtSecurityTokenHandler().WriteToken(newAccessToken),
            refreshToken = newRefreshToken
        });
    }

    /// <summary>
    /// Revoga um refresh token
    /// </summary>
    /// /// <remarks>
    /// Acesso restrito ao perfil: <b>Admin</b>.
    /// </remarks>
    [Authorize(Policy = "AdminOnly")]
    [HttpPost]
    [Route("revoke/{username}")]
    public async Task<IActionResult> Revoke(string username)
    {
        var user = await _userManager.FindByNameAsync(username);

        if (user == null) return BadRequest("Invalid user name");

        user.RefreshToken = null;

        await _userManager.UpdateAsync(user);

        return NoContent();
    }

    /// <summary>
    /// Cria um novo cargo
    /// </summary>
    /// /// <remarks>
    /// Acesso restrito ao perfil: <b>Admin</b>.
    /// </remarks>
    [HttpPost]
    [Route("CreateRole")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> CreateRole(string roleName)
    {
        var roleExist = await _roleManager.RoleExistsAsync(roleName);

        if (!roleExist)
        {
            var roleResult = await _roleManager.CreateAsync(new IdentityRole(roleName));

            if (roleResult.Succeeded)
            {
                _logger.LogInformation(1, "Roles Added");
                return StatusCode(StatusCodes.Status200OK, new LoginResponse { Status = "Sucess", Message = $"Role {roleName} added sucessfuly" });

            }
            else
            {
                _logger.LogInformation(2, "Error");
                return StatusCode(StatusCodes.Status400BadRequest, new LoginResponse
                {
                    Status = "Error",
                    Message =
                    $"Issue adding the new {roleName} role"
                });
            }
        }
        return StatusCode(StatusCodes.Status400BadRequest, new LoginResponse
        {
            Status = "Error",
            Message = "Role already exist. "
        });

    }

    /// <summary>
    /// Adiciona um usuario a um cargo
    /// </summary>
    /// /// <remarks>
    /// Acesso restrito ao perfil: <b>Admin</b>.
    /// </remarks>
    [HttpPost]
    [Route("AddUserToRole")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> AddUserToRole(string email, string roleName)
    {
        var user = await _userManager.FindByEmailAsync(email);

        if (user != null)
        {
            var result = await _userManager.AddToRoleAsync(user, roleName);
            if (result.Succeeded)
            {
                _logger.LogInformation(1, $"User {user.Email} added to the {roleName} role");
                return StatusCode(StatusCodes.Status200OK,
                    new LoginResponse
                    {
                        Status = "Sucess",
                        Message =
                    $"User {user.Email} added to the {roleName} role"
                    });
            }
            else
            {
                _logger.LogInformation(1, $"Error: Unable to add user {user.Email} to the {roleName} role");
                return StatusCode(StatusCodes.Status400BadRequest,
                    new LoginResponse
                    {
                        Status = "Error",
                        Message =
                    $"Error: Unable to add user {user.Email} to the {roleName} role"
                    });
            }
        }
        return BadRequest(new { Error = "Unable to find user" });
    }

    /// <summary>
    /// Realiza o logout
    /// </summary>
    [Authorize]
    [HttpPost]
    [Route("logout")]
    public async Task<IActionResult> Logout()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (userId == null)
            return Unauthorized("Usuário não autenticado.");

        var user = await _userManager.FindByIdAsync(userId);

        if (user == null)
            return NotFound("Usuário não encontrado.");

        // Invalida o refresh token
        user.RefreshToken = null;
        user.RefreshTokenExpiryTime = DateTime.MinValue;

        await _userManager.UpdateAsync(user);

        return Ok(new LoginResponse
        {
            Status = "Sucesso",
            Message = "Logout realizado com sucesso. Token revogado."
        });
    }
}
