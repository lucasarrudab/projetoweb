using System.Text.Json.Serialization;

namespace ApiPDV.Models;

public class ProdutoCarrinho
{
    public int Id { get; set; }
    public Produto? Produto { get; set; }
    public int ProdutoId { get; set; }
    public int Quantidade { get; set; }
    public decimal ValorUnitario { get; set; }
    public decimal ValorTotal => Quantidade * ValorUnitario;

    [JsonIgnore]
    public Carrinho? Carrinho { get; set; }

   

}
