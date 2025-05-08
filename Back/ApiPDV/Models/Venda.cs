

using System.Text.Json.Serialization;

namespace ApiPDV.Models;

public class Venda
{
    public int Id { get; set; }
    public Carrinho? Carrinho { get; set; }
    public int CarrinhoId { get; set; }
    public DateTime Data { get; set; }
    public int MetodoPagamentoId { get; set; }

    
    public MetodoPagamento? MetodoPagamento { get; set; }
}
