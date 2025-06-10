using ApiPDV.DTOs;

namespace ApiPDV.Services;

public interface ICarrinhoService
{
    Task<CarrinhoDTO> CriarCarrinhoAsync();
    Task<CarrinhoDTO> AdicionarProdutoAoCarrinhoAsync(string code);
    Task<CarrinhoDTO> AlterarQuantidadeProdutoAsync(int produtoCarrinhoId, int quantidade);
}
