using AutoMapper;
using Mango.Services.ShoppingCartAPI.Models;
using Mango.Services.ShoppingCartAPI.Models.Dto;

namespace Mango.Services.ShoppingCartAPI
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<ProductDto, Product>().ReverseMap();
            CreateMap<CartDetailsDto, CartDetails>().ReverseMap();
            CreateMap<CartHeaderDto, CartHeader>().ReverseMap();
            CreateMap<CartDto, Cart>().ReverseMap();
        }
    }
}
