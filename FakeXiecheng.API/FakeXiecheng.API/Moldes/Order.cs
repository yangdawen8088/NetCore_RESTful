using Stateless;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FakeXiecheng.API.Moldes
{
    public enum OrderStateEnum
    {
        Pending, // 订单已生成
        Processing, // 支付处理中
        Completed, //交易成功
        Declined, // 交易失败
        Cancelled, // 订单取消
        Refund // 已退款
    }
    public enum OrderStateTriggerEnum
    {
        PlaceOrder, // 支付
        Approve,    // 支付成功
        Reject, // 支付失败
        Cancel, // 取消
        Return  // 退货
    }
    public class Order
    {

        public Order()
        {
            StateMachineInit();
        }
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
        /// 订单中的商品项目
        /// </summary>
        public ICollection<LineItem> OrderItems { get; set; }
        /// <summary>
        /// 订单当前状态
        /// </summary>
        public OrderStateEnum State { get; set; }
        /// <summary>
        /// 订单的创建时间，这里是一个经验总结，数据库中的时间一般使用 UTC 时间
        /// </summary>
        public DateTime CreateDateUTC { get; set; }
        /// <summary>
        /// 第三方交易信息
        /// </summary>
        public string TransactionMetadata { get; set; }
        /// <summary>
        /// 私有的状态机变量
        /// </summary>
        StateMachine<OrderStateEnum, OrderStateTriggerEnum> _machine;

        public void PaymentProcessing()
        {
            _machine.Fire(OrderStateTriggerEnum.PlaceOrder);
        }
        public void PaymentApprove()
        {
            _machine.Fire(OrderStateTriggerEnum.Approve);
        }
        public void PaymentReject()
        {
            _machine.Fire(OrderStateTriggerEnum.Reject);
        }
        /// <summary>
        /// 初始化状态机
        /// </summary>
        private void StateMachineInit()
        {
            _machine = new StateMachine<OrderStateEnum, OrderStateTriggerEnum>(
                () => State, s => State = s);

            _machine.Configure(OrderStateEnum.Pending)
                .Permit(OrderStateTriggerEnum.PlaceOrder, OrderStateEnum.Processing)
                .Permit(OrderStateTriggerEnum.Cancel, OrderStateEnum.Cancelled);

            _machine.Configure(OrderStateEnum.Processing)
                .Permit(OrderStateTriggerEnum.Approve, OrderStateEnum.Completed)
                .Permit(OrderStateTriggerEnum.Reject, OrderStateEnum.Declined);

            _machine.Configure(OrderStateEnum.Declined)
                .Permit(OrderStateTriggerEnum.PlaceOrder, OrderStateEnum.Processing);

            _machine.Configure(OrderStateEnum.Completed)
                .Permit(OrderStateTriggerEnum.Return, OrderStateEnum.Refund);
        }
    }
}
