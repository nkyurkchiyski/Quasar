using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Quasar.Web.Models.Products
{
    public class SortProductViewModel : DisplayProductViewModel
    {
        public DateTime DateAdded { get; set; }

        public int OrdersCount { get; set; }
    }
}
