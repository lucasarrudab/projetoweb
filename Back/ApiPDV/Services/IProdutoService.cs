using ApiPDV.DTOs.Request;
using ApiPDV.DTOs.Response;
using ApiPDV.Models;
using ApiPDV.Pagination;

namespace ApiPDV.Services;

public interface IProdutoService
{
    Task<IEnumerable<ProdutoResponseDTO>> ListarTodosAsync();
    Task<PagedList<Produto>> ListarPaginadoAsync(ProdutosParameters parametros);
    Task<PagedList<Produto>> FiltrarPorPrecoPaginadoAsync(ProdutosFiltroPreco filtro);
    Task<PagedList<Produto>> FiltrarPorNomePaginadoAsync(ProdutosFiltroNome filtro);
    Task<ProdutoResponseDTO?> ObterPorIdAsync(int id);
    Task<ProdutoResponseDTO> CriarAsync(ProdutoRequestDTO dto);
    Task<ProdutoResponseDTO?> AtualizarAsync(int id, ProdutoRequestDTO dto);
    Task<ProdutoResponseDTO?> ExcluirAsync(int id);
    Task<ProdutoResponseDTO?> AdicionarEstoqueAsync(int id, int quantidade);
    Task<string> SalvarImagemAsync(IFormFile imagem);
}