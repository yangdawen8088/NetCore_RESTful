using AutoMapper;
using FakeXiecheng.API.DTOs;
using FakeXiecheng.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;

namespace FakeXiecheng.API.Controllers
{
    [Route("api/orders")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        public readonly IHttpContextAccessor _httpContextAccessor;
        public readonly ITouristRouteRepository _touristRouteRepository;
        public readonly IMapper _mapper;
        public readonly IHttpClientFactory _httpClientFactory;
        public OrdersController(
            IHttpContextAccessor httpContextAccessor,
            ITouristRouteRepository touristRouteRepository,
            IMapper mapper,
            IHttpClientFactory httpClientFactory)
        {
            _httpContextAccessor = httpContextAccessor;
            _touristRouteRepository = touristRouteRepository;
            _mapper = mapper;
            _httpClientFactory = httpClientFactory;
        }

        [HttpGet]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<IActionResult> GetOrders()
        {
            // 1 获取当前用户
            var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            // 2 使用用户 ID 来获取订单历史记录
            var orders = await _touristRouteRepository.GetOrdersByUserId(userId);
            return Ok(_mapper.Map<IEnumerable<OrderDto>>(orders));
        }

        [HttpGet("{orderId}")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<IActionResult> GetOrderById([FromRoute] Guid orderId)
        {
            // 1 获取当前用户
            var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var order = await _touristRouteRepository.GetOrderById(orderId);
            return Ok(_mapper.Map<OrderDto>(order));
        }

        [HttpPost("{orderId}/placeOrder")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<IActionResult> PlaceOrder([FromRoute] Guid orderId)
        {
            // 1 获取当前用户
            var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            // 2 开始处理支付
            var order = await _touristRouteRepository.GetOrderById(orderId);
            order.PaymentProcessing();
            await _touristRouteRepository.SaveAsync();
            // 3 向第三方提交支付请求，等待第三方响应
            var httpClient = _httpClientFactory.CreateClient();
            string url = @"http://123.56.149.216/api/FakePaymentProcess?icode={0}&orderNumber={1}&returnFault={2}";
            var response = await httpClient.PostAsync(string.Format(url, "B6B1DFB12AA5FB9F", order.Id, false),null);
            // 4 提取支付结果，以及支付信息
            bool isApproved = false;
            string transactionMetadata = "";
            if (response.IsSuccessStatusCode)
            {
                transactionMetadata = await response.Content.ReadAsStringAsync();
                var jsonObject = (JObject)JsonConvert.DeserializeObject(transactionMetadata);
                isApproved = jsonObject["approved"].Value<bool>();
            }
            // 如果第三方支付成功，则完成订单
            if (isApproved)
            {
                order.PaymentApprove();
            }
            else
            {
                order.PaymentReject();
            }
            order.TransactionMetadata = transactionMetadata;
            await _touristRouteRepository.SaveAsync();
            return Ok(_mapper.Map<OrderDto>(order));
        }
    }
}
