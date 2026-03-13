using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.models
{
    public class Order
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public DateTime OrderDate { get; set; } = DateTime.UtcNow;
        public OrderStatus Status { get; set; } = OrderStatus.Pending;

        public List<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

        // محاسبه قیمت کل با کسر تمام تخفیف‌ها
        public decimal TotalAmount {  get; set; }
    }

    public enum OrderStatus
    {
        Pending,    // در انتظار پرداخت
        Confirmed,  // تایید شده
        Preparing,  // در حال آماده‌سازی
        Shipped,    // ارسال شده
        Cancelled
    }
}

   

