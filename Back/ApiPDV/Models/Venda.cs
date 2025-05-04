

namespace ApiPDV.Models;

public class Venda
{
    public int Id { get; set; }
    public Carrinho? Carrinho { get; set; }
    public int CarrinhoId { get; set; }
    public DateTime Data { get; set; }
    public MetodoPagamento? MetodoPagamento { get; set; }
}
