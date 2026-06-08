using AutoMapper;
using RetroGaming.Common.DTOs;
using RetroGaming.DAL.Entities;

namespace RetroGaming.BLL.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Manufacturer, ManufacturerDto>()
                .ForMember(d => d.ConsoleCount,
                           o => o.MapFrom(s => s.ConsoleCount));

            CreateMap<Manufacturer, ManufacturerMapDto>()
                .ForMember(d => d.ConsoleCount,
                           o => o.MapFrom(s => s.ConsoleCount));

            CreateMap<CreateManufacturerDto, Manufacturer>();
            CreateMap<UpdateManufacturerDto, Manufacturer>();



            CreateMap<GameConsole, ConsoleDto>()
                .ForMember(d => d.ManufacturerName,
                           o => o.MapFrom(s => s.ManufacturerName));

            CreateMap<CreateConsoleDto, GameConsole>();
            CreateMap<UpdateConsoleDto, GameConsole>();
        }
    }
}