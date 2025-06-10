using ApiPDV.DTOs;
using ApiPDV.DTOs.Response;
using ApiPDV.Models;
using ApiPDV.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;

public class AuthService : IAuthService
{
    private readonly ITokenService _tokenService;
    private readonly UserManager<AplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly IConfiguration _configuration;
    private readonly ILogger<AuthService> _logger;

    public AuthService(ITokenService tokenService,
                       UserManager<AplicationUser> userManager,
                       RoleManager<IdentityRole> roleManager,
                       IConfiguration configuration,
                       ILogger<AuthService> logger)
    {
        _tokenService = tokenService;
        _userManager = userManager;
        _roleManager = roleManager;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<IEnumerable<object>> GetUsersAsync()
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

        return userRoles;
    }

    public async Task<IActionResult> LoginAsync(LoginModel model)
    {
        var user = await _userManager.FindByNameAsync(model.UserName);

        if (user != null && await _userManager.CheckPasswordAsync(user, model.Password))
        {
            var userRoles = await _userManager.GetRolesAsync(user);
            var authClaims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim("id", user.UserName),
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };

            foreach (var userRole in userRoles)
                authClaims.Add(new Claim(ClaimTypes.Role, userRole));

            var token = _tokenService.GenerateAccessToken(authClaims, _configuration);
            var refreshToken = _tokenService.GenerateRefreshToken();

            int.TryParse(_configuration["JWT:RefreshTokenValidityInMinutes"], out int validity);
            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddMinutes(validity);

            await _userManager.UpdateAsync(user);

            return new OkObjectResult(new
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                RefreshToken = refreshToken,
                Expiration = token.ValidTo
            });
        }

        return new UnauthorizedResult();
    }

    public async Task<IActionResult> RegisterAsync(RegisterModel model)
    {
        var userExists = await _userManager.FindByNameAsync(model.UserName);
        if (userExists != null)
        {
            return new ObjectResult(new LoginResponse { Status = "Error", Message = "User already exists!" })
            {
                StatusCode = StatusCodes.Status500InternalServerError
            };
        }

        var user = new AplicationUser
        {
            Email = model.Email,
            SecurityStamp = Guid.NewGuid().ToString(),
            UserName = model.UserName
        };

        var result = await _userManager.CreateAsync(user, model.Password);
        if (!result.Succeeded)
        {
            return new ObjectResult(new LoginResponse { Status = "Error", Message = "Falha ao criar usuario." })
            {
                StatusCode = StatusCodes.Status500InternalServerError
            };
        }

        return new OkObjectResult(new LoginResponse { Status = "Sucess", Message = "Usuario criado!" });
    }

    public async Task<IActionResult> EditUserAsync(UpdateModel model, string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null) return new BadRequestObjectResult("Usuario não encontrado");

        user.Email = model.Email;
        user.UserName = model.UserName;

        var result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded)
        {
            return new ObjectResult(new LoginResponse { Status = "Error", Message = "Erro ao atualizar usuario." })
            {
                StatusCode = StatusCodes.Status500InternalServerError
            };
        }

        return new OkObjectResult(new LoginResponse { Status = "Sucess", Message = "Usuario atualizado!" });
    }

    public async Task<IActionResult> ChangePasswordAsync(ChangePasswordModel model, string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null) return new BadRequestObjectResult("Usuario não encontrado");

        var result = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
        if (!result.Succeeded)
        {
            return new ObjectResult(new LoginResponse { Status = "Error", Message = "Senha incorreta." })
            {
                StatusCode = StatusCodes.Status400BadRequest
            };
        }

        return new OkObjectResult(new LoginResponse { Status = "Sucess", Message = "Senha atualizada com sucesso!" });
    }

    public async Task<IActionResult> DeleteUserAsync(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null) return new BadRequestObjectResult("Usuario não encontrado");

        var result = await _userManager.DeleteAsync(user);
        if (!result.Succeeded)
        {
            return new ObjectResult(new LoginResponse { Status = "Error", Message = "Erro ao excluir usuario." })
            {
                StatusCode = StatusCodes.Status500InternalServerError
            };
        }

        return new OkObjectResult(new LoginResponse { Status = "Sucess", Message = "Usuario excluido com sucesso!" });
    }

    public async Task<IActionResult> RefreshTokenAsync(TokenModel tokenModel)
    {
        if (tokenModel == null) return new BadRequestObjectResult("Invalid client request");

        var principal = _tokenService.GetPrincipalFromExpiredToken(tokenModel.AccessToken, _configuration);
        if (principal == null) return new BadRequestObjectResult("Invalid access token/refresh token");

        var username = principal.Identity?.Name;
        var user = await _userManager.FindByNameAsync(username);

        if (user == null || user.RefreshToken != tokenModel.RefreshToken || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
        {
            return new BadRequestObjectResult("Invalid access token/refresh token");
        }

        var newAccessToken = _tokenService.GenerateAccessToken(principal.Claims.ToList(), _configuration);
        var newRefreshToken = _tokenService.GenerateRefreshToken();

        user.RefreshToken = newRefreshToken;
        await _userManager.UpdateAsync(user);

        return new OkObjectResult(new
        {
            accessToken = new JwtSecurityTokenHandler().WriteToken(newAccessToken),
            refreshToken = newRefreshToken
        });
    }

    public async Task<IActionResult> RevokeAsync(string username)
    {
        var user = await _userManager.FindByNameAsync(username);
        if (user == null) return new BadRequestObjectResult("Invalid user name");

        user.RefreshToken = null;
        await _userManager.UpdateAsync(user);

        return new NoContentResult();
    }

    public async Task<IActionResult> CreateRoleAsync(string roleName)
    {
        var roleExist = await _roleManager.RoleExistsAsync(roleName);
        if (!roleExist)
        {
            var roleResult = await _roleManager.CreateAsync(new IdentityRole(roleName));
            if (roleResult.Succeeded)
            {
                _logger.LogInformation(1, "Roles Added");
                return new OkObjectResult(new LoginResponse { Status = "Sucess", Message = $"Role {roleName} added sucessfuly" });
            }
            else
            {
                _logger.LogInformation(2, "Error");
                return new BadRequestObjectResult(new LoginResponse { Status = "Error", Message = $"Issue adding the new {roleName} role" });
            }
        }

        return new BadRequestObjectResult(new LoginResponse { Status = "Error", Message = "Role already exist." });
    }

    public async Task<IActionResult> AddUserToRoleAsync(string email, string roleName)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user != null)
        {
            var result = await _userManager.AddToRoleAsync(user, roleName);
            if (result.Succeeded)
            {
                _logger.LogInformation(1, $"User {user.Email} added to the {roleName} role");
                return new OkObjectResult(new LoginResponse { Status = "Sucess", Message = $"User {user.Email} added to the {roleName} role" });
            }
            else
            {
                _logger.LogInformation(1, $"Error: Unable to add user {user.Email} to the {roleName} role");
                return new BadRequestObjectResult(new LoginResponse { Status = "Error", Message = $"Error: Unable to add user {user.Email} to the {roleName} role" });
            }
        }

        return new BadRequestObjectResult(new { Error = "Unable to find user" });
    }

    public async Task<IActionResult> LogoutAsync(ClaimsPrincipal userPrincipal)
    {
        var userId = userPrincipal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userId == null) return new UnauthorizedObjectResult("Usuário não autenticado.");

        var user = await _userManager.FindByIdAsync(userId);
        if (user == null) return new NotFoundObjectResult("Usuário não encontrado.");

        user.RefreshToken = null;
        user.RefreshTokenExpiryTime = DateTime.MinValue;
        await _userManager.UpdateAsync(user);

        return new OkObjectResult(new LoginResponse { Status = "Sucesso", Message = "Logout realizado com sucesso. Token revogado." });
    }
}
