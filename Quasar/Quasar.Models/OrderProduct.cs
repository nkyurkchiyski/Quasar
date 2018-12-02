using System;
using System.Collections.Generic;
using System.Text;

namespace Quasar.Models
{
    public class OrderProduct
    {
        public OrderProduct()
        {
            this.Id = Guid.NewGuid();
        }

        public Guid Id { get; set; }

        public Guid ProductId { get; set; }
        public virtual Product Product { get; set; }

        public Guid OrderId { get; set; }
        public virtual Order Order { get; set; }

        public int Quantity { get; set; }

    }
}
