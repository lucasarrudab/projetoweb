using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ApiPDV.Models;

public class MetodoPagamento
{
    public int Id { get; set; }
    
    public string Nome { get; set; }

    [JsonIgnore]
    public ICollection<Venda>? Vendas { get; set; }
}
