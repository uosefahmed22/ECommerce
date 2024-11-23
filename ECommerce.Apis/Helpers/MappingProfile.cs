using AutoMapper;
using ECommerce.Core.DTOs;
using ECommerce.Core.Models;
using StackExchange.Redis;

namespace ECommerce.Apis.Helpers
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Brand, BrandDto>().ReverseMap();
            CreateMap<Category, CategoryDto>().ReverseMap();
            CreateMap<Product, ProductDto>().ReverseMap();
            CreateMap<OrderModel, OrderModelDto>().ReverseMap();
        }
    }
}
