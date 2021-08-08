using FakeXiecheng.API.DTOs;
using FakeXiecheng.API.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using FakeXiecheng.API.ResourceParameters;
using FakeXiecheng.API.Moldes;
using Microsoft.AspNetCore.JsonPatch;
using FakeXiecheng.API.Helper;
using Microsoft.AspNetCore.Authorization;

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
        public async Task<IActionResult> GerTouristRoutes(
            [FromQuery] TouristRouteResourceParamaters paramaters
            //[FromQuery] string keyword,
            //string rating//评分条件，小于lessThan，大于largerThan，等于equalTo lessThan3，largerThan2，equalTo5
            )//FromQuery负责接收URL的参数，FromBody负责接收请求主体，即请求body中的数据
        {
            var touristRoutesFromRepo = await _touristRouteRepository.GetTouristRoutesAsync(paramaters.Keyword, paramaters.RatingOperator, paramaters.RatingValue);
            if (touristRoutesFromRepo == null || touristRoutesFromRepo.Count() <= 0)
            {
                return NotFound("没有旅游路线");
            }
            var touristRoutesDto = _mapper.Map<IEnumerable<TouristRouteDto>>(touristRoutesFromRepo);
            return Ok(touristRoutesDto);
        }
        // api/touristroutes/{touristRouteId}
        [HttpGet("{touristRouteId}", Name = "GetTouristRouteById")]
        [HttpHead]
        public async Task<IActionResult> GetTouristRouteById(Guid touristRouteId)
        {
            var touristRouteFromRepo = await _touristRouteRepository.GetTouristRouteAsync(touristRouteId);
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
        [HttpPost]
        [Authorize(AuthenticationSchemes ="Bearer")]
        //[Authorize(Roles ="Admin")]// 设置此 API 需要经过用户登陆后才能访问，这里设置了参数，表示只用用户角色为 Admin 的用户才能访问
        public async Task<IActionResult> CreateTouristRoute([FromBody] TouristRouteForCreationDto touristRouteForCreationDto)
        {
            var touristRouteModel = _mapper.Map<TouristRoute>(touristRouteForCreationDto);
            _touristRouteRepository.AddTouristRoute(touristRouteModel);
            await _touristRouteRepository.SaveAsync();
            var touristRouteToReture = _mapper.Map<TouristRouteDto>(touristRouteModel);
            return CreatedAtRoute("GetTouristRouteById", new { touristRouteId = touristRouteToReture.Id }, touristRouteToReture);
        }
        [HttpPut("{touristRouteId}")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateToristRoute(
            [FromRoute] Guid touristRouteId,
            [FromBody] TouristRouteForUpdateDto touristRouteForUpdateDto)
        {
            if (!await _touristRouteRepository.TouristRouteExistsAsync(touristRouteId))
            {
                return NotFound("路由路线找不到");
            }
            var touristRouteFromRepo = await _touristRouteRepository.GetTouristRouteAsync(touristRouteId);
            // 1. 映射dto 2. 更新dto    3.映射model
            _mapper.Map(touristRouteForUpdateDto, touristRouteFromRepo);
            // 上面一行代码已经对数据仓库里面的数据做了修改，这一步由EF完成的，下面的一行代码直接将所作的更改保存到数据库中
            await _touristRouteRepository.SaveAsync();
            return NoContent();// 204 状态码
        }
        [HttpPatch("{touristRouteId}")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> PartiallyUpdateTouristRoute(
            [FromRoute] Guid touristRouteId,
            [FromBody] JsonPatchDocument<TouristRouteForUpdateDto> patchDocument)
        {
            if (!await _touristRouteRepository.TouristRouteExistsAsync(touristRouteId))
            {
                return NotFound("路由路线找不到");
            }
            var touristRouteFromRepo = await _touristRouteRepository.GetTouristRouteAsync(touristRouteId);
            var touristRouteToPatch = _mapper.Map<TouristRouteForUpdateDto>(touristRouteFromRepo);
            patchDocument.ApplyTo(touristRouteToPatch, ModelState);
            if (!TryValidateModel(touristRouteToPatch))
            {
                return ValidationProblem(ModelState);
            }
            _mapper.Map(touristRouteToPatch, touristRouteFromRepo);
            await _touristRouteRepository.SaveAsync();
            return NoContent();
        }
        [HttpDelete("{touristRouteId}")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteTouristRoute([FromRoute] Guid touristRouteId)
        {
            if (!await _touristRouteRepository.TouristRouteExistsAsync(touristRouteId))
            {
                return NotFound("路由路线找不到");
            }
            var touristRoute = await _touristRouteRepository.GetTouristRouteAsync(touristRouteId);
            _touristRouteRepository.DeleteTouristRoute(touristRoute);
            await _touristRouteRepository.SaveAsync();
            return NoContent();
        }
        [HttpDelete("({touristIDs})")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteByIDs([ModelBinder(BinderType = typeof(ArrayModelBinder))][FromRoute] IEnumerable<Guid> touristIDs)
        {
            if (touristIDs == null)
            {
                return BadRequest();
            }
            var touristRoutesFromRepo = await _touristRouteRepository.GetTouristRoutesByIDListAsync(touristIDs);
            _touristRouteRepository.DeleteTOuristRoutes(touristRoutesFromRepo);
            await _touristRouteRepository.SaveAsync();
            return NoContent();
        }
    }
}