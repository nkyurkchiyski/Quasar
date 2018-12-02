using Quasar.Web.Models.Products;
using System.ComponentModel.DataAnnotations;

namespace Quasar.Web.Models.Orders
{
    public class OrderProductViewModel : DisplayProductViewModel
    {
        [Range(0, int.MaxValue)]
        public int Quantity { get; set; }
    }
}
