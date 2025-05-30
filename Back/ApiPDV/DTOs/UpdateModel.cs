using System.ComponentModel.DataAnnotations;

namespace ApiPDV.DTOs
{
    public class UpdateModel
    {
        [Required(ErrorMessage = "Insira o usuário")]
        public string? UserName { get; set; }

        [EmailAddress]
        [Required(ErrorMessage = "Insira o Email")]
        public string? Email { get; set; }
    }
}
