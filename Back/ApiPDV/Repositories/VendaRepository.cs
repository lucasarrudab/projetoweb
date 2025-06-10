using ApiPDV.Context;
using ApiPDV.Models;
using ApiPDV.Pagination;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Linq.Expressions;

namespace ApiPDV.Repositories
{
    public class VendaRepository : Repository<Venda>, IVendaRepository
    {
        public VendaRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Venda>> GetAllIncludeAsync()
        {
            return await _context.Vendas.Include(v => v.Carrinho).ThenInclude(c => c.Produtos).ThenInclude(pc => pc.Produto).Include(v => v.MetodoPagamento).OrderByDescending(v => v.Data).ToListAsync();
        }

        public async Task<PagedList<Venda>> GetAllIncludePagedAsync<TKey>(Expression<Func<Venda, bool>>? predicate, Expression<Func<Venda, TKey>>? orderPredicate, int pageNumber, int pageSize)
        {
            var query = _context.Vendas.Include(v => v.Carrinho).ThenInclude(c => c.Produtos).ThenInclude(pc => pc.Produto).Include(v => v.MetodoPagamento).AsQueryable(); 

            if (predicate != null)
            {
                query = query.Where(predicate);
            }
            if (orderPredicate != null)
            {
                query = query.OrderBy(orderPredicate);
            }
            return await PagedList<Venda>.ToPagedListAsync(query, pageNumber, pageSize);
        }

        public async Task<Venda> GetIncludeAsync(int id)
        {
            return await _context.Vendas.Where(v => v.Id == id).Include(v => v.Carrinho).ThenInclude(c => c.Produtos).ThenInclude(pc => pc.Produto).Include(v => v.MetodoPagamento).FirstOrDefaultAsync();

        }

        public async Task<IEnumerable<Venda>> GetAllDay(VendasFiltroData vendasParameters)
        {
            if(vendasParameters.DataFinal == DateTime.MinValue)
            {
                return await GetAllIncludePagedAsync(v => v.Data.DayOfYear == vendasParameters.DataInicial.DayOfYear, v => v.Data, vendasParameters.PageNumber, vendasParameters.PageSize);
            }
            return await GetAllIncludePagedAsync(v => v.Data.DayOfYear >= vendasParameters.DataInicial.DayOfYear && v.Data.DayOfYear <= vendasParameters.DataFinal.DayOfYear, v => v.Data, vendasParameters.PageNumber, vendasParameters.PageSize);
        }

        public async Task<IEnumerable<Venda>> GetAllMonth(VendaFiltroMes vendasParameters)
        {
            
                return await GetAllIncludePagedAsync(v => v.Data.Month == vendasParameters.Mes, v => v.Data, vendasParameters.PageNumber, vendasParameters.PageSize);
            
            
        }

        public async Task<IEnumerable<Venda>> GetAllPayment(VendaFiltroPagamento vendasParameters)
        {

            return await GetAllIncludePagedAsync(v => v.MetodoPagamento.Nome.ToUpper().Equals(vendasParameters.Pagamento.ToUpper()), v => v.Data, vendasParameters.PageNumber, vendasParameters.PageSize);


        }

        public async Task<IEnumerable<Venda>> GetAllDays(VendasFiltroDias vendasParameters)
        {

            return await GetAllIncludePagedAsync(v =>(DateTime.UtcNow.DayOfYear - v.Data.DayOfYear) <= (vendasParameters.Dias-1), v =>v.Data , vendasParameters.PageNumber, vendasParameters.PageSize);


        }







    }
}
