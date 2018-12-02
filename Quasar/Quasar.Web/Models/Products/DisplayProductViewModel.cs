using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Quasar.Web.Models.Products
{
    public class DisplayProductViewModel
    {
        public Guid Id { get; set; }
        
        public string Name { get; set; }
        
        public string Platform { get; set; }

        public string Cover { get; set; }
        
        public decimal Price { get; set; }
        
    }
}
