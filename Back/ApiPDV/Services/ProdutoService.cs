using ApiPDV.DTOs.Request;
using ApiPDV.DTOs.Response;
using ApiPDV.Models;
using ApiPDV.Pagination;
using ApiPDV.Repositories;
using AutoMapper;

namespace ApiPDV.Services
{
    public class ProdutoService : IProdutoService
    {
        private readonly IUnitOfWork _uof;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _env;

        public ProdutoService(IUnitOfWork uof, IMapper mapper, IWebHostEnvironment env)
        {
            _uof = uof;
            _mapper = mapper;
            _env = env;
        }

        public async Task<IEnumerable<ProdutoResponseDTO>> ListarTodosAsync()
        {
            var produtos = await _uof.ProdutoRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<ProdutoResponseDTO>>(produtos);
        }

        public async Task<PagedList<Produto>> ListarPaginadoAsync(ProdutosParameters parametros)
        {
            return await _uof.ProdutoRepository.GetPagedAsync(null, p => p.Nome, parametros.PageNumber, parametros.PageSize);
            
        }

        public async Task<PagedList<Produto>> FiltrarPorPrecoPaginadoAsync(ProdutosFiltroPreco filtro)
        {
            return await _uof.ProdutoRepository.GetAllPagFiltroPrecoAsync(filtro);
            
        }

        public async Task<PagedList<Produto>> FiltrarPorNomePaginadoAsync(ProdutosFiltroNome filtro)
        {
            return await _uof.ProdutoRepository.GetPagedAsync(p => p.Nome.Contains(filtro.Nome), p => p.Nome, filtro.PageNumber, filtro.PageSize);
            
        }

        public async Task<ProdutoResponseDTO?> ObterPorIdAsync(int id)
        {
            var produto = await _uof.ProdutoRepository.GetAsync(p => p.Id == id);
            return produto is null ? null : _mapper.Map<ProdutoResponseDTO>(produto);
        }

        public async Task<ProdutoResponseDTO> CriarAsync(ProdutoRequestDTO dto)
        {
            var produto = _mapper.Map<Produto>(dto);
            produto.DataCadastro = DateTime.UtcNow;

            if (dto.Imagem != null && dto.Imagem.Length > 0)
                produto.URLImagem = await SalvarImagemAsync(dto.Imagem);

            _uof.ProdutoRepository.Create(produto);
            await _uof.CommitAsync();

            return _mapper.Map<ProdutoResponseDTO>(produto);
        }

        public async Task<ProdutoResponseDTO?> AtualizarAsync(int id, ProdutoRequestDTO dto)
        {
            var produto = await _uof.ProdutoRepository.GetAsync(p => p.Id == id);
            if (produto is null) return null;

            _mapper.Map(dto, produto);

            if (dto.Imagem != null && dto.Imagem.Length > 0)
                produto.URLImagem = await SalvarImagemAsync(dto.Imagem);

            _uof.ProdutoRepository.Update(produto);
            await _uof.CommitAsync();

            return _mapper.Map<ProdutoResponseDTO>(produto);
        }

        public async Task<ProdutoResponseDTO?> AdicionarEstoqueAsync(int id, int quantidade)
        {
            if (quantidade <= 0) return null;

            var produto = await _uof.ProdutoRepository.GetAsync(p => p.Id == id);
            if (produto is null) return null;

            produto.Estoque += quantidade;
            _uof.ProdutoRepository.Update(produto);
            await _uof.CommitAsync();

            return _mapper.Map<ProdutoResponseDTO>(produto);
        }

        public async Task<ProdutoResponseDTO?> ExcluirAsync(int id)
        {
            var produto = await _uof.ProdutoRepository.GetAsync(p => p.Id == id);
            if (produto is null) return null;

            _uof.ProdutoRepository.Delete(produto);
            await _uof.CommitAsync();

            return _mapper.Map<ProdutoResponseDTO>(produto);
        }

        public async Task<string> SalvarImagemAsync(IFormFile imagem)
        {
            var pasta = Path.Combine(_env.WebRootPath, "images/produtos");
            Directory.CreateDirectory(pasta);

            var nomeArquivo = $"{Guid.NewGuid()}{Path.GetExtension(imagem.FileName)}";
            var caminho = Path.Combine(pasta, nomeArquivo);

            using var stream = new FileStream(caminho, FileMode.Create);
            await imagem.CopyToAsync(stream);

            return $"/images/produtos/{nomeArquivo}";
        }


    }
}
