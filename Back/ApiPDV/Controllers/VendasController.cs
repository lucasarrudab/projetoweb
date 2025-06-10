using ApiPDV.DTOs;
using ApiPDV.Pagination;
using ApiPDV.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Route("[controller]")]
[ApiController]
public class VendasController : Controller
{
    private readonly IVendasService _vendasService;

    public VendasController(IVendasService vendasService)
    {
        _vendasService = vendasService;
    }

    [HttpGet]
    [Authorize(Policy = "All")]
    public async Task<ActionResult<IEnumerable<VendaDTO>>> GetAllVendas()
    {
        return Ok(await _vendasService.GetAllVendasAsync());
    }

    [HttpGet("{id:int}")]
    [Authorize(Policy = "All")]
    public async Task<ActionResult<VendaDTO>> GetVenda(int id)
    {
        return Ok(await _vendasService.GetVendaByIdAsync(id));
    }

    [HttpGet("pagination")]
    [Authorize]
    public async Task<ActionResult<PagedList<VendaDTO>>> GetAllPaged([FromQuery] VendasParameters parameters)
    {
        return Ok(await _vendasService.GetPagedVendasAsync(parameters));
    }

    [HttpGet("filter/pagination/date")]
    [Authorize]
    public async Task<ActionResult<PagedList<VendaDTO>>> GetAllPaged([FromQuery] VendasFiltroData parameters)
    {
        return Ok(await _vendasService.GetVendasByDataAsync(parameters));
    }

    [HttpGet("filter/pagination/month")]
    [Authorize]
    public async Task<ActionResult<PagedList<VendaDTO>>> GetAllPagedMonth([FromQuery] VendaFiltroMes parameters)
    {
        var result = await _vendasService.GetVendasByMesAsync(parameters);
        if (!result.Any())
            return NotFound("Não foram encontradas vendas nesse filtro");

        return Ok(result);
    }

    [HttpGet("filter/pagination/payment")]
    [Authorize]
    public async Task<ActionResult<PagedList<VendaDTO>>> GetAllPagedPayment([FromQuery] VendaFiltroPagamento parameters)
    {
        return Ok(await _vendasService.GetVendasByPagamentoAsync(parameters));
    }

    [HttpGet("filter/pagination/days")]
    [Authorize]
    public async Task<ActionResult<PagedList<VendaDTO>>> GetAllPageddays([FromQuery] VendasFiltroDias parameters)
    {
        return Ok(await _vendasService.GetVendasByDiasAsync(parameters));
    }

    [HttpPost]
    [Authorize]
    public async Task<ActionResult<VendaDTO>> RealizarVenda([FromBody] string nomeMetodoPagamento)
    {
        var (success, errorMessage, vendaDto) = await _vendasService.RealizarVendaAsync(nomeMetodoPagamento);

        if (!success)
            return BadRequest(errorMessage);

        return Ok(vendaDto);
    }
}
