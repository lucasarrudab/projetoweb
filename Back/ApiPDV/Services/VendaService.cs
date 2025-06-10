using ApiPDV.DTOs;
using ApiPDV.Models;
using ApiPDV.Pagination;
using ApiPDV.Repositories;
using AutoMapper;
using Microsoft.Extensions.Caching.Memory;

namespace ApiPDV.Services
{
    public class VendasService :IVendasService
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _uof;
        private readonly IMemoryCache _cache;

        private const string CacheKey = "CacheCarrinho";

        public VendasService(IMapper mapper, IUnitOfWork uof, IMemoryCache cache)
        {
            _mapper = mapper;
            _uof = uof;
            _cache = cache;
        }

        public async Task<IEnumerable<VendaDTO>> GetAllVendasAsync()
        {
            var vendas = await _uof.VendaRepository.GetAllIncludeAsync();
            return _mapper.Map<IEnumerable<VendaDTO>>(vendas);
        }

        public async Task<VendaDTO> GetVendaByIdAsync(int id)
        {
            var venda = await _uof.VendaRepository.GetIncludeAsync(id);
            return _mapper.Map<VendaDTO>(venda);
        }

        public async Task<PagedList<VendaDTO>> GetPagedVendasAsync(VendasParameters parameters)
        {
            var vendas = await _uof.VendaRepository
                .GetAllIncludePagedAsync(null, v => v.Data, parameters.PageNumber, parameters.PageSize);
            return _mapper.Map<PagedList<VendaDTO>>(vendas);
        }

        public async Task<IEnumerable<VendaDTO>> GetVendasByDataAsync(VendasFiltroData parameters)
        {
            var vendas = await _uof.VendaRepository.GetAllDay(parameters);
            return _mapper.Map<IEnumerable<VendaDTO>>(vendas);
        }

        public async Task<IEnumerable<VendaDTO>> GetVendasByMesAsync(VendaFiltroMes parameters)
        {
            var vendas = await _uof.VendaRepository.GetAllMonth(parameters);
            return _mapper.Map<IEnumerable<VendaDTO>>(vendas);
        }

        public async Task<IEnumerable<VendaDTO>> GetVendasByPagamentoAsync(VendaFiltroPagamento parameters)
        {
            var vendas = await _uof.VendaRepository.GetAllPayment(parameters);
            return _mapper.Map<IEnumerable<VendaDTO>>(vendas);
        }

        public async Task<IEnumerable<VendaDTO>> GetVendasByDiasAsync(VendasFiltroDias parameters)
        {
            var vendas = await _uof.VendaRepository.GetAllDays(parameters);
            return _mapper.Map<IEnumerable<VendaDTO>>(vendas);
        }

        public async Task<(bool Success, string? ErrorMessage, VendaDTO? Venda)> RealizarVendaAsync(string nomeMetodoPagamento)
        {
            if (_cache.TryGetValue(CacheKey, out Carrinho? carrinho))
            {
                if (carrinho == null)
                    return (false, "Não há nenhum carrinho ativo", null);

                if (!carrinho.Produtos.Any())
                    return (false, "Adicione produtos no carrinho antes de finalizar", null);

                var metodoPagamento = await _uof.MetodoPagamentoRepository.GetAsync(mp => mp.Nome.ToUpper() == nomeMetodoPagamento.ToUpper());
                if (metodoPagamento is null)
                    return (false, "Insira um metodo de pagamento válido", null);

                var venda = new Venda
                {
                    Carrinho = carrinho,
                    Data = DateTime.UtcNow,
                    MetodoPagamento = metodoPagamento
                };

                carrinho.Situacao = Models.Enum.StatusCarrinho.Fechado;

                foreach (var produtoCarrinho in carrinho.Produtos)
                {
                    var produto = produtoCarrinho.Produto;
                    produto.Estoque -= produtoCarrinho.Quantidade;
                    _uof.ProdutoRepository.Update(produto);
                }

                _uof.CarrinhoRepository.Update(carrinho);
                _uof.VendaRepository.Create(venda);
                await _uof.CommitAsync();

                _cache.Remove(CacheKey);

                return (true, null, _mapper.Map<VendaDTO>(venda));
            }

            return (false, "Inicie um novo atendimento", null);
        }
    }
}
