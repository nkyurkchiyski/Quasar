using Quasar.Models.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Quasar.Models
{
    public class Order
    {
        public Order()
        {
            this.Id = Guid.NewGuid();
            this.Products = new HashSet<OrderProduct>();
            this.OrderStatus = OrderStatus.AwaitingUser;
        }

        public Guid Id { get; set; }

        public DateTime? OrderedOn { get; set; }

        public DateTime? DeliveredOn { get; set; }
        
        public string UserId { get; set; }
        public virtual User User { get; set; }

        public virtual ICollection<OrderProduct> Products { get; set; }

        public OrderStatus OrderStatus { get; set; }
    }
}
