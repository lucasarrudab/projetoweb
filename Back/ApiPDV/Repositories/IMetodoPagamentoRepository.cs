using ApiPDV.Models;

namespace ApiPDV.Repositories
{
    public interface IMetodoPagamentoRepository : IRepository<MetodoPagamento>
    {
        Task<MetodoPagamento> GetPagamentoPorNome(string metodoPagamento);
    }
}
