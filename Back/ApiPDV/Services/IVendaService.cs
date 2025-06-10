using ApiPDV.DTOs;
using ApiPDV.Models;
using ApiPDV.Pagination;

namespace ApiPDV.Services
{
    public interface IVendasService
    {
        Task<IEnumerable<VendaDTO>> GetAllVendasAsync();
        Task<VendaDTO> GetVendaByIdAsync(int id);
        Task<PagedList<VendaDTO>> GetPagedVendasAsync(VendasParameters parameters);
        Task<IEnumerable<VendaDTO>> GetVendasByDataAsync(VendasFiltroData parameters);
        Task<IEnumerable<VendaDTO>> GetVendasByMesAsync(VendaFiltroMes parameters);
        Task<IEnumerable<VendaDTO>> GetVendasByPagamentoAsync(VendaFiltroPagamento parameters);
        Task<IEnumerable<VendaDTO>> GetVendasByDiasAsync(VendasFiltroDias parameters);
        Task<(bool Success, string? ErrorMessage, VendaDTO? Venda)> RealizarVendaAsync(string nomeMetodoPagamento);
    }
}
