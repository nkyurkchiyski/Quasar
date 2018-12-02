using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Quasar.Models.Enums;
using Quasar.Services.Contracts;
using Quasar.Web.Models;
using Quasar.Web.Models.Home;
using Quasar.Web.Models.Products;

namespace Quasar.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IProductsService productsService;
        private readonly Random random;

        public HomeController(IProductsService productsService)
        {
            this.productsService = productsService;
            this.random = new Random();
        }

        public async Task<IActionResult> Index()
        {
            var products = await this.productsService.All<SortProductViewModel>();

            int numberOfProducts = products.Count >= 8 ? 8 : products.Count;
            
            var newProducts = products
                    .OrderByDescending(x => x.DateAdded)
                    .Take(numberOfProducts)
                    .ToList();
            
            var topProducts = products
                    .OrderByDescending(x => x.OrdersCount)
                    .Take(numberOfProducts)
                    .ToList();

            var editorsProducts = products
                .OrderBy(x => this.random.Next())
                .Take(numberOfProducts)
                .ToList();

            var indexViewModel = new IndexViewModel
            {
                EditorsChoiceProducts = editorsProducts,
                NewProducts = newProducts,
                TopProducts = topProducts
            };

            return this.View(indexViewModel);
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
