using System;
using System.Collections.Generic;
using System.Text;

namespace Quasar.Models
{
    public class Address
    {
        public Address()
        {
            this.Id = Guid.NewGuid();
            this.Users = new HashSet<User>();
        }

        public Guid Id { get; set; }

        public string Country { get; set; }

        public string City { get; set; }

        public string Street { get; set; }

        public string PostCode { get; set; }

        public virtual ICollection<User> Users { get; set; }
        
    }
}
