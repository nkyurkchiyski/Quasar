using Quasar.Web.Models.Products;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Quasar.Web.Models.Orders
{
    public class OrderViewModel
    {
        public Guid Id { get; set; }

        public DateTime? OrderedOn { get; set; }

        public DateTime? DeliveredOn { get; set; }

        public string OrderStatus { get; set; }

        public ICollection<OrderProductViewModel> Products { get; set; }

        public decimal? TotalPrice => Products.Sum(x => x.Quantity * x.Price);
    }
}
