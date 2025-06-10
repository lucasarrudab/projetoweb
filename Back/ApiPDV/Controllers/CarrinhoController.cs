using ApiPDV.DTOs;
using ApiPDV.Repositories;
using ApiPDV.Services;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ApiPDV.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CarrinhoController : ControllerBase
    {
        private readonly ICarrinhoService _carrinhoService;
        private readonly IUnitOfWork _uof;
        private readonly IMapper _mapper;

        public CarrinhoController(ICarrinhoService carrinhoService, IUnitOfWork uof, IMapper mapper)
        {
            _carrinhoService = carrinhoService;
            _uof = uof;
            _mapper = mapper;
        }

        [HttpGet]
        [Authorize(Policy = "Management")]
        public async Task<ActionResult<IEnumerable<CarrinhoDTO>>> GetAllAsync()
        {
            var carrinhos = await _uof.CarrinhoRepository.GetAllAsync();
            return Ok(_mapper.Map<IEnumerable<CarrinhoDTO>>(carrinhos));
        }

        [HttpGet("{id:int}")]
        [Authorize(Policy = "Management")]
        public async Task<ActionResult<CarrinhoDTO>> Get(int id)
        {
            var carrinho = await _uof.CarrinhoRepository.GetAsync(id);
            if (carrinho == null)
                return NotFound();

            return Ok(_mapper.Map<CarrinhoDTO>(carrinho));
        }

        [HttpPost]
        //[Authorize(Policy = "All")]
        public async Task<ActionResult<CarrinhoDTO>> Create()
        {
            var carrinhoDto = await _carrinhoService.CriarCarrinhoAsync();
            return Ok(carrinhoDto);
        }

        [HttpPut]
        //[Authorize(Policy = "All")]
        public async Task<ActionResult<CarrinhoDTO>> AdicionarAoCarrinho([FromBody] string code)
        {
            try
            {
                var carrinhoDto = await _carrinhoService.AdicionarProdutoAoCarrinhoAsync(code);
                if (carrinhoDto == null)
                    return NotFound("Produto não encontrado");

                return Ok(carrinhoDto);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("/quantiadeproduto/{id:int}")]
        [Authorize(Policy = "All")]
        public async Task<ActionResult<CarrinhoDTO>> AlterarQuantidade(int id, [FromBody] int quantidade)
        {
            if (quantidade < 0)
                return BadRequest("A quantidade precisa ser maior que 0");

            try
            {
                var carrinhoDto = await _carrinhoService.AlterarQuantidadeProdutoAsync(id, quantidade);
                return Ok(carrinhoDto);
            }
            catch (KeyNotFoundException e)
            {
                return NotFound(e.Message);
            }
            catch (InvalidOperationException e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpDelete("{id:int}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<ActionResult<CarrinhoDTO>> Delete(int id)
        {
            var carrinho = await _uof.CarrinhoRepository.GetAsync(c => c.Id == id);
            if (carrinho == null)
                return NotFound();

            var excluido = _uof.CarrinhoRepository.Delete(carrinho);
            await _uof.CommitAsync();

            return Ok(_mapper.Map<CarrinhoDTO>(excluido));
        }
    }
}
