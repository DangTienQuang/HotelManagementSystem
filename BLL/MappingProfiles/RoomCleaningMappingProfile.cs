using AutoMapper;
using DTOs;
using DTOs.Entities;

namespace BLL.MappingProfiles
{
    public class RoomCleaningMappingProfile : Profile
    {
        public RoomCleaningMappingProfile()
        {
            CreateMap<RoomCleaning, RoomCleaningDto>()
                .ForMember(dest => dest.RoomNumber, opt => opt.MapFrom(src => src.Room.RoomNumber))
                .ForMember(dest => dest.CleanedById, opt => opt.MapFrom(src => src.CleanedBy))
                .ForMember(dest => dest.CleanerName, opt => opt.MapFrom(src => src.Cleaner != null ? src.Cleaner.FullName : string.Empty));
            
            CreateMap<RoomCleaningDto, RoomCleaning>()
                .ForMember(dest => dest.Room, opt => opt.Ignore())
                .ForMember(dest => dest.Cleaner, opt => opt.Ignore())
                .ForMember(dest => dest.CleanedBy, opt => opt.MapFrom(src => src.CleanedById));
        }
    }
}
