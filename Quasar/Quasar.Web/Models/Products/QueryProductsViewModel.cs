using Quasar.Web.Utils;

namespace Quasar.Web.Models.Products
{
    public class QueryProductsViewModel
    {
        public string Name { get; set; }

        public PaginatedList<DisplayProductViewModel> Products { get; set; }
    }
}
