using Quasar.Web.Models.Products;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Quasar.Web.Models.Home
{
    public class IndexViewModel
    {
        public IndexViewModel()
        {
            this.EditorsChoiceProducts = new List<SortProductViewModel>();
            this.NewProducts = new List<SortProductViewModel>();
            this.TopProducts = new List<SortProductViewModel>();
        }

        public ICollection<SortProductViewModel> EditorsChoiceProducts { get; set; }

        public ICollection<SortProductViewModel> NewProducts { get; set; }

        public ICollection<SortProductViewModel> TopProducts { get; set; }
    }
}
