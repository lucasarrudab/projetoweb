using ApiPDV.DTOs;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

public interface IAuthService
{
    Task<IEnumerable<object>> GetUsersAsync();
    Task<IActionResult> LoginAsync(LoginModel model);
    Task<IActionResult> RegisterAsync(RegisterModel model);
    Task<IActionResult> EditUserAsync(UpdateModel model, string id);
    Task<IActionResult> ChangePasswordAsync(ChangePasswordModel model, string id);
    Task<IActionResult> DeleteUserAsync(string id);
    Task<IActionResult> RefreshTokenAsync(TokenModel tokenModel);
    Task<IActionResult> RevokeAsync(string username);
    Task<IActionResult> CreateRoleAsync(string roleName);
    Task<IActionResult> AddUserToRoleAsync(string email, string roleName);
    Task<IActionResult> LogoutAsync(ClaimsPrincipal user);
}
