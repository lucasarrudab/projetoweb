using ApiPDV.Models;

namespace ApiPDV.Repositories
{
    public interface ICarrinhoRepository : IRepository<Carrinho>
    {
        Task<IEnumerable<Carrinho>> GetAllAsync();
        Task<Carrinho> GetAsync(int id);
        
    }


}
