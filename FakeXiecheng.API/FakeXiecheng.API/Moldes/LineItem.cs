using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace FakeXiecheng.API.Moldes
{
    public class LineItem
    {
        /// <summary>
        /// 主键
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        /// <summary>
        /// 外键，旅游线路产品 Id
        /// </summary>
        [ForeignKey("TouristRouteId")]
        public Guid TouristRouteId { get; set; }
        /// <summary>
        /// 旅游线路
        /// </summary>
        public TouristRoute TouristRoute { get; set; }
        /// <summary>
        /// 购物车 Id
        /// </summary>
        public Guid? ShoppingCartId { get; set; }
        /// <summary>
        /// 订单 Id
        /// </summary>
        //public Guid? OrderId { get; set; }
        /// <summary>
        /// 原价
        /// </summary>
        [Column(TypeName ="decimal(18,2)")]
        public decimal OriginalPrice { get; set; }
        /// <summary>
        /// 折扣
        /// </summary>
        [Range(0.0,1.0)]
        public double? DiscountPresent { get; set; }
    }
}
