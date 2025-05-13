using ApiPDV.DTOs;
using ApiPDV.Models;
using ApiPDV.Pagination;
using ApiPDV.Repositories;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System.Runtime.CompilerServices;

namespace ApiPDV.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class VendasController : Controller
    {

        private readonly IMapper _mapper;
        private readonly IUnitOfWork _uof;
        private readonly IMemoryCache _cache;

        private const string CacheKey = "CacheCarrinho";

        public VendasController(IMapper mapper, IUnitOfWork uof, IMemoryCache memoryCache)
        {
            _mapper = mapper;
            _uof = uof;
            _cache = memoryCache;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<VendaDTO>>> GetAllVendas()
        {
            var vendas = await _uof.VendaRepository.GetAllIncludeAsync();
            var vendasDto = _mapper.Map<IEnumerable<VendaDTO>>(vendas);
            return Ok(vendasDto);
        }
        /// <summary>
        /// Retorna todos os produtos disponíveis.
        /// </summary>
        [HttpGet("{id:int}")]
        public async Task<ActionResult<VendaDTO>> GetVenda(int id)
        {
            var venda = await _uof.VendaRepository.GetIncludeAsync(id);
            var vendaDto = _mapper.Map<VendaDTO>(venda);
            return Ok(vendaDto);

        }

        [HttpGet("pagination")]
        public async Task<ActionResult<PagedList<VendaDTO>>> GetAllPaged([FromQuery] VendasParameters vendasParameters)
        {
            var vendas = await _uof.VendaRepository
                .GetAllIncludePagedAsync(null, v => v.Data, vendasParameters.PageNumber, vendasParameters.PageSize);
            var vendasDto = _mapper.Map<PagedList<VendaDTO>>(vendas);
            return Ok(vendasDto);
        }

        [HttpGet("filter/pagination/date")]
        public async Task<ActionResult<PagedList<VendaDTO>>> GetAllPaged([FromQuery] VendasFiltroData vendasParameters)
        {
            var vendas = await _uof.VendaRepository.GetAllDay(vendasParameters);
            var vendasDto = _mapper.Map<IEnumerable<VendaDTO>>(vendas);
            return Ok (vendasDto);

        }

        [HttpGet("filter/pagination/month")]
        public async Task<ActionResult<PagedList<VendaDTO>>> GetAllPagedMonth([FromQuery] VendaFiltroMes vendasParameters)
        {
            var vendas = await _uof.VendaRepository.GetAllMonth(vendasParameters);
            if(!vendas.Any())
            {
                return NotFound("Não foram encontradas vendas nesse filtro");
            }
            var vendasDto = _mapper.Map<IEnumerable<VendaDTO>>(vendas);
            return Ok(vendasDto);

        }

        [HttpGet("filter/pagination/payment")]
        public async Task<ActionResult<PagedList<VendaDTO>>> GetAllPagedPayment([FromQuery] VendaFiltroPagamento vendasParameters)
        {
            var vendas = await _uof.VendaRepository.GetAllPayment(vendasParameters);
            var vendasDto = _mapper.Map<IEnumerable<VendaDTO>>(vendas);
            return Ok(vendasDto);

        }



        [HttpPost]
        public async Task<ActionResult<VendaDTO>> RealizarVenda([FromBody]string nomeMetodoPagamento)
        {
           
            if (_cache.TryGetValue(CacheKey, out Carrinho? carrinho))
            {
                if (carrinho == null)
                {
                    return BadRequest("Não há nenhum carrinho ativo");
                }
                if (!carrinho.Produtos.Any())
                {
                    return BadRequest("Adicione produtos no carrinho antes de finalizar");
                }
                var metodoPagamento = await _uof.MetodoPagamentoRepository.GetAsync(mp => mp.Nome.ToUpper().Equals(nomeMetodoPagamento.ToUpper()));
                if (metodoPagamento is null)
                    return NotFound("Insira um metodo de pagamento válido");
                var venda = new Venda { Carrinho = carrinho, Data = DateTime.Now,MetodoPagamento = metodoPagamento };
                carrinho.Situacao = Models.Enum.StatusCarrinho.Fechado;
                foreach(var produtoCarrinho in carrinho.Produtos)
                {
                    var produto = produtoCarrinho.Produto;
                    produto.Estoque -= produtoCarrinho.Quantidade;
                    _uof.ProdutoRepository.Update(produto);
                }
                _uof.CarrinhoRepository.Update(carrinho);
                _uof.VendaRepository.Create(venda);
                await _uof.CommitAsync();
                _cache.Remove(CacheKey);
                var vendaDto = _mapper.Map<VendaDTO>(venda);
                return Ok(vendaDto);
            }
            return BadRequest("Inicie um novo atendimento");


        }

       
    }
}
