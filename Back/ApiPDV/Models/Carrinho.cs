using ApiPDV.Models.Enum;

namespace ApiPDV.Models;

public class Carrinho
{
    public int Id { get; set; }
    public ICollection<ProdutoCarrinho> Produtos { get; set; } = new List<ProdutoCarrinho>();
    public decimal ValorTotalCarrinho => Produtos.Count > 0 ? Produtos.Sum(p => p.ValorTotal) : 0;
    public StatusCarrinho Situacao { get; set; }
}
