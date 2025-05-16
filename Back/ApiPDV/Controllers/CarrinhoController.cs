using ApiPDV.DTOs;
using ApiPDV.Models;
using ApiPDV.Repositories;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
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

        private const string CacheKey = "CacheCarrinho";

        public CarrinhoController(IUnitOfWork uof, IMapper mapper, IMemoryCache cache)
        {
            _uof = uof;
            _mapper = mapper;
            _cache = cache;
        }


        /// <summary>
        /// Retorna todos os carrinhos
        /// </summary>
        /// <remarks>
        /// Acesso restrito ao perfil: <b>Admin ou Gerente</b>.
        /// </remarks>
        [HttpGet]
        [Authorize(Policy = "Management")]
        public async Task<ActionResult<IEnumerable<CarrinhoDTO>>> GetAllAsync()
        {
            var carrinhos = await _uof.CarrinhoRepository.GetAllAsync();
            var carrinhosDto = _mapper.Map<IEnumerable<CarrinhoDTO>>(carrinhos);
            return Ok(carrinhosDto);
        }

        /// <summary>
        /// Retorna um carringo por id
        /// </summary>
        /// <remarks>
        /// Acesso restrito ao perfil: <b>Admin ou Gerente</b>.
        /// </remarks>
        [HttpGet("{id:int}")]
        [Authorize(Policy = "Management")]
        public async Task<ActionResult<CarrinhoDTO>> Get(int id)
        {
            var carrinho = await _uof.CarrinhoRepository.GetAsync(id);
            if (carrinho == null)
            {
                return NotFound();
            }
            var carrinhoDto = _mapper.Map<CarrinhoDTO>(carrinho);
            return Ok(carrinhoDto);
        }

        /// <summary>
        /// Cria um novo carrinho vazio e salva no cache
        /// </summary>
        /// <remarks>
        /// Acesso somente ao logar.
        /// </remarks>
        [HttpPost]
        [Authorize(Policy = "All")]
        public async Task<ActionResult<CarrinhoDTO>> Create()
        {
            var carrinho = new Carrinho { Situacao = Models.Enum.StatusCarrinho.Aberto };
            var carrinhoCriado = _uof.CarrinhoRepository.Create(carrinho);
            await _uof.CommitAsync();
            var carrinhoDto = _mapper.Map<CarrinhoDTO>(carrinhoCriado);
            SetCache<Carrinho>(CacheKey, carrinho);
            return Ok(carrinhoDto);
        }

        /// <summary>
        /// Adiciona um produto ao carrinho salvo no cache
        /// </summary>
        /// <remarks>
        /// <param name="code">Id ou codigo de barras</param> 
        /// Acesso somente ao logar.
        /// </remarks>
        [HttpPut]
        [Authorize(Policy = "All")]
        public async Task<ActionResult<CarrinhoDTO>> AdicionarAoCarrinho([FromBody] int code)
        {
            Produto produto;
           
            if (code.ToString().Length >= 7)
            {
                var codBarras = code.ToString();
               produto = await _uof.ProdutoRepository.GetNoTrackingAsync(p => p.Codigo.Equals(codBarras));
            }
            else
            {
                produto = await _uof.ProdutoRepository.GetNoTrackingAsync(p => p.Id == code);
            }
            if (produto is null)
                return NotFound("Produto não encontrado");
            
            if (_cache.TryGetValue(CacheKey, out Carrinho? carrinho))
            {
                if (carrinho == null)
                {
                    return NotFound("Carrinho não encontrado!!");
                }

                var produtoCarrinho = carrinho.Produtos.Where(p => p.ProdutoId == produto.Id).FirstOrDefault();
                if (produtoCarrinho != null)
                {
                    produtoCarrinho.Quantidade++;

                }
                else
                {
                    produtoCarrinho = new ProdutoCarrinho { Produto = produto, Quantidade = 1, ValorUnitario = produto.Preco };
                    carrinho.Produtos.Add(produtoCarrinho);
                }
                _uof.CarrinhoRepository.Update(carrinho);
                SetCache<Carrinho>(CacheKey, carrinho);
                await _uof.CommitAsync();
            }
            else
            {
                return BadRequest();
            }
            var carrinhoDto = _mapper.Map<CarrinhoDTO>(carrinho);
            
            return Ok(carrinhoDto);



        }

        /// <summary>
        /// altera a quantidade de um produto do carrinho no cache
        /// </summary>
        /// <param name="id">id do produtocarrinho</param>
        /// <param name="quantidade">Nova quantidade</param>
        /// <remarks>
        /// Acesso somente ao logar.
        /// </remarks>
        [HttpPut("/quantiadeproduto/{id:int}")]
        [Authorize(Policy = "All")]
        public async Task<ActionResult<CarrinhoDTO>> AlterarQuantidade(int id, [FromBody] int quantidade)
        {
            if(quantidade < 0)
            {
                return BadRequest("A quantidade precisa ser maior que 0");
            }
            if (_cache.TryGetValue(CacheKey, out Carrinho? carrinho))
            {
                if(carrinho is null)
                {
                    return NotFound("Carrinho não encontrado");
                }
                var produtoCarrinho = carrinho.Produtos.FirstOrDefault(pc => pc.Id == id);
                
                if(produtoCarrinho is null)
                {
                    return NotFound("Produto não encontrado");
                }
                if (quantidade == 0)
                {
                    carrinho.Produtos.Remove(produtoCarrinho);
                    _uof.ProdutoCarrinhoRepository.Delete(produtoCarrinho);
                }
                else
                {
                    produtoCarrinho.Quantidade = quantidade;
                }
                _uof.CarrinhoRepository.Update(carrinho);
                SetCache<Carrinho>(CacheKey, carrinho);
                await _uof.CommitAsync();
            }
            else
            {
                return BadRequest();
            }
            var carrinhoDto = _mapper.Map<CarrinhoDTO>(carrinho);

            return Ok(carrinhoDto);
        }

        /// <summary>
        /// Deleta um carrinho
        /// </summary>
        /// <param name="id">id do carrinho</param>
        /// <remarks>
        /// Acesso restrito ao perfil: <b>Admin</b>.
        /// </remarks>
        [HttpDelete("{id:int}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<ActionResult<CarrinhoDTO>> Delete(int id)
        {
            var carrinho = await _uof.CarrinhoRepository.GetAsync(c => c.Id == id);
            var carrinhoExcluido = _uof.CarrinhoRepository.Delete(carrinho);
            await _uof.CommitAsync();
            var carrinhoExcluidoDto = _mapper.Map<CarrinhoDTO>(carrinhoExcluido);
            return Ok(carrinhoExcluidoDto);
        }

        public void SetCache<T>(string key, T data)
        {
            var cacheOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10),
                SlidingExpiration = TimeSpan.FromMinutes(5),
                Priority = CacheItemPriority.High
            };
            _cache.Set(key, data, cacheOptions);
        }


    }
}
