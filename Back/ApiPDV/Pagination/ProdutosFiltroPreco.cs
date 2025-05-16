namespace ApiPDV.Pagination
{
    public class ProdutosFiltroPreco : QueryStringParameters
    {
       
        public decimal? Preco { get; set; }
        /// <summary>
        /// Maior, menor, ou igual ao preço
        /// </summary>
        public string? PrecoCriterio { get; set; }


    }
}
