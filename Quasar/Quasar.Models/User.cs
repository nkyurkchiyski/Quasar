using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;

namespace Quasar.Models
{
    public class User : IdentityUser
    {
        public User()
        {
            this.WishlistedProducts = new HashSet<UserProduct>();
            this.Orders = new HashSet<Order>();
        }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public Guid AddressId { get; set; }

        public virtual Address Address { get; set; }

        public virtual ICollection<UserProduct> WishlistedProducts { get; set; }
        
        public virtual ICollection<Order> Orders { get; set; }
    }
}
