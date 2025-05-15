using System.ComponentModel.DataAnnotations;

namespace ApiPDV.DTOs
{
    public class RegisterModel
    {
        [Required(ErrorMessage="Insira o usuário")]
        public string? UserName { get; set; }

        [EmailAddress]
        [Required(ErrorMessage = "Insira o Email")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Insira a senha")]
        public string? Password { get; set; }
    }
}
