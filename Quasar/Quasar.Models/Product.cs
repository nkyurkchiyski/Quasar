using Quasar.Models.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Quasar.Models
{
    public class Product
    {
        public Product()
        {
            this.Id = Guid.NewGuid();
            this.WishlistUsers = new HashSet<UserProduct>();
            this.Orders = new HashSet<OrderProduct>();
            this.Images = new HashSet<Image>();
        }

        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }
        
        public DateTime DateAdded { get; set; }

        public string CoverPublicId { get; set; }
        public string Cover { get; set; }

        public int TotalQuantity { get; set; }
        public decimal Price { get; set; }

        public ProductType Type { get; set; }
        public Category Category { get; set; }
        public Platform Platform { get; set; }

        public virtual ICollection<UserProduct> WishlistUsers { get; set; }
        public virtual ICollection<OrderProduct> Orders { get; set; }
        public virtual ICollection<Image> Images { get; set; }
        
        [NotMapped]
        public bool InStock => this.TotalQuantity > 0;



    }
}
