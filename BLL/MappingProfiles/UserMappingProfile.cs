using AutoMapper;
using DTOs;
using DTOs.Entities;

namespace BLL.MappingProfiles
{
    public class UserMappingProfile : Profile
    {
        public UserMappingProfile()
        {
            CreateMap<User, UserDto>()
                .ForMember(dest => dest.Password, opt => opt.Ignore())
                .ForMember(dest => dest.IsStaff, opt => opt.MapFrom(src => src.Role == "Staff"));
            
            CreateMap<UserDto, User>()
                .ForMember(dest => dest.PasswordHash, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.Staffs, opt => opt.Ignore());
        }
    }
}
