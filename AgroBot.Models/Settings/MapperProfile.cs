using AgroBot.Models.ModelsDB;
using AutoMapper;
using System;

namespace AgroBot.Models.Settings
{
    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            CreateMap<RouteDto, Route>().AfterMap((src, dest) =>
            {
                dest.Id = Guid.NewGuid();
                dest.CreatedDate = DateTime.Now;
            });
            CreateMap<CheckPointDto, CheckPoint>().AfterMap((src, dest) =>
            {
                dest.Id = Guid.NewGuid();
            });
        }
    }
}
