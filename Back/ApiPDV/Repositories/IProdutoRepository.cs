using ApiPDV.Models;
using ApiPDV.Pagination;
using System.Linq.Expressions;

namespace ApiPDV.Repositories
{
    public interface IProdutoRepository : IRepository<Produto>
    {
        
        Task<PagedList<Produto>> GetAllPagFiltroPrecoAsync(ProdutosFiltroPreco produtosFiltroPreco);
        Task<Produto> GetNoTrackingAsync(Expression<Func<Produto, bool>> predicate);
    }
}
