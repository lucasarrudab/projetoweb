using ApiPDV.DTOs.Request;
using ApiPDV.DTOs.Response;
using ApiPDV.Models;
using AutoMapper;

namespace ApiPDV.DTOs.Mapping
{
    public class DTOMapping : Profile
    {
        public DTOMapping()
        {
            CreateMap<Produto, ProdutoRequestDTO>().ReverseMap();
            CreateMap<Produto, ProdutoResponseDTO>().ReverseMap();
            CreateMap<Carrinho, CarrinhoDTO>().ReverseMap();
            CreateMap<ProdutoCarrinho, ProdutoCarrinhoDTO>()
                .ForMember(dest => dest.ProdutoNome, opt => opt.MapFrom(src => src.Produto.Nome));
            CreateMap<Venda, VendaDTO>()
                .ForMember(dest => dest.MetodoPagamento, opt => opt.MapFrom(src => src.MetodoPagamento.Nome));
                
           
        }

    }
}
