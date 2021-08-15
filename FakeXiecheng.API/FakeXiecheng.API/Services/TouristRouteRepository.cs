using FakeXiecheng.API.Database;
using FakeXiecheng.API.DTOs;
using FakeXiecheng.API.Helper;
using FakeXiecheng.API.Moldes;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FakeXiecheng.API.Services
{
    public class TouristRouteRepository : ITouristRouteRepository
    {
        private readonly AppDbContext _context;
        private readonly IPropertyMappingService _propertyMappingService;

        public TouristRouteRepository(AppDbContext context, IPropertyMappingService propertyMappingService)
        {
            _context = context;
            _propertyMappingService = propertyMappingService;
        }

        public async Task<TouristRoute> GetTouristRouteAsync(Guid touristRouteId)
        {
            return await _context.TouristRoutes.Include(t => t.TouristRoutePictures).FirstOrDefaultAsync(n => n.Id == touristRouteId);
        }

        public async Task<PaginationList<TouristRoute>> GetTouristRoutesAsync(
            string keyword,
            string ratingOperator,
            int? raringValue,
            int pageSize,
            int pageNumber,
            string orderBy
         )
        {
            IQueryable<TouristRoute> result = _context.TouristRoutes.Include(t => t.TouristRoutePictures);
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                keyword = keyword.Trim();
                result = result.Where(t => t.Title.Contains(keyword));
            }
            if (raringValue >= 0)
            {
                result = ratingOperator switch
                {
                    "largerThan" => result.Where(t => t.Rating >= raringValue),
                    "lessThan" => result.Where(t => t.Rating <= raringValue),
                    _ => result.Where(t => t.Rating == raringValue),
                };
            }
            if (!string.IsNullOrWhiteSpace(orderBy))
            {
                //if (orderBy.ToLowerInvariant()=="originalprice")
                //{
                //    result = result.OrderBy(t => t.OriginalPrice);
                //}
                var touristRouteMappingDictionary = _propertyMappingService.GetPropertyMapping<TouristRouteDto, TouristRoute>();
                result = result.ApplySort(orderBy, touristRouteMappingDictionary);
            }
            //include vs join
            return await PaginationList<TouristRoute>.CreateAsync(pageNumber, pageSize, result);
        }
        public async Task<bool> TouristRouteExistsAsync(Guid touristRouteId)
        {
            return await _context.TouristRoutes.AnyAsync(t => t.Id == touristRouteId);//判断是否存在Id的数据
        }
        public async Task<IEnumerable<TouristRoutePicture>> GetPicturesByTouristRouteIdAsync(Guid touristRouteId)
        {
            return await _context.TouristRoutePictures.Where(p => p.TouristRouteId == touristRouteId).ToListAsync();
        }

        public async Task<TouristRoutePicture> GetPictureAsync(int pictureId)
        {
            return await _context.TouristRoutePictures.Where(p => p.Id == pictureId).FirstOrDefaultAsync();
        }

        public void AddTouristRoute(TouristRoute touristRoute)
        {
            if (touristRoute == null)
            {
                throw new ArgumentException(nameof(touristRoute));
            }
            _context.TouristRoutes.Add(touristRoute);
            //_context.SaveChanges();
        }

        public void AddTouristRoutePicture(Guid touristRouteId, TouristRoutePicture touristRoutePicture)
        {
            if (touristRouteId == Guid.Empty)
            {
                throw new ArgumentException(nameof(touristRouteId));
            }
            if (touristRoutePicture == null)
            {
                throw new ArgumentException(nameof(touristRoutePicture));
            }
            touristRoutePicture.TouristRouteId = touristRouteId;
            _context.TouristRoutePictures.Add(touristRoutePicture);
        }

        public void DeleteTouristRoute(TouristRoute touristRoute)
        {
            _context.TouristRoutes.Remove(touristRoute);
        }

        public void DeleteTOuristRoutePicture(TouristRoutePicture touristRoutePicture)
        {
            _context.TouristRoutePictures.Remove(touristRoutePicture);
        }

        public async Task<IEnumerable<TouristRoute>> GetTouristRoutesByIDListAsync(IEnumerable<Guid> ids)
        {
            return await _context.TouristRoutes.Where(t => ids.Contains(t.Id)).ToListAsync();
        }

        public void DeleteTOuristRoutes(IEnumerable<TouristRoute> touristRoutes)
        {
            _context.TouristRoutes.RemoveRange(touristRoutes);
        }

        public async Task<ShoppingCart> GetShoppingCartByUserId(string userId)
        {
            return await _context.ShoppingCarts
                .Include(s => s.User)
                .Include(s => s.ShoppingCartItems).ThenInclude(li => li.TouristRoute)
                .Where(s => s.UserId == userId)
                .FirstOrDefaultAsync();
        }

        public async Task CreateShoppingCart(ShoppingCart shoppingCart)
        {
            await _context.ShoppingCarts.AddAsync(shoppingCart);
        }

        public async Task AddShoppingCartItem(LineItem lineItem)
        {
            await _context.LineItems.AddAsync(lineItem);
        }

        public async Task<LineItem> GetShoppingCartItemByItemId(int lineItemId)
        {
            return await _context.LineItems.Where(li => li.Id == lineItemId).FirstOrDefaultAsync();
        }

        public void DeleteShoppingCartItem(LineItem lineItem)
        {
            _context.LineItems.Remove(lineItem);
        }

        public async Task<IEnumerable<LineItem>> GetShoppingCartsByIdListAsync(IEnumerable<int> ids)
        {
            return await _context.LineItems.Where(li => ids.Contains(li.Id)).ToListAsync();
        }

        public void DeleteShoppingCartItems(IEnumerable<LineItem> lineItems)
        {
            _context.LineItems.RemoveRange(lineItems);
        }

        public async Task AddOrderAsync(Order order)
        {
            await _context.Orders.AddAsync(order);
        }

        public async Task<PaginationList<Order>> GetOrdersByUserId(string userId, int pageSize, int pageNumber)
        {
            //return await _context.Orders.Where(o => o.UserId == userId).ToListAsync();
            IQueryable<Order> result = _context.Orders.Where(o => o.UserId == userId);
            return await PaginationList<Order>.CreateAsync(pageNumber, pageSize, result);
        }

        public async Task<Order> GetOrderById(Guid orderId)
        {
            return await _context.Orders
                .Include(o => o.OrderItems).ThenInclude(oi => oi.TouristRoute)
                .Where(o => o.Id == orderId)
                .FirstOrDefaultAsync();
        }

        public async Task<bool> SaveAsync()
        {
            // 返回值为 bool 类型，表示如果保存操作成功，那么为 true ，否则为 false 。
            return (await _context.SaveChangesAsync() >= 0);
        }
    }
}
