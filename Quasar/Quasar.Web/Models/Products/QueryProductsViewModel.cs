using System.Collections.Generic;

namespace Quasar.Web.Models.Products
{
    public class QueryProductsViewModel
    {
        public string Name { get; set; }

        public ICollection<DisplayProductViewModel> Products { get; set; }
    }
}
