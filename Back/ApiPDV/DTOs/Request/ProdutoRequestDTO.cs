using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiPDV.DTOs.Request
{
    public class ProdutoRequestDTO
    {
        [Required(ErrorMessage ="Insira um nome")]
        [MinLength(3)]
        [MaxLength(100)]
        public string? Nome { get; set; }
        [Required(ErrorMessage = "Insira o código do produto")]
        [MinLength(8)]
        [MaxLength(14)]
        public string? Codigo { get; set; }
        [Required(ErrorMessage = "Insira o valor do produto")]
        [Column(TypeName = "decimal(12,2)")]
        public decimal Preco { get; set; }
        [Required(ErrorMessage = "Insira o estoque do produto")]
        [Range(1, 9999)]
        public int Estoque { get; set; }



    }
}
