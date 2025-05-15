using ApiPDV.DTOs;
using ApiPDV.Models;
using ApiPDV.Pagination;
using Microsoft.AspNetCore.Mvc;
using System.Linq.Expressions;

namespace ApiPDV.Repositories
{
    public interface IVendaRepository : IRepository<Venda>
    {
        Task<IEnumerable<Venda>> GetAllIncludeAsync();
        Task<PagedList<Venda>> GetAllIncludePagedAsync<TKey>(Expression<Func<Venda, bool>>? predicate, Expression<Func<Venda, TKey>>? orderPredicate, int pageNumber, int pageSize);
        Task<Venda> GetIncludeAsync(int id);
        Task<IEnumerable<Venda>> GetAllDay(VendasFiltroData vendasParameters);
        Task<IEnumerable<Venda>> GetAllMonth(VendaFiltroMes vendasParameters);
        Task<IEnumerable<Venda>> GetAllPayment(VendaFiltroPagamento vendasParameters);
        Task<IEnumerable<Venda>> GetAllDays(VendasFiltroDias vendasParameters);
    }
}
