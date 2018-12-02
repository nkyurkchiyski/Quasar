using Quasar.Models.Enums;
using System;
using System.ComponentModel.DataAnnotations;

namespace Quasar.Web.Models.Products
{
    public class DetailsProductViewModel
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        [DataType(DataType.MultilineText)]
        public string Description { get; set; }

        public DateTime DateAdded { get; set; }

        public int TotalQuantity { get; set; }
        
        public decimal Price { get; set; }
        
        public ProductType Type { get; set; }
        
        public Category Category { get; set; }
        
        public Platform Platform { get; set; }
        
        public string CurrentCoverImage { get; set; }
        
        public string[] CurrentImages { get; set; }
    }
}
