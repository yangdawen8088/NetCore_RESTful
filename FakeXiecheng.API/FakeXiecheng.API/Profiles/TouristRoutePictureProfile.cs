﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using FakeXiecheng.API.DTOs;
using FakeXiecheng.API.Moldes;

namespace FakeXiecheng.API.Profiles
{
    public class TouristRoutePictureProfile : Profile
    {
        public TouristRoutePictureProfile()
        {
            CreateMap<TouristRoutePicture, TouristRoutePictureDto>();
            CreateMap<TouristRouteForCreationDto, TouristRoutePicture> ();
        }
    }
}
