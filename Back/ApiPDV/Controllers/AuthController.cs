using ApiPDV.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Supabase.Gotrue;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpGet("usuarios")]
    public async Task<ActionResult<IEnumerable<object>>> GetUsers() =>
        Ok(await _authService.GetUsersAsync());

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginModel model) =>
        await _authService.LoginAsync(model);

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterModel model) =>
        await _authService.RegisterAsync(model);

    [HttpPut("{id}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> EditUser([FromBody] UpdateModel model, string id) =>
        await _authService.EditUserAsync(model, id);

    [HttpPut("senha/{id}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordModel model, string id) =>
        await _authService.ChangePasswordAsync(model, id);

    [HttpDelete("{id}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> DeleteUser(string id) =>
        await _authService.DeleteUserAsync(id);

    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshToken([FromBody] TokenModel tokenModel) =>
        await _authService.RefreshTokenAsync(tokenModel);

    [HttpPost("revoke/{username}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> Revoke(string username) =>
        await _authService.RevokeAsync(username);

    [HttpPost("CreateRole")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> CreateRole(string roleName) =>
        await _authService.CreateRoleAsync(roleName);

    [HttpPost("AddUserToRole")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> AddUserToRole(string email, string roleName) =>
        await _authService.AddUserToRoleAsync(email, roleName);

    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> Logout() =>
        await _authService.LogoutAsync(User);
}
