using ApiPDV.Models;

namespace ApiPDV.DTOs
{
    public class CarrinhoDTO
    {
        public int Id { get; set; }
        public decimal ValorTotalCarrinho { get; set; }
        public ICollection<ProdutoCarrinhoDTO>? Produtos { get; set; }
    }
}
