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
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Net.Http.Headers;

namespace FakeXiecheng.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]//属性限定
    public class TouristRoutesController : ControllerBase
    {
        private ITouristRouteRepository _touristRouteRepository;
        private readonly IMapper _mapper;
        private readonly IUrlHelper _urlHelper;
        private readonly IPropertyMappingService _propertyMappingService;
        public TouristRoutesController(
            ITouristRouteRepository touristRouteRepository,
            IMapper mapper,
            IUrlHelperFactory urlHelperFactory,
            IActionContextAccessor actionContextAccessor,
            IPropertyMappingService propertyMappingService)
        {
            _touristRouteRepository = touristRouteRepository;
            _mapper = mapper;
            _urlHelper = urlHelperFactory.GetUrlHelper(actionContextAccessor.ActionContext);
            _propertyMappingService = propertyMappingService;
        }
        private string GenerateTouristRouteResourceURL(
            TouristRouteResourceParamaters paramaters,
            PaginationResourceParamaters paramaters2,
            ResourceUrlType type
            )
        {
            return type switch
            {
                ResourceUrlType.PreviousPage => _urlHelper.Link("GerTouristRoutes",
                new
                {
                    fields = paramaters.Fields,
                    orderBy = paramaters.OrderBy,
                    keyword = paramaters.Keyword,
                    rating = paramaters.Rating,
                    pageNumber = paramaters2.PageNumber - 1,
                    pageSize = paramaters2.PageSize
                }),
                ResourceUrlType.NextPage => _urlHelper.Link("GerTouristRoutes",
                new
                {
                    fields = paramaters.Fields,
                    orderBy = paramaters.OrderBy,
                    keyword = paramaters.Keyword,
                    rating = paramaters.Rating,
                    pageNumber = paramaters2.PageNumber + 1,
                    pageSize = paramaters2.PageSize
                }),
                _ => _urlHelper.Link("GerTouristRoutes",
                new
                {
                    fields = paramaters.Fields,
                    orderBy = paramaters.OrderBy,
                    keyword = paramaters.Keyword,
                    rating = paramaters.Rating,
                    pageNumber = paramaters2.PageNumber,
                    pageSize = paramaters2.PageSize
                })
            };
        }
        // api/touristRoutes?keyword=传入的参数（关键字）
        // 1. application/json -> 旅游路线资源
        // 2. application/vnd.{公司/企业名称}.hatecas+json
        // vnd:Vendor 供应商的缩写，表示这个媒体类型是特定供应商所使用的
        // hatecas 表示返回的响应里面要包含超媒体相关的链接
        // json 表示我们需要的响应输出是 json 格式
        [HttpGet(Name = "GerTouristRoutes")]
        [HttpHead]
        public async Task<IActionResult> GerTouristRoutes(
            [FromQuery] TouristRouteResourceParamaters paramaters,
            [FromQuery] PaginationResourceParamaters paramaters2,
            [FromHeader(Name ="Accept")] string mediaType
            //[FromQuery] string keyword,
            //string rating//评分条件，小于lessThan，大于largerThan，等于equalTo lessThan3，largerThan2，equalTo5
            )//FromQuery负责接收URL的参数，FromBody负责接收请求主体，即请求body中的数据
        {
            if (!MediaTypeHeaderValue.TryParse(mediaType,out MediaTypeHeaderValue parsedMediatype))//可以解析一个 List 的媒体类型值 如 TryParseList
            {// 表示解析头部信息 如果不成功，则执行判断题
                return BadRequest();
            }
            // 检测用户输入的排序字段输入是否正确 如果不正确 则返回 400 级别错误
            if (!_propertyMappingService.IsMappingExists<TouristRouteDto, TouristRoute>(paramaters.OrderBy))
            {
                return BadRequest("请输入正确的排序参数。");
            }
            if (!_propertyMappingService.IsPropertiesExists<TouristRouteDto>(paramaters.Fields))
            {
                return BadRequest("请输入正确的塑型参数。");
            }
            var touristRoutesFromRepo = await _touristRouteRepository
                .GetTouristRoutesAsync(
                paramaters.Keyword,
                paramaters.RatingOperator,
                paramaters.RatingValue,
                paramaters2.PageSize,
                paramaters2.PageNumber,
                paramaters.OrderBy);
            if (touristRoutesFromRepo == null || touristRoutesFromRepo.Count() <= 0)
            {
                return NotFound("没有旅游路线");
            }
            var touristRoutesDto = _mapper.Map<IEnumerable<TouristRouteDto>>(touristRoutesFromRepo);
            // 处理分页数据
            var previousPageLink = touristRoutesFromRepo.HasPreviors
                ? GenerateTouristRouteResourceURL(paramaters, paramaters2, ResourceUrlType.PreviousPage)
                : null;
            var nextPageLink = touristRoutesFromRepo.HasNext
                ? GenerateTouristRouteResourceURL(paramaters, paramaters2, ResourceUrlType.NextPage)
                : null;
            // 将分页信息加入到 HTTP 请求的头部信息中
            // x-pagination
            var paginationMetadata = new
            {
                previousPageLink,
                nextPageLink,
                totalCount = touristRoutesFromRepo.TotalCount,
                pageSize = touristRoutesFromRepo.PageSize,
                currentPage = touristRoutesFromRepo.CurrentPage,
                totalPages = touristRoutesFromRepo.TotalPages
            };
            // 取得头部控制权 并将分页信息添加到响应请求头中
            Response.Headers.Add("x-pagination", Newtonsoft.Json.JsonConvert.SerializeObject(paginationMetadata));
            var shapedDtoList = touristRoutesDto.ShapeData(paramaters.Fields);
            if (parsedMediatype.MediaType=="application/vnd.yangdawen.hateoas+json")// 这里企业名称建议从配置文件中获取 然后拼接起来
            {
                var linkDto = CreateLinksForTouristRouteList(paramaters, paramaters2);
                var shapedDtoWithLinklist = shapedDtoList.Select(t =>
                {
                    var touristRouteDictionary = t as IDictionary<string, object>;
                    var links = CreateLinkForTouristRoute((Guid)touristRouteDictionary["Id"], null);
                    touristRouteDictionary.Add("links", links);
                    return touristRouteDictionary;
                });
                var result = new
                {
                    value = shapedDtoWithLinklist,
                    links = linkDto
                };
                return Ok(result);
            }
            return Ok(shapedDtoList);
        }
        private IEnumerable<LinkDto> CreateLinksForTouristRouteList(
            TouristRouteResourceParamaters paramaters,
            PaginationResourceParamaters paramaters2)
        {

            var links = new List<LinkDto>();
            // 添加 self，自我链接
            links.Add(new LinkDto(
                GenerateTouristRouteResourceURL(paramaters, paramaters2, ResourceUrlType.CurrnetPage),
                "self",
                "GET"
                ));
            // "api/touistRoutes"
            // 添加创建旅游路线
            links.Add(new LinkDto(
                Url.Link("CreateTouristRoute", null),
                "create_tourist_route",
                "POST"));
            return links;
        }
        // api/touristroutes/{touristRouteId}
        [HttpGet("{touristRouteId}", Name = "GetTouristRouteById")] // Name 的意思就是标记这个 API 的名称
        [HttpHead]
        public async Task<IActionResult> GetTouristRouteById(Guid touristRouteId, string fields)
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
            //return Ok(touristRouteDto.ShapeData(fields));
            var linkDtos = CreateLinkForTouristRoute(touristRouteId, fields);
            var result = touristRouteDto.ShapeData(fields) as IDictionary<string, object>;
            result.Add("links", linkDtos);
            return Ok(result);
        }
        private IEnumerable<LinkDto> CreateLinkForTouristRoute(Guid touristRouteId, string fields)
        {
            var links = new List<LinkDto>();
            // 获取旅游路线
            links.Add(new LinkDto(
                Url.Link("GetTouristRouteById", new { touristRouteId, fields }),
                "self",
                "GET"));
            // 更新
            links.Add(new LinkDto(
                Url.Link("UpdateToristRoute", new { touristRouteId }),
                "update",
                "PUT"));
            // 局部更新
            links.Add(new LinkDto(
                Url.Link("PartiallyUpdateTouristRoute", new { touristRouteId }),
                "Partially_Update",
                "PATCH"));
            // 删除
            links.Add(new LinkDto(
                Url.Link("DeleteTouristRoute", new { touristRouteId }),
                "delete",
                "DELETE"));
            // 获取旅游路线图片
            links.Add(new LinkDto(
                Url.Link("GetPictureListForTouristRoute", new { touristRouteId }),
                "get_Pictures",
                "GET"));
            // 添加新的图片
            links.Add(new LinkDto(
                Url.Link("CreateTouristRoutePicture", new { touristRouteId }),
                "create_Picture",
                "POST"));
            return links;
        }
        [HttpPost(Name = "CreateTouristRoute")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        //[Authorize(Roles ="Admin")]// 设置此 API 需要经过用户登陆后才能访问，这里设置了参数，表示只用用户角色为 Admin 的用户才能访问
        public async Task<IActionResult> CreateTouristRoute([FromBody] TouristRouteForCreationDto touristRouteForCreationDto)
        {
            var touristRouteModel = _mapper.Map<TouristRoute>(touristRouteForCreationDto);
            _touristRouteRepository.AddTouristRoute(touristRouteModel);
            await _touristRouteRepository.SaveAsync();
            var touristRouteToReture = _mapper.Map<TouristRouteDto>(touristRouteModel);
            var links = CreateLinkForTouristRoute(touristRouteModel.Id, null);
            var result = touristRouteToReture.ShapeData(null) as IDictionary<string, object>;
            result.Add("links", links);
            return CreatedAtRoute(
                "GetTouristRouteById",
                new { touristRouteId = result["Id"] },
                result);
        }
        [HttpPut("{touristRouteId}", Name = "UpdateToristRoute")]
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
        [HttpPatch("{touristRouteId}", Name = "PartiallyUpdateTouristRoute")]
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
        [HttpDelete("{touristRouteId}", Name = "DeleteTouristRoute")]
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