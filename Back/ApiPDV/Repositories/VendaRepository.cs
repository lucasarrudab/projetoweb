using ApiPDV.Context;
using ApiPDV.Models;
using Microsoft.EntityFrameworkCore;

namespace ApiPDV.Repositories
{
    public class VendaRepository : Repository<Venda>, IVendaRepository
    {
        public VendaRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Venda>> GetAllIncludeAsync()
        {
            return await _context.Vendas.Include(v => v.Carrinho).ThenInclude(c => c.Produtos).ThenInclude(pc => pc.Produto).Include(v => v.MetodoPagamento).ToListAsync();
        }

        public async Task<Venda> GetIncludeAsync(int id)
        {
            return await _context.Vendas.Where(v => v.Id == id).Include(v => v.Carrinho).ThenInclude(c => c.Produtos).ThenInclude(pc => pc.Produto).Include(v => v.MetodoPagamento).FirstOrDefaultAsync();

        }
    }
}
