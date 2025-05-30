using System.ComponentModel.DataAnnotations;

namespace ApiPDV.DTOs
{
    public class ChangePasswordModel
    {
        [Required(ErrorMessage = "Insira a senha")]
        public string? OldPassword { get; set; }

        [Required(ErrorMessage = "Insira a senha")]
        public string? NewPassword { get; set; }
    }
}
