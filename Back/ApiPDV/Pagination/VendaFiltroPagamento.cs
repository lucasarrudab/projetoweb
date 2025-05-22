namespace ApiPDV.Pagination
{
    public class VendaFiltroPagamento : QueryStringParameters
    {
        /// <summary>
        /// Dinheiro, debito, credito ou pix
        /// </summary>
        public string? Pagamento { get; set; }
    }
}
