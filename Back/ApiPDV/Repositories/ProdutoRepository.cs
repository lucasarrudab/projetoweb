using ApiPDV.Context;
using ApiPDV.Models;
using ApiPDV.Pagination;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace ApiPDV.Repositories
{
    public class ProdutoRepository : Repository<Produto>, IProdutoRepository
    {

        public ProdutoRepository(AppDbContext context) : base(context) { }

        public async Task<Produto> FindByBarCode(string barCode)
        {
            var produto = await _context.Produtos.Where(p => p.Codigo.Equals(barCode)).AsNoTracking().FirstOrDefaultAsync();
            return produto;
        }

        public async Task<PagedList<Produto>> GetAllPagFiltroPrecoAsync(ProdutosFiltroPreco produtosFiltroPreco)
        {
            PagedList<Produto> produtos = new PagedList<Produto>();

            if (produtosFiltroPreco.Preco.HasValue && !string.IsNullOrEmpty(produtosFiltroPreco.PrecoCriterio))
            {
                if (produtosFiltroPreco.PrecoCriterio.Equals("maior", StringComparison.OrdinalIgnoreCase))
                {
                    produtos = await GetPagedAsync(p => p.Preco > produtosFiltroPreco.Preco.Value,p => p.Nome, produtosFiltroPreco.PageNumber, produtosFiltroPreco.PageSize);
                }
                else if (produtosFiltroPreco.PrecoCriterio.Equals("menor", StringComparison.OrdinalIgnoreCase))
                {
                    produtos = await GetPagedAsync(p => p.Preco < produtosFiltroPreco.Preco.Value, p => p.Nome, produtosFiltroPreco.PageNumber, produtosFiltroPreco.PageSize);
                }
                else if (produtosFiltroPreco.PrecoCriterio.Equals("igual", StringComparison.OrdinalIgnoreCase))
                {
                    produtos = await GetPagedAsync(p => p.Preco == produtosFiltroPreco.Preco.Value, p => p.Nome, produtosFiltroPreco.PageNumber, produtosFiltroPreco.PageSize);
                }


            }
            return produtos;
        }
    }
}
