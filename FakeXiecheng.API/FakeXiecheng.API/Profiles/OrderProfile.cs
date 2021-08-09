using AutoMapper;
using FakeXiecheng.API.DTOs;
using FakeXiecheng.API.Moldes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FakeXiecheng.API.Profiles
{
    public class OrderProfile : Profile
    {
        public OrderProfile()
        {
            CreateMap<Order, OrderDto>().ForMember(
                dest => dest.State,
                opt =>
                {
                    opt.MapFrom(src => src.State.ToString());
                });
        }
    }
}
