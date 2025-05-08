using ApiPDV.Models;

namespace ApiPDV.Repositories
{
    public interface IVendaRepository : IRepository<Venda>
    {
        Task<IEnumerable<Venda>> GetAllIncludeAsync();

        Task<Venda> GetIncludeAsync(int id);
    }
}
