using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Quasar.Web.Models.Users
{
    public class UserProductViewModel
    {
        public Guid ProductId { get; set; }

        public string ProductName { get; set; }

        public string ProductType { get; set; }

        public decimal ProductPrice { get; set; }
    }
}
