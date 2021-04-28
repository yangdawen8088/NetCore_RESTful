using FakeXiecheng.API.DTOs;
using FakeXiecheng.API.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FakeXiecheng.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]//属性限定
    public class TouistRoutesController : ControllerBase
    {
        private ITouristRouteRepository _touristRouteRepository;
        public TouistRoutesController(ITouristRouteRepository touristRouteRepository)
        {
            _touristRouteRepository = touristRouteRepository;
        }
        [HttpGet]
        public IActionResult GerTouristRoutes()
        {
            var touristRoutesFromRepo = _touristRouteRepository.GetTouristRoutes();
            if (touristRoutesFromRepo==null|| touristRoutesFromRepo.Count()<=0)
            {
                return NotFound("没有旅游路线");
            }
            return Ok(touristRoutesFromRepo);
        }
        [HttpGet("{touristRouteId}")]
        public IActionResult GetTouristRouteById(Guid touristRouteId)
        {
            var touristRouteFromRepo = _touristRouteRepository.GetTouristRoute(touristRouteId);
            if(touristRouteFromRepo==null)
            {
                return NotFound($"旅游路线{touristRouteId}找不到");
            }
            var touristRouteDto = new TouristRouteDto()
            {
                Id= touristRouteFromRepo.Id,
                Title= touristRouteFromRepo.Title,
                Description= touristRouteFromRepo.Description,
                Price= touristRouteFromRepo.OriginalPrice*(decimal)(touristRouteFromRepo.DiscountPresent ?? 1),
                CreateTime= touristRouteFromRepo.CreateTime,
                UpdateTime= touristRouteFromRepo.UpdateTime,
                Features= touristRouteFromRepo.Features,
                Fees= touristRouteFromRepo.Fees,
                Notes= touristRouteFromRepo.Notes,
                Rating= touristRouteFromRepo.Rating,
                TripType= touristRouteFromRepo.TripType.ToString(),
                TravelDays= touristRouteFromRepo.TravelDays.ToString(),
                DepartureCity= touristRouteFromRepo.DepartureCity.ToString()
            };
            //OK()表示Http状态码200的请求情况
            return Ok(touristRouteDto);
        }
    }
}
