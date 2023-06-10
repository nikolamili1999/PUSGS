using AutoMapper;
using IdentityApi.Dto;
using IdentityApi.Models;

namespace IdentityApi.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<User, UserDto>().ReverseMap();
            CreateMap<User, RegisterDto>().ReverseMap();
            CreateMap<User, LoginDto>().ReverseMap();
            CreateMap<User, UpdateUserDto>().ReverseMap();
            CreateMap<User, GoogleLoginDto>().ReverseMap();
        }
    }
}
