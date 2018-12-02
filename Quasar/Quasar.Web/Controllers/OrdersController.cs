using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Quasar.Services.Contracts;
using Quasar.Web.Models.Orders;
using Quasar.Web.Utils;

namespace Quasar.Web.Controllers
{
    public class OrdersController : Controller
    {
        private readonly IOrdersService ordersService;

        public OrdersController(IOrdersService ordersService)
        {
            this.ordersService = ordersService;
        }

        [Authorize]
        public async Task<IActionResult> AddProduct(Guid id)
        {
            if (!ModelState.IsValid)
            {
                return Redirect("/");
            }

            var result = await this.ordersService.AddProduct(this.User.Identity.Name, id);

            if (!result)
            {
                return Redirect("/");
            }

            return this.RedirectToAction(nameof(ShoppingCart));
        }

        [Authorize]
        public async Task<IActionResult> RemoveProduct(Guid id)
        {
            if (!ModelState.IsValid)
            {
                return Redirect("/");
            }

            var result = await this.ordersService.RemoveProduct(this.User.Identity.Name, id);

            if (!result)
            {
                return Redirect("/");
            }

            return this.RedirectToAction(nameof(ShoppingCart));
        }

        [Authorize]
        public async Task<IActionResult> ShoppingCart(int? pageIndex)
        {
            var shoppingCart = await this.ordersService
                .GetShoppingCart<OrderViewModel>(this.User.Identity.Name);

            if (shoppingCart != null)
            {
                int pageSize = 6;

                var paginatedProducts = PaginatedList<OrderProductViewModel>.Create(
                    shoppingCart.Products,
                    pageIndex ?? 1,
                    pageSize);

                shoppingCart.PaginatedProducts = paginatedProducts;
            }

            return this.View(shoppingCart);
        }

        [Authorize]
        public async Task<IActionResult> MyOrders()
        {
            var myOrders = await this.ordersService
                .All<OrderViewModel>(this.User.Identity.Name);

            if (myOrders == null)
            {
                return Redirect("/");
            }

            return this.RedirectToAction(controllerName: "Users", actionName: "Details");
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AllOrders(int? pageIndex)
        {
            var orders = await this.ordersService
                .All<OrderViewModel>();

            if (orders == null)
            {
                return Redirect("/");
            }

            int pageSize = 6;

            var paginatedOrders = PaginatedList<OrderViewModel>.Create(
                orders,
                pageIndex ?? 1,
                pageSize);

            return this.View(paginatedOrders);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ChangeStatus(Guid id)
        {
            if (!ModelState.IsValid)
            {
                return Redirect("/");
            }

            var result = await this.ordersService.ChangeStatus(id);

            if (result == false)
            {
                return Redirect("/");
            }

            return this.RedirectToAction(nameof(AllOrders));
        }

        [Authorize]
        public async Task<IActionResult> Order()
        {
            if (!ModelState.IsValid)
            {
                return Redirect("/");
            }

            var result = await this.ordersService.Order(this.User.Identity.Name);

            if (result == false)
            {
                return Redirect("/");
            }

            return this.RedirectToAction(nameof(MyOrders));
        }

        [Authorize]
        public async Task<IActionResult> Acquire(Guid id)
        {
            if (!ModelState.IsValid)
            {
                return Redirect("/");
            }

            var result = await this.ordersService.Acquire(this.User.Identity.Name, id);

            if (result == false)
            {
                return Redirect("/");
            }

            return this.RedirectToAction(nameof(MyOrders));
        }

        [Authorize]
        public async Task<IActionResult> Update(OrderProductViewModel item)
        {
            if (!ModelState.IsValid)
            {
                return Redirect("/");
            }

            var result = await this.ordersService
                .UpdateQuantity(
                    this.User.Identity.Name,
                    item.Id,
                    item.Quantity);

            if (!result)
            {
                return Redirect("/");
            }

            return this.RedirectToAction(nameof(ShoppingCart));
        }

        [Authorize]
        public async Task<IActionResult> Details(Guid id, int? pageIndex)
        {
            var order = await this.ordersService
                .Details<OrderViewModel>(id);

            if (order == null)
            {
                return Redirect("/");
            }

            int pageSize = 6;

            var paginatedProducts = PaginatedList<OrderProductViewModel>.Create(
                order.Products,
                pageIndex ?? 1,
                pageSize);

            order.PaginatedProducts = paginatedProducts;

            return this.View(order);
        }
    }
}
