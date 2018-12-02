using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Quasar.Models;
using Quasar.Models.Enums;
using Quasar.Services.Contracts;
using Quasar.Web.Models.Products;
using Quasar.Web.Utils;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Quasar.Web.Controllers
{
    public class ProductsController : Controller
    {
        private readonly IProductsService productsService;

        public ProductsController(IProductsService productsService)
        {
            this.productsService = productsService;
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Create() => this.View();

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(ProductViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var createProductOutcome = await this.productsService.Create(
                model.Name,
                model.Description,
                model.TotalQuantity,
                model.Price,
                model.Category,
                model.Type,
                model.Platform,
                model.CoverImage,
                model.Images);

            return Redirect("/");
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> All(int? pageIndex)
        {
            var products = await this.productsService
                .All<ProductViewModel>();

            int pageSize = 6;
            
            var paginatedProducts = PaginatedList<ProductViewModel>.Create(
                products,
                pageIndex ?? 1,
                pageSize);

            return View(paginatedProducts);
        }

        public async Task<IActionResult> Details(Guid id)
        {
            var product = await this.productsService
                .GetProductInfo<DetailsProductViewModel>(id);

            return View(product);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(Guid id)
        {
            var product = await this.productsService
                .GetProductInfo<EditProductViewModel>(id);

            return View(product);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(EditProductViewModel model)
        {
            if (!this.ModelState.IsValid)
            {
                return Redirect("/");
            }

            if (!model.Cover.Selected && model.NewCover == null)
            {
                return Redirect("/");
            }

            if (model.Cover.Selected && model.NewCover != null)
            {
                return Redirect("/");
            }

            var editProductOutcome = await this.productsService.Edit(
                model.Id,
                model.Name,
                model.Description,
                model.TotalQuantity,
                model.Price,
                model.Category,
                model.Type,
                model.Platform,
                model.Cover.Selected ? "" : model.Cover.PublicId,
                model.Images.Where(x => !x.Selected).Select(x => x.PublicId).ToArray(),
                model.NewCover,
                model.NewImages);

            if (!editProductOutcome)
            {
                return this.Redirect("/");
            }

            return this.RedirectToAction(actionName: $"Details/{model.Id}", controllerName: "Products");
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var product = await this.productsService
                .GetProductInfo<ProductViewModel>(id);
            return View(product);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult DeletePost(Guid id)
        {
            if (!this.ModelState.IsValid)
            {
                return View(id);
            }

            this.productsService.Delete(id);

            return Redirect("/");
        }

        [Route("Products/ProductsBy/{queryType}/{queryValue}")]
        public IActionResult ProductsBy(string queryType, string queryValue, int? pageIndex)
        {
            var products = this.productsService
                .GetProducts<DisplayProductViewModel>(queryType, queryValue);

            if (products == null)
            {
                return Redirect("/");
            }

            int pageSize = 12;

            var paginatedProducts = PaginatedList<DisplayProductViewModel>.Create(
                products,
                pageIndex ?? 1,
                pageSize);

            var queryProductsViewModel = new QueryProductsViewModel
            {
                Name = $"{queryType}: '{queryValue}'",
                Products = paginatedProducts
            };

            return this.View(queryProductsViewModel);
        }

        public IActionResult Search(string queryValue)
        {
            string queryType = "Name";

            return this.RedirectToAction(actionName: "ProductsBy", routeValues: new { queryType, queryValue });
        }

    }
}
