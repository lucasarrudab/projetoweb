using ApiPDV.DTOs.Mapping;
using ApiPDV.DTOs.Request;
using ApiPDV.DTOs.Response;
using ApiPDV.Models;
using ApiPDV.Pagination;
using ApiPDV.Repositories;
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
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _uof;

        public ProdutoController(IUnitOfWork uof, IMapper mapper)
        {
            _uof = uof;
            _mapper = mapper;

        }

        /// <summary>
        /// Retorna todos os produtos disponíveis.
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProdutoResponseDTO>>> GetAllAsync()
        {
            var produtos = await _uof.ProdutoRepository.GetAllAsync();
            var produtosDto = _mapper.Map<IEnumerable<ProdutoResponseDTO>>(produtos);
            return Ok(produtosDto);
        }

        /// <summary>
        /// Retorna todos os produtos com paginação
        /// </summary>
        [HttpGet("pagination")]
        public async Task<ActionResult<IEnumerable<ProdutoResponseDTO>>> GetAllAsync([FromQuery] ProdutosParameters produtosParameters)
        {
            var produtos = await _uof.ProdutoRepository.GetPagedAsync(null,p => p.Nome, produtosParameters.PageNumber, produtosParameters.PageSize);
            
            return ObterProdutos(produtos);


        }

        /// <summary>
        /// Retorna todos os produtos com paginação filtrando por preço
        /// </summary>
        [HttpGet("filter/pagination/preco")]
        public async Task<ActionResult<IEnumerable<ProdutoResponseDTO>>> GetAllAsync([FromQuery] ProdutosFiltroPreco produtosParameters)
        {
            var produtos = await _uof.ProdutoRepository.GetAllPagFiltroPrecoAsync(produtosParameters);

            return ObterProdutos(produtos);

        }

        /// <summary>
        /// Retorna todos os produtos com paginação filtrando por nome
        /// </summary>
        [HttpGet("filter/pagination/nome")]
        public async Task<ActionResult<IEnumerable<ProdutoResponseDTO>>> GetAllAsync([FromQuery] ProdutosFiltroNome produtosParameters)
        {
            var produtos = await _uof.ProdutoRepository.GetPagedAsync(p => p.Nome.Contains(produtosParameters.Nome), p => p.Nome, produtosParameters.PageNumber, produtosParameters.PageSize);

            return ObterProdutos(produtos);
        }

        /// <summary>
        /// Obtem o produto pelo seu identificador id
        /// </summary>
        
        [HttpGet("{id:int}")]
        public async Task<ActionResult<ProdutoResponseDTO>> GetByIdAsync(int id)
        {
          
            var produto = await _uof.ProdutoRepository.GetAsync(p => p.Id == id);
    
            if(produto == null)
            {
                return NotFound("Produto não encontrado");
            }
            var produtoDto = _mapper.Map<ProdutoResponseDTO>(produto);
            return Ok(produtoDto);


        }

        /// <summary>
        /// Cria um novo produto
        /// </summary>
        /// <remarks>
        /// Acesso restrito ao perfil: <b>Admin ou Gerente</b>.
        /// </remarks>
        [HttpPost]
        [Authorize(Policy = "Management")]
        public async Task<ActionResult<ProdutoResponseDTO>> Create([FromBody] ProdutoRequestDTO produtoDto)
        {
            if (produtoDto is null)
                return BadRequest();

            //var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "imagens");
            //Directory.CreateDirectory(folderPath);

            //var fileName = Guid.NewGuid().ToString() + Path.GetExtension(produtoDto.Imagem.FileName);
            //var filePath = Path.Combine(folderPath, fileName);

            //using (var stream = new FileStream(filePath, FileMode.Create))
            //{
            //    await produtoDto.Imagem.CopyToAsync(stream);

            //}
            //var url = $"{Request.Scheme}://{Request.Host}/imagens/{fileName}";
            var produto = _mapper.Map<Produto>(produtoDto);
            //produto.URLImagem = url;
            produto.DataCadastro = DateTime.UtcNow;
            var novoProduto = _uof.ProdutoRepository.Create(produto);
            await _uof.CommitAsync();
            var novoProdutoDto = _mapper.Map<ProdutoResponseDTO>(novoProduto);
            return Ok(novoProdutoDto);
        }

        /// <summary>
        /// Atualiza um produto
        /// </summary>
        /// <remarks>
        /// Acesso restrito ao perfil: <b>Admin ou Gerente</b>.
        /// </remarks>
        [HttpPut("{id:int}")]
        [Authorize(Policy = "Management")]
        public async Task<ActionResult<ProdutoResponseDTO>> Put(ProdutoRequestDTO produtoDto, int id)
        {
            var produtoExistente = await _uof.ProdutoRepository.GetAsync(p => p.Id == id);
        if (produtoExistente is null)
            return NotFound("Produto não encontrado.");

        _mapper.Map(produtoDto, produtoExistente); // Atualiza os campos do produto existente
        var produtoAtualizado = _uof.ProdutoRepository.Update(produtoExistente);
        await _uof.CommitAsync();

        var produtoAtualizadoDto = _mapper.Map<ProdutoResponseDTO>(produtoAtualizado);
        return Ok(produtoAtualizadoDto);
}

        [HttpPut("estoque/{id:int}")]
        public async Task<ActionResult<ProdutoResponseDTO>> AddEstoque(int id, [FromBody]int quantidade)
        {
            var produto = await _uof.ProdutoRepository.GetAsync(p => p.Id == id);
            if (quantidade <= 0)
            {
                return BadRequest("Insira um valor maior que zero");
            }
            produto.Estoque += quantidade;
            var produtoAtualizado = _uof.ProdutoRepository.Update(produto);
            await _uof.CommitAsync();
            var produtoDto = _mapper.Map<ProdutoResponseDTO>(produtoAtualizado);
            return Ok (produtoDto);
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult<Produto>> Delete(int id)
        {
            var produto = await _uof.ProdutoRepository.GetAsync(p => p.Id == id);
            if (produto is null)
                return NotFound();

            var produtoDeletado = _uof.ProdutoRepository.Delete(produto);
            await _uof.CommitAsync();
            var produtoDeletadoDto = _mapper.Map<ProdutoResponseDTO>(produtoDeletado);
            return Ok(produtoDeletadoDto);

        }

        private ActionResult<IEnumerable<ProdutoResponseDTO>> ObterProdutos(PagedList<Produto> produtos) {
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


