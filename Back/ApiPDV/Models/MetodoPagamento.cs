namespace ApiPDV.Models;

public class MetodoPagamento
{
    public int Id { get; set; }
    public string Nome { get; set; }
    public ICollection<Venda>? Vendas { get; set; }
}
