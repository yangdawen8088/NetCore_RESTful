using FakeXiecheng.API.DTOs;
using FakeXiecheng.API.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using System.Text.RegularExpressions;

namespace FakeXiecheng.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]//属性限定
    public class TouristRoutesController : ControllerBase
    {
        private ITouristRouteRepository _touristRouteRepository;
        private readonly IMapper _mapper;
        public TouristRoutesController(ITouristRouteRepository touristRouteRepository, IMapper mapper)
        {
            _touristRouteRepository = touristRouteRepository;
            _mapper = mapper;
        }
        // api/touristRoutes?keyword=传入的参数（关键字）
        [HttpGet]
        [HttpHead]
        public IActionResult GerTouristRoutes(
            [FromQuery] string keyword,
            string rating//评分条件，小于lessThan，大于largerThan，等于equalTo lessThan3，largerThan2，equalTo5
            )//FromQuery负责接收URL的参数，FromBody负责接收请求主体，即请求body中的数据
        {
            Regex regex = new Regex(@"([A-Za-z0-9\-]+)(\d+)");
            string operatorType = "";
            int raringValue = -1;
            Match match = regex.Match(rating);
            if (match.Success)
            {
                operatorType = match.Groups[1].Value;
                raringValue = Int32.Parse(match.Groups[2].Value);
            }
            var touristRoutesFromRepo = _touristRouteRepository.GetTouristRoutes(keyword, operatorType, raringValue);
            if (touristRoutesFromRepo == null || touristRoutesFromRepo.Count() <= 0)
            {
                return NotFound("没有旅游路线");
            }
            var touristRoutesDto = _mapper.Map<IEnumerable<TouristRouteDto>>(touristRoutesFromRepo);
            return Ok(touristRoutesDto);
        }
        // api/touristroutes/{touristRouteId}
        [HttpGet("{touristRouteId}")]
        [HttpHead]
        public IActionResult GetTouristRouteById(Guid touristRouteId)
        {
            var touristRouteFromRepo = _touristRouteRepository.GetTouristRoute(touristRouteId);
            if (touristRouteFromRepo == null)
            {
                return NotFound($"旅游路线{touristRouteId}找不到");
            }
            //var touristRouteDto = new TouristRouteDto()
            //{
            //    Id= touristRouteFromRepo.Id,
            //    Title= touristRouteFromRepo.Title,
            //    Description= touristRouteFromRepo.Description,
            //    Price= touristRouteFromRepo.OriginalPrice*(decimal)(touristRouteFromRepo.DiscountPresent ?? 1),
            //    CreateTime= touristRouteFromRepo.CreateTime,
            //    UpdateTime= touristRouteFromRepo.UpdateTime,
            //    Features= touristRouteFromRepo.Features,
            //    Fees= touristRouteFromRepo.Fees,
            //    Notes= touristRouteFromRepo.Notes,
            //    Rating= touristRouteFromRepo.Rating,
            //    TripType= touristRouteFromRepo.TripType.ToString(),
            //    TravelDays= touristRouteFromRepo.TravelDays.ToString(),
            //    DepartureCity= touristRouteFromRepo.DepartureCity.ToString()
            //};
            var touristRouteDto = _mapper.Map<TouristRouteDto>(touristRouteFromRepo);
            //OK()表示Http状态码200的请求情况
            return Ok(touristRouteDto);
        }
    }
}
