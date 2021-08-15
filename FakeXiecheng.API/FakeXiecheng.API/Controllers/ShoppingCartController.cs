using AutoMapper;
using FakeXiecheng.API.DTOs;
using FakeXiecheng.API.Helper;
using FakeXiecheng.API.Moldes;
using FakeXiecheng.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace FakeXiecheng.API.Controllers
{
    [Route("api/ShoppingCart")]
    [ApiController]
    public class ShoppingCartController : ControllerBase
    {
        public readonly IHttpContextAccessor _httpContextAccessor;
        public readonly ITouristRouteRepository _touristRouteRepository;
        public readonly IMapper _mapper;

        public ShoppingCartController(
            IHttpContextAccessor httpContextAccessor,
            ITouristRouteRepository touristRouteRepository,
            IMapper mapper)
        {
            _httpContextAccessor = httpContextAccessor;
            _touristRouteRepository = touristRouteRepository;
            _mapper = mapper;
        }
        [HttpGet(Name = "GetShoppingCart")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<IActionResult> GetShoppingCart()
        {
            // 1 获取当前用户
            var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            // 2 使用 userid 获得购物车
            var shoppingcart = await _touristRouteRepository.GetShoppingCartByUserId(userId);
            return Ok(_mapper.Map<ShoppingCartDto>(shoppingcart));
        }
        [HttpPost("items")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<IActionResult> AddShoppingCartItem([FromBody] AddShoppingCartItemDto addShoppingCartItemDto)
        {
            // 1 获取当前用户
            var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            // 2 使用 userid 获得购物车
            var shoppingcart = await _touristRouteRepository.GetShoppingCartByUserId(userId);
            // 3 创建 lineItem
            var touristRoute = await _touristRouteRepository.GetTouristRouteAsync(addShoppingCartItemDto.TouristRouteId);
            if (touristRoute == null)
            {
                return NotFound("路由路线不存在");
            }
            var lineItem = new LineItem()
            {
                TouristRouteId = addShoppingCartItemDto.TouristRouteId,
                ShoppingCartId = shoppingcart.Id,
                OriginalPrice = touristRoute.OriginalPrice,
                DiscountPresent = touristRoute.DiscountPresent
            };
            // 4 添加 lineitem，并保存数据库
            await _touristRouteRepository.AddShoppingCartItem(lineItem);
            await _touristRouteRepository.SaveAsync();
            return Ok(_mapper.Map<ShoppingCartDto>(shoppingcart));
        }
        [HttpDelete("items/{itemId}")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<IActionResult> DeleteShoppingCartItem([FromRoute] int itemId)
        {
            // 1 获取 lineitem 数据
            var lineItem = await _touristRouteRepository.GetShoppingCartItemByItemId(itemId);
            // 2 判断购物车商品是否存在
            if (lineItem == null)
            {
                return NotFound("购物车商品找不到");
            }
            // 3 删除获取成功的购物车商品
            _touristRouteRepository.DeleteShoppingCartItem(lineItem);
            // 4 保存数据仓库
            await _touristRouteRepository.SaveAsync();
            // 5 返回 API 执行结果
            return NoContent();
        }
        [HttpDelete("items/({itemIDs})")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<IActionResult> RemoveShoppingCartItems(
            [ModelBinder(BinderType = typeof(ArrayModelBinder))][FromRoute] IEnumerable<int> itemIDs
            )
        {
            var lineItems = await _touristRouteRepository.GetShoppingCartsByIdListAsync(itemIDs);
            _touristRouteRepository.DeleteShoppingCartItems(lineItems);
            await _touristRouteRepository.SaveAsync();
            return NoContent();// 返回的是 204 
        }
        [HttpPost("checkout")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<IActionResult> Checkout()
        {
            // 1 获取当前用户
            var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            // 2 使用 userid 获得购物车
            var shoppingcart = await _touristRouteRepository.GetShoppingCartByUserId(userId);
            // 3 创建订单
            var order = new Order()
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                State = OrderStateEnum.Pending,
                OrderItems = shoppingcart.ShoppingCartItems,
                CreateDateUTC = DateTime.UtcNow
            };
            // 清空当前购物车
            shoppingcart.ShoppingCartItems = null;
            // 4 保存数据
            await _touristRouteRepository.AddOrderAsync(order);
            await _touristRouteRepository.SaveAsync();
            // 5 return
            return Ok(_mapper.Map<OrderDto>(order));
        }
    }
}
