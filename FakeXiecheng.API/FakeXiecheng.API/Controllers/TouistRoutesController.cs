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
        public IActionResult GerTouristRoutes()
        {
            var routes = _touristRouteRepository.GetTouristRoutes();
            return Ok(routes);
        }
    }
}
