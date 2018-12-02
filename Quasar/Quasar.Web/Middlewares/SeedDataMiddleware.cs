using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Quasar.Data;
using Quasar.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Quasar.Web.Middlewares
{
    public class SeedDataMiddleware
    {
        private readonly RequestDelegate next;

        public SeedDataMiddleware(RequestDelegate next)
        {
            this.next = next;
        }

        public async Task InvokeAsync(HttpContext context, IServiceProvider provider)
        {
            var userManager = provider.GetRequiredService<UserManager<User>>();
            var roleManager = provider.GetRequiredService<RoleManager<IdentityRole>>();
            var dbContext = provider.GetRequiredService<QuasarDbContext>();

            this.SeedRoles(roleManager);
            this.SeedUsers(userManager, dbContext);

            await this.next(context);
        }

        private void SeedRoles(RoleManager<IdentityRole> roleManager)
        {
            if (!roleManager.RoleExistsAsync("User").Result)
            {
                IdentityRole role = new IdentityRole
                {
                    Name = "User"
                };
                IdentityResult roleResult = roleManager.
                CreateAsync(role).Result;
            }

            if (!roleManager.RoleExistsAsync("Admin").Result)
            {
                IdentityRole role = new IdentityRole
                {
                    Name = "Admin"
                };
                IdentityResult roleResult = roleManager.
                CreateAsync(role).Result;
            }
        }

        private void SeedUsers(UserManager<User> userManager, QuasarDbContext context)
        {
            if (userManager.FindByNameAsync("admin").Result == null)
            {
                var address = new Address();

                context.Add(address);
                context.SaveChanges();

                User user = new User
                {
                    UserName = "admin",
                    Address = address
                };

                IdentityResult result = userManager.CreateAsync
                (user, "123456").Result;

                if (result.Succeeded)
                {
                    userManager.AddToRoleAsync(user, "Admin").Wait();
                }
            }
        }


    }
}
