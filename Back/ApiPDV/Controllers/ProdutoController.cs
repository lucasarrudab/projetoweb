using ApiPDV.DTOs.Request;
using ApiPDV.DTOs.Response;
using ApiPDV.Models;
using ApiPDV.Pagination;
using ApiPDV.Services;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace ApiPDV.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ProdutoController : Controller
    {
        private readonly IProdutoService _produtoService;
        private readonly IMapper _mapper;

        public ProdutoController(IProdutoService produtoService, IMapper mapper)
        {
            _produtoService = produtoService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProdutoResponseDTO>>> GetAllAsync()
        {
            var produtos = await _produtoService.ListarTodosAsync();
            var produtosDto = _mapper.Map<IEnumerable<ProdutoResponseDTO>>(produtos);
            return Ok(produtosDto);
        }

        [HttpGet("pagination")]
        public async Task<ActionResult<IEnumerable<ProdutoResponseDTO>>> GetAllAsync([FromQuery] ProdutosParameters parametros)
        {
            var produtos = await _produtoService.ListarPaginadoAsync(parametros);
            return ObterProdutos(produtos);
        }

        [HttpGet("filter/pagination/preco")]
        public async Task<ActionResult<IEnumerable<ProdutoResponseDTO>>> GetAllAsync([FromQuery] ProdutosFiltroPreco filtro)
        {
            var produtos = await _produtoService.FiltrarPorPrecoPaginadoAsync(filtro);
            return ObterProdutos(produtos);
        }

        [HttpGet("filter/pagination/nome")]
        public async Task<ActionResult<IEnumerable<ProdutoResponseDTO>>> GetAllAsync([FromQuery] ProdutosFiltroNome filtro)
        {
            var produtos = await _produtoService.FiltrarPorNomePaginadoAsync(filtro);
            return ObterProdutos(produtos);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<ProdutoResponseDTO>> GetByIdAsync(int id)
        {
            var produto = await _produtoService.ObterPorIdAsync(id);
            if (produto == null)
                return NotFound("Produto não encontrado");

            var produtoDto = _mapper.Map<ProdutoResponseDTO>(produto);
            return Ok(produtoDto);
        }

        [HttpPost]
        [Authorize(Policy = "Management")]
        public async Task<ActionResult<ProdutoResponseDTO>> Create([FromForm] ProdutoRequestDTO dto)
        {
            var produto = await _produtoService.CriarAsync(dto);
            var produtoDto = _mapper.Map<ProdutoResponseDTO>(produto);
            return Ok(produtoDto);
        }

        [HttpPut("{id:int}")]
        [Authorize(Policy = "Management")]
        public async Task<ActionResult<ProdutoResponseDTO>> Update([FromForm] ProdutoRequestDTO dto, int id)
        {
            var produto = await _produtoService.AtualizarAsync(id, dto);
            if (produto == null)
                return NotFound("Produto não encontrado");

            var produtoDto = _mapper.Map<ProdutoResponseDTO>(produto);
            return Ok(produtoDto);
        }

        [HttpPut("estoque/{id:int}")]
        public async Task<ActionResult<ProdutoResponseDTO>> AddEstoque(int id, [FromBody] int quantidade)
        {
            var produto = await _produtoService.AdicionarEstoqueAsync(id, quantidade);
            if (produto == null)
                return NotFound("Produto não encontrado");

            var produtoDto = _mapper.Map<ProdutoResponseDTO>(produto);
            return Ok(produtoDto);
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult<ProdutoResponseDTO>> Delete(int id)
        {
            var produto = await _produtoService.ExcluirAsync(id);
            if (produto == null)
                return NotFound("Produto não encontrado");

            var produtoDto = _mapper.Map<ProdutoResponseDTO>(produto);
            return Ok(produtoDto);
        }

        
        private ActionResult<IEnumerable<ProdutoResponseDTO>> ObterProdutos(PagedList<Produto> produtos)
        {
            var metadata = new
            {
                produtos.TotalCount,
                produtos.PageSize,
                produtos.CurrentPage,
                produtos.TotalPages,
                produtos.HasNext,
                produtos.HasPrevious,
            };

            Response.Headers.Append("X-Pagination", JsonConvert.SerializeObject(metadata));
            var produtosDto = _mapper.Map<IEnumerable<ProdutoResponseDTO>>(produtos);
            return Ok(produtosDto);
        }
    }
}
