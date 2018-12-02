using Quasar.Web.Models.Addresses;
using Quasar.Web.Models.Orders;
using Quasar.Web.Models.Products;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Quasar.Web.Models.Users
{
    public class DetailsUserViewModel
    {
        public string UserName { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public AddressViewModel Address { get; set; }

        public ICollection<UserProductViewModel> WishlistedProducts { get; set; }

        public ICollection<OrderViewModel> Orders { get; set; }
    }
}
