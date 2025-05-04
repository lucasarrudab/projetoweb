using ApiPDV.DTOs;
using ApiPDV.Models;
using ApiPDV.Repositories;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System.Runtime.CompilerServices;

namespace ApiPDV.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CarrinhoController : Controller
    {

        private readonly IUnitOfWork _uof;
        private readonly IMapper _mapper;
        private readonly IMemoryCache _cache;

        public CarrinhoController(IUnitOfWork uof, IMapper mapper, IMemoryCache cache)
        {
            _uof = uof;
            _mapper = mapper;
            _cache = cache;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CarrinhoDTO>>> GetAllAsync()
        {
            var carrinhos = await _uof.CarrinhoRepository.GetAllAsync();
            var carrinhosDto = _mapper.Map<IEnumerable<CarrinhoDTO>>(carrinhos);
            return Ok(carrinhosDto);
        }

        [HttpPost]
        public async Task<ActionResult<CarrinhoDTO>> Create()
        {
            var carrinho = new Carrinho { Situacao = Models.Enum.StatusCarrinho.Aberto };
            var carrinhoCriado = _uof.CarrinhoRepository.Create(carrinho);
            await _uof.CommitAsync();
            var carrinhoDto = _mapper.Map<CarrinhoDTO>(carrinhoCriado);
            return Ok(carrinhoDto);
        }

        [HttpPost("{id:int}")]
        public async Task<ActionResult<CarrinhoDTO>> AddToCart(int id, [FromBody]string barCode)
        {
            var produto = await _uof.ProdutoRepository.FindByBarCode(barCode);
            var carrinho = await _uof.CarrinhoRepository.GetAsync(c => c.Id == id);
            if(carrinho == null)
            {
                return BadRequest();
            }
            var produtoCarrinho = new ProdutoCarrinho { Produto = produto, Quantidade =1, ValorUnitario = produto.Preco };
            carrinho.Produtos.Add(produtoCarrinho);
            await _uof.CommitAsync();
            var carrinhoDto = _mapper.Map<CarrinhoDTO>(carrinho);
            return Ok(carrinhoDto);



        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult<CarrinhoDTO>> Delete(int id)
        {
            var carrinho = await _uof.CarrinhoRepository.GetAsync(c => c.Id == id);
            var carrinhoExcluido = _uof.CarrinhoRepository.Delete(carrinho);
            await _uof.CommitAsync();
            var carrinhoExcluidoDto = _mapper.Map<CarrinhoDTO>(carrinhoExcluido);
            return Ok(carrinhoExcluidoDto);
        }
    }
}
