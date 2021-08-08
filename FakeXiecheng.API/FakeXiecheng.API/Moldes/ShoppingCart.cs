using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FakeXiecheng.API.Moldes
{
    public class ShoppingCart
    {
        /// <summary>
        /// 购物车主键
        /// </summary>
        [Key]
        public Guid Id { get; set; }
        /// <summary>
        /// 用户 Id
        /// </summary>
        public string UserId { get; set; }
        /// <summary>
        /// 用户信息
        /// </summary>
        public ApplicationUser User { get; set; }
        /// <summary>
        /// 购物车商品项目
        /// </summary>
        public ICollection<LineItem> ShoppingCartItems { get; set; }
    }
}
