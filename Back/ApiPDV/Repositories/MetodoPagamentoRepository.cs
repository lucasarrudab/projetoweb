using ApiPDV.Context;
using ApiPDV.Models;
using Microsoft.EntityFrameworkCore;

namespace ApiPDV.Repositories;

public class MetodoPagamentoRepository : Repository<MetodoPagamento>, IMetodoPagamentoRepository
{
    
    public MetodoPagamentoRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<MetodoPagamento> GetPagamentoPorNome(string metodoPagamento)
    {
        return await _context.MetodosPagamento.Where(mp => mp.Nome.Equals(metodoPagamento, StringComparison.OrdinalIgnoreCase)).FirstOrDefaultAsync();
    }
}
