using ApiPDV.Context;
using ApiPDV.Models;

namespace ApiPDV.Repositories
{
    public class ProdutoCarrinhoRepository : Repository<ProdutoCarrinho>, IProdutoCarrinhoRepository
    {
        public ProdutoCarrinhoRepository(AppDbContext context) : base(context)
        {
        }
    }
}
