namespace ApiPDV.Pagination
{
    public class VendasFiltroData : QueryStringParameters
    {
        public DateTime DataInicial { get; set; } 
        public DateTime DataFinal { get; set; } = DateTime.MinValue;
    }
}
