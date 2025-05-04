namespace ApiPDV.DTOs.Response
{
    public class ProdutoResponseDTO
    {
        public int Id { get; set; }
        public string? Nome { get; set; }
        public string? Codigo { get; set; }
        public decimal Preco { get; set; }
        public DateTime DataCadastro { get; set; }
        public int Estoque { get; set; }
    }
}
