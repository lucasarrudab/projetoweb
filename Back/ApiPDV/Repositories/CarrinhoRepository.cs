using ApiPDV.Context;
using ApiPDV.Models;
using Microsoft.EntityFrameworkCore;

namespace ApiPDV.Repositories
{
    public class CarrinhoRepository : Repository<Carrinho>, ICarrinhoRepository
    {
        public CarrinhoRepository(AppDbContext context) : base(context) { }

        public  async Task<IEnumerable<Carrinho>> GetAllAsync()
        {

            return await _context.Carrinhos.Include(c => c.Produtos).ThenInclude(pc => pc.Produto).ToListAsync();
        }

    }
}
