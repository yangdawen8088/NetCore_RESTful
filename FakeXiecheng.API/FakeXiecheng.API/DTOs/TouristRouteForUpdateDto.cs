﻿using FakeXiecheng.API.ValidationAttributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FakeXiecheng.API.DTOs
{
    [TouristRouteTitleMustBeDifferentFromDescriptionAttribute]
    public class TouristRouteForUpdateDto:TouristRouteForManipulationDto
    {
        [Required(ErrorMessage ="更新必备")]
        [MaxLength(1500)]
        public override string Description { get; set; }
    }
}
