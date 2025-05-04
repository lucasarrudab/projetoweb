using ApiPDV.Models;
using ApiPDV.Pagination;

namespace ApiPDV.Repositories
{
    public interface IProdutoRepository : IRepository<Produto>
    {
        
        Task<PagedList<Produto>> GetAllPagFiltroPrecoAsync(ProdutosFiltroPreco produtosFiltroPreco);
        Task<Produto> FindByBarCode (string barCode);
    }
}
