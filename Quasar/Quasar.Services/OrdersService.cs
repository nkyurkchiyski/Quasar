using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Quasar.Data;
using Quasar.Models;
using Quasar.Models.Enums;
using Quasar.Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quasar.Services
{
    public class OrdersService : IOrdersService
    {
        private readonly QuasarDbContext context;
        private readonly UserManager<User> userManager;
        private readonly IMapper mapper;

        public OrdersService(QuasarDbContext context, UserManager<User> userManager, IMapper mapper)
        {
            this.context = context;
            this.userManager = userManager;
            this.mapper = mapper;
        }

        private async Task<Order> GetFirstUserOrderByStatus(string username, OrderStatus status)
        {
            var order = await this.context.Orders
                .FirstOrDefaultAsync(x => x.OrderStatus == status && x.User.UserName == username);

            return order;
        }

        private async Task<Order> GetOrder(Guid id)
        {
            var order = await this.context.Orders
                .FindAsync(id);

            return order;
        }

        private async Task<Order> CreateOrder(string username)
        {
            var user = await this.userManager.FindByNameAsync(username);
            var order = new Order();

            user.Orders.Add(order);
            await this.context.SaveChangesAsync();

            return order;
        }

        public async Task<bool> AddProduct(string username, Guid id)
        {
            var shoppingCart = await
                this.GetFirstUserOrderByStatus(username, OrderStatus.AwaitingUser);

            var product = await this.context
                .Products
                .FindAsync(id);

            if (product == null || product.TotalQuantity == 0)
            {
                return false;
            }

            if (shoppingCart == null)
            {
                shoppingCart = await this.CreateOrder(username);
            }

            if (shoppingCart == null)
            {
                return false;
            }

            var orderProduct = await this.context.OrdersProducts
                .FirstOrDefaultAsync(x => x.OrderId == shoppingCart.Id &&
                                          x.ProductId == product.Id);

            if (orderProduct == null)
            {
                orderProduct = new OrderProduct
                {
                    Order = shoppingCart,
                    Product = product,
                    Quantity = 1
                };
            }
            else
            {
                orderProduct.Quantity++;
            }

            shoppingCart.Products.Add(orderProduct);
            await this.context.SaveChangesAsync();

            return true;
        }

        public async Task<ICollection<T>> All<T>(string username = null)
        {
            var orders = await this.context.Orders
                .ToListAsync();

            if (username != null)
            {
                orders = orders.Where(x => x.User.UserName == username)
                    .ToList();
            }

            var models = orders.Select(x => this.mapper.Map<T>(x))
                .ToList();

            return models;
        }

        public async Task<bool> ChangeStatus(Guid id)
        {
            var order = await GetOrder(id);

            if (order == null)
            {
                return false;
            }

            switch (order.OrderStatus)
            {
                case OrderStatus.Pending:
                    order.OrderStatus = OrderStatus.Shipped;
                    break;
                case OrderStatus.Shipped:
                    order.OrderStatus = OrderStatus.Delivered;
                    order.DeliveredOn = DateTime.Now;
                    break;
                case OrderStatus.Delivered:
                    order.OrderStatus = OrderStatus.Acquired;
                    break;
                case OrderStatus.Acquired:
                case OrderStatus.AwaitingUser:
                default:
                    return false;
            }

            await this.context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> Order(string username)
        {
            var user = await this.userManager.FindByNameAsync(username);

            if (user == null)
            {
                return false;
            }

            var order = await this.GetFirstUserOrderByStatus(username, OrderStatus.AwaitingUser);

            if (order == null)
            {
                return false;
            }

            order.OrderedOn = DateTime.Now;
            order.OrderStatus = OrderStatus.Pending;

            foreach (var op in order.Products)
            {
                op.Product.TotalQuantity -= op.Quantity;
            }

            await this.context.SaveChangesAsync();

            return true;

        }

        public async Task<bool> RemoveProduct(string username, Guid id)
        {
            var user = await this.userManager.FindByNameAsync(username);

            if (user == null)
            {
                return false;
            }

            var order = await this.GetFirstUserOrderByStatus(username, OrderStatus.AwaitingUser);

            if (order == null)
            {
                return false;
            }

            var orderProduct = await this.context.OrdersProducts
                .FirstOrDefaultAsync(x => x.Order == order &&
                                          x.ProductId == id);

            if (orderProduct == null)
            {
                return false;
            }

            this.context.OrdersProducts.Remove(orderProduct);
            await this.context.SaveChangesAsync();

            if (!order.Products.Any())
            {
                this.context.Orders.Remove(order);
                await this.context.SaveChangesAsync();
            }

            return true;
        }

        public async Task<bool> UpdateQuantity(string username, Guid id, int newAmmount)
        {
            if (newAmmount <= 0)
            {
                return await this.RemoveProduct(username, id);
            }

            var user = await this.userManager.FindByNameAsync(username);

            if (user == null)
            {
                return false;
            }

            var order = await this.GetFirstUserOrderByStatus(username, OrderStatus.AwaitingUser);

            if (order == null)
            {
                return false;
            }

            var orderProduct = await this.context.OrdersProducts
                .FirstOrDefaultAsync(x => x.Order == order &&
                                          x.ProductId == id);

            if (orderProduct == null)
            {
                return false;
            }

            var product = orderProduct.Product;

            if (product.TotalQuantity < newAmmount)
            {
                return false;
            }

            orderProduct.Quantity = newAmmount;

            await this.context.SaveChangesAsync();
            return true;
        }

        public async Task<T> GetShoppingCart<T>(string username)
        {
            var user = await this.userManager.FindByNameAsync(username);

            if (user == null)
            {
                return default(T);
            }

            var order = await this.GetFirstUserOrderByStatus(username, OrderStatus.AwaitingUser);

            if (order == null)
            {
                return default(T);
            }

            var model = this.mapper.Map<T>(order);

            return model;
        }

        public async Task<bool> Acquire(string username, Guid id)
        {
            var user = await this.userManager.FindByNameAsync(username);

            if (user == null)
            {
                return false;
            }

            var order = await this.GetOrder(id);

            if (order == null)
            {
                return false;
            }

            order.OrderStatus = OrderStatus.Acquired;

            await this.context.SaveChangesAsync();

            return true;
        }

        public async Task<T> Details<T>(Guid id)
        {
            var order = await this.GetOrder(id);

            if (order == null)
            {
                return default(T);
            }

            var model = this.mapper.Map<T>(order);

            return model;
        }
    }
}
