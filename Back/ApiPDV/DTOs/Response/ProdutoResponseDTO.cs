namespace ApiPDV.DTOs.Response
{
    public class ProdutoResponseDTO
    {
        /// <summary>
        /// Id do produto
        /// </summary>
        public int Id { get; set; }
        public string? Nome { get; set; }
        public string? Codigo { get; set; }
        public string? URLImagem { get; set; }
        public decimal Preco { get; set; }
        public DateTime DataCadastro { get; set; }
        public int Estoque { get; set; }
    }
}
