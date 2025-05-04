using ApiPDV.DTOs.Response;
using ApiPDV.Models;

namespace ApiPDV.DTOs
{
    public class ProdutoCarrinhoDTO
    {
        public int Id { get; set; }
        public ProdutoResponseDTO? ProdutoResponseDto { get; set; }
        public int ProdutoId { get; set; }
        public int Quantidade { get; set; }
        public decimal ValorUnitario { get; set; }
        public decimal ValorTotal => Quantidade * ValorUnitario;
    }
}
