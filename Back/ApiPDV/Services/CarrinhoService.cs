using ApiPDV.DTOs;
using ApiPDV.Models;
using ApiPDV.Repositories;
using AutoMapper;
using Microsoft.Extensions.Caching.Memory;

namespace ApiPDV.Services
{
    public class CarrinhoService : ICarrinhoService
    {
        private readonly IUnitOfWork _uof;
        private readonly IMapper _mapper;
        private readonly IMemoryCache _cache;

        private const string CacheKey = "CacheCarrinho";

        public CarrinhoService(IUnitOfWork uof, IMapper mapper, IMemoryCache cache)
        {
            _uof = uof;
            _mapper = mapper;
            _cache = cache;
        }

        public async Task<CarrinhoDTO> CriarCarrinhoAsync()
        {
            var carrinho = new Carrinho
            {
                Situacao = Models.Enum.StatusCarrinho.Aberto,
                Produtos = new List<ProdutoCarrinho>()
            };

            var carrinhoCriado = _uof.CarrinhoRepository.Create(carrinho);
            await _uof.CommitAsync();

            SetCache(CacheKey, carrinhoCriado);
            return _mapper.Map<CarrinhoDTO>(carrinhoCriado);
        }

        public async Task<CarrinhoDTO?> AdicionarProdutoAoCarrinhoAsync(string code)
        {
            Produto produto = await FindProduto(code);
            if (produto is null)
                return null;

            if (!_cache.TryGetValue(CacheKey, out Carrinho? carrinho) || carrinho == null)
                throw new InvalidOperationException("Carrinho não encontrado. Crie um novo carrinho primeiro.");

            var produtoCarrinho = carrinho.Produtos.FirstOrDefault(p => p.ProdutoId == produto.Id);

            if (produtoCarrinho != null)
            {
                produtoCarrinho.Quantidade++;
            }
            else
            {
                produtoCarrinho = new ProdutoCarrinho
                {
                    ProdutoId = produto.Id,
                    Produto = produto,
                    Quantidade = 1,
                    ValorUnitario = produto.Preco
                };
                carrinho.Produtos.Add(produtoCarrinho);
            }

            _uof.CarrinhoRepository.Update(carrinho);
            SetCache(CacheKey, carrinho);
            await _uof.CommitAsync();

            return _mapper.Map<CarrinhoDTO>(carrinho);
        }

        public async Task<CarrinhoDTO> AlterarQuantidadeProdutoAsync(int produtoCarrinhoId, int quantidade)
        {
            if (!_cache.TryGetValue(CacheKey, out Carrinho? carrinho) || carrinho == null)
                throw new InvalidOperationException("Carrinho não encontrado no cache.");

            var produtoCarrinho = carrinho.Produtos.FirstOrDefault(pc => pc.Id == produtoCarrinhoId);
            if (produtoCarrinho == null)
                throw new KeyNotFoundException("Produto não encontrado no carrinho.");

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
            SetCache(CacheKey, carrinho);
            await _uof.CommitAsync();

            return _mapper.Map<CarrinhoDTO>(carrinho);
        }

        private async Task<Produto?> FindProduto(string code)
        {
             if (code.ToString().Length >= 7)
            {
                var codBarras = code.ToString();
                return  await _uof.ProdutoRepository.GetNoTrackingAsync(p => p.Codigo.Equals(codBarras));
            }
            else
            {
                return await _uof.ProdutoRepository.GetNoTrackingAsync(p => p.Id.ToString() == code);
            }
        }

        private void SetCache<T>(string key, T data)
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
