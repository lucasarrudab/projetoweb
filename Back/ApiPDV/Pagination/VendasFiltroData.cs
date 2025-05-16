namespace ApiPDV.Pagination
{
    public class VendasFiltroData : QueryStringParameters
    {
        /// <summary>
        /// Data inicial
        /// </summary>
        public DateTime DataInicial { get; set; }
        /// <summary>
        /// Data final
        /// </summary>
        public DateTime DataFinal { get; set; } = DateTime.MinValue;
    }
}
