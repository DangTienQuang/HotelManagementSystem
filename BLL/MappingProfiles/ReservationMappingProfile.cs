using AutoMapper;
using DTOs;
using DTOs.Entities;
using System;

namespace BLL.MappingProfiles
{
    public class ReservationMappingProfile : Profile
    {
        public ReservationMappingProfile()
        {
            CreateMap<Reservation, ReservationDto>()
                .ForMember(dest => dest.CustomerName, opt => opt.MapFrom(src => src.Customer.FullName))
                .ForMember(dest => dest.RoomNumber, opt => opt.MapFrom(src => src.Room.RoomNumber))
                .ForMember(dest => dest.NumberOfNights, opt => opt.MapFrom(src => (src.CheckOutDate - src.CheckInDate).Days))
                .ForMember(dest => dest.TotalPrice, opt => opt.MapFrom(src => src.Room.Price * (src.CheckOutDate - src.CheckInDate).Days));
        }
    }
}
