using System.ComponentModel.DataAnnotations;

namespace ApiPDV.DTOs;

public class LoginModel
{
    [Required(ErrorMessage ="Insira o usuário")]
    public string? UserName { get; set; }
    [Required(ErrorMessage = "Insira a senha")]
    public string? Password { get; set; }
}
