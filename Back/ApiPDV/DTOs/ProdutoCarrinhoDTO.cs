using ApiPDV.DTOs.Response;
using ApiPDV.Models;
using System.ComponentModel.DataAnnotations;

namespace ApiPDV.DTOs
{
    public class ProdutoCarrinhoDTO
    {
        public int Id { get; set; }
        public string? ProdutoNome { get; set; }
        public string? UrlImagem { get; set; }
        public int ProdutoId { get; set; }
        [Range(0,9999)]
        public int Quantidade { get; set; }
        public decimal ValorUnitario { get; set; }
        public decimal ValorTotal => Quantidade * ValorUnitario;
    }
}
