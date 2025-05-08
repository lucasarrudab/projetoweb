using ApiPDV.Models;
using System.ComponentModel.DataAnnotations;

namespace ApiPDV.DTOs
{
    public class VendaDTO
    {
        public int Id { get; set; }
        public CarrinhoDTO Carrinho { get; set; }
        public DateTime Data { get; set; }
        [Required(ErrorMessage = "Insira um metodo de pagamento")]
        public string? MetodoPagamento { get; set; }
    }
}
