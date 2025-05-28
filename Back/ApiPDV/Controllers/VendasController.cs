using ApiPDV.DTOs;
using ApiPDV.Models;
using ApiPDV.Pagination;
using ApiPDV.Repositories;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
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

        /// <summary>
        /// Retorna todas as vendas
        /// </summary>
        /// <remarks>
        /// Acesso somente ao logar
        /// </remarks>
        [HttpGet]
        [Authorize(Policy = "All")]
        public async Task<ActionResult<IEnumerable<VendaDTO>>> GetAllVendas()
        {
            var vendas = await _uof.VendaRepository.GetAllIncludeAsync();
            var vendasDto = _mapper.Map<IEnumerable<VendaDTO>>(vendas);
            return Ok(vendasDto);
        }

        /// <summary>
        /// Retorna uma venda pelo id
        /// </summary>
        /// <remarks>
        /// Acesso somente ao logar.
        /// </remarks>
        [HttpGet("{id:int}")]
        [Authorize(Policy = "All")]
        public async Task<ActionResult<VendaDTO>> GetVenda(int id)
        {
            var venda = await _uof.VendaRepository.GetIncludeAsync(id);
            var vendaDto = _mapper.Map<VendaDTO>(venda);
            return Ok(vendaDto);

        }

        /// <summary>
        /// Retorna todas as vendas com paginação
        /// </summary>
        /// <remarks>
        /// Acesso somente ao logar.
        /// </remarks>
        [HttpGet("pagination")]
        [Authorize]
        public async Task<ActionResult<PagedList<VendaDTO>>> GetAllPaged([FromQuery] VendasParameters vendasParameters)
        {
            var vendas = await _uof.VendaRepository
                .GetAllIncludePagedAsync(null, v => v.Data, vendasParameters.PageNumber, vendasParameters.PageSize);
            var vendasDto = _mapper.Map<PagedList<VendaDTO>>(vendas);
            return Ok(vendasDto);
        }

        /// <summary>
        /// Retorna todas as vendas com paginação e filtro por data
        /// </summary>
        /// <remarks>
        /// Acesso somente ao logar.
        /// </remarks>
        [HttpGet("filter/pagination/date")]
        [Authorize]
        public async Task<ActionResult<PagedList<VendaDTO>>> GetAllPaged([FromQuery] VendasFiltroData vendasParameters)
        {
            var vendas = await _uof.VendaRepository.GetAllDay(vendasParameters);
            var vendasDto = _mapper.Map<IEnumerable<VendaDTO>>(vendas);
            return Ok (vendasDto);

        }

        /// <summary>
        /// Retorna todas as vendas com paginação e filtro por mês
        /// </summary>
        /// <remarks>
        /// Acesso somente ao logar.
        /// </remarks>
        [HttpGet("filter/pagination/month")]
        [Authorize]
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

        /// <summary>
        /// Retorna todas as vendas com paginação e filtro por tipo de pagamento
        /// </summary>
        /// <remarks>
        /// Acesso somente ao logar.
        /// </remarks>
        [HttpGet("filter/pagination/payment")]
        [Authorize]
        public async Task<ActionResult<PagedList<VendaDTO>>> GetAllPagedPayment([FromQuery] VendaFiltroPagamento vendasParameters)
        {
            var vendas = await _uof.VendaRepository.GetAllPayment(vendasParameters);
            var vendasDto = _mapper.Map<IEnumerable<VendaDTO>>(vendas);
            return Ok(vendasDto);

        }

        /// <summary>
        /// Retorna todas as vendas com paginação e filtro nos ultimos X dias
        /// </summary>
        /// <remarks>
        /// Acesso somente ao logar.
        /// </remarks>
        [HttpGet("filter/pagination/days")]
        [Authorize]
        public async Task<ActionResult<PagedList<VendaDTO>>> GetAllPageddays([FromQuery] VendasFiltroDias vendasParameters)
        {
            var vendas = await _uof.VendaRepository.GetAllDays(vendasParameters);
            var vendasDto = _mapper.Map<IEnumerable<VendaDTO>>(vendas);
            return Ok(vendasDto);

        }


        /// <summary>
        /// Realiza a venda do carrinho salvo no cache
        /// </summary>
        /// <param name="nomeMetodoPagamento">Dinheiro, debito, credito ou pix</param>
        /// <remarks>
        /// Acesso somente ao logar.
        /// </remarks>
        [HttpPost]
        [Authorize]
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
                var venda = new Venda { Carrinho = carrinho, Data = DateTime.UtcNow,MetodoPagamento = metodoPagamento };
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
