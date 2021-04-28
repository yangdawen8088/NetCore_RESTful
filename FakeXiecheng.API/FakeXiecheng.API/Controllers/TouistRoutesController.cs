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
            //OK()表示Http状态码200的请求情况
            return Ok(touristRouteFromRepo);
        }
    }
}
