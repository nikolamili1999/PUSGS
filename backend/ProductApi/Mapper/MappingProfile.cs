using AutoMapper;
using ProductApi.Dto;
using ProductApi.Models;

namespace Backend.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Product, ProductDto>().ReverseMap();
            CreateMap<Product, CreateUpdateProductDto>().ReverseMap().ForSourceMember(x => x.ImageFile, y => y.DoNotValidate()); ;
        }
    }
}
