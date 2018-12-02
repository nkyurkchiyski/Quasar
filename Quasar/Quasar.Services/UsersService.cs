using AutoMapper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Quasar.Data;
using Quasar.Models;
using Quasar.Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Quasar.Services
{
    public class UsersService : IUsersService
    {
        private readonly SignInManager<User> signInManager;
        private readonly UserManager<User> userManager;
        private readonly QuasarDbContext context;
        private readonly IMapper mapper;

        public UsersService(SignInManager<User> signInManager,
            UserManager<User> userManager,
            QuasarDbContext context,
            IMapper mapper)
        {
            this.signInManager = signInManager;
            this.userManager = userManager;
            this.context = context;
            this.mapper = mapper;
        }

        public async Task<bool> AddToWishlist(string username, Guid productId)
        {
            var product = await this.context.Products.FindAsync(productId);
            var user = await this.userManager.FindByNameAsync(username);

            if (product == null || user == null)
            {
                return false;
            }

            var userProduct = new UserProduct
            {
                User = user,
                Product = product
            };

            bool alreadyExists = this.context.UsersProducts
                .FirstOrDefault(x => x.ProductId == userProduct.Product.Id &&
                    x.UserId == userProduct.User.Id) != null;

            if (!alreadyExists)
            {
                user.WishlistedProducts.Add(userProduct);
                await this.context.SaveChangesAsync();
            }

            return true;
        }

        public async Task<bool> RemoveFromWishlist(string username, Guid productId)
        {
            var product = await this.context.Products.FindAsync(productId);
            var user = await this.userManager.FindByNameAsync(username);

            if (product == null || user == null)
            {
                return false;
            }

            var userProduct = this.context
                .UsersProducts
                .FirstOrDefault(x =>
                    x.ProductId == productId &&
                    x.UserId == user.Id);

            if (userProduct == null)
            {
                return false;
            }

            this.context.UsersProducts.Remove(userProduct);
            await this.context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> Edit(
            string username,
            string email,
            string firstName,
            string lastName,
            string country,
            string city,
            string street,
            string postCode)
        {
            var user = this.context.Users.FirstOrDefault(x => x.UserName == username);

            if (user == null)
            {
                return false;
            }

            user.Email = email;
            user.FirstName = firstName;
            user.LastName = lastName;

            var address = new Address
            {
                Street = street,
                City = city,
                Country = country,
                PostCode = postCode
            };

            var newAddress = await this.CreateOrGetAddressAsync(address);

            user.AddressId = newAddress.Id;

            var upadateResult = await this.userManager.UpdateAsync(user);

            await this.context.SaveChangesAsync();

            return upadateResult.Succeeded;
        }

        public async Task<T> GetUserByUsername<T>(string username)
        {
            var user = await this.userManager.FindByNameAsync(username);

            var model = this.mapper.Map<T>(user);

            return model;
        }

        public async Task<ICollection<T>> GetWishlist<T>(string username)
        {
            var user = await this.userManager.FindByNameAsync(username);

            var userProducts = await this.context.UsersProducts
                .Where(x => x.UserId == user.Id)
                .ToListAsync();

            var models = userProducts
                .Select(x => this.mapper.Map<T>(x))
                .ToList();

            return models;
        }

        public async Task<bool> Login(string username, string password, bool rememberMe)
        {
            var user = await this.userManager.FindByNameAsync(username);

            if (user == null)
            {
                return false;
            }

            var signInResult = await this.signInManager.PasswordSignInAsync(user, password, rememberMe, lockoutOnFailure: false);

            return signInResult.Succeeded;
        }

        public async void Logout()
        {
            await this.signInManager.SignOutAsync();
        }

        public async Task<bool> Register(
            string username,
            string password,
            string confirmPassword,
            string email,
            string firstName,
            string lastName,
            string country,
            string city,
            string street,
            string postCode)
        {
            if (this.context.Users.FirstOrDefault(x => x.UserName == username) != null)
            {
                return false;
            }

            var user = new User
            {
                UserName = username,
                Email = email,
                FirstName = firstName,
                LastName = lastName
            };

            var address = new Address
            {
                Street = street,
                City = city,
                Country = country,
                PostCode = postCode
            };

            user.Address = await this.CreateOrGetAddressAsync(address);

            var userCreateResult = await this.userManager.CreateAsync(user, password);

            if (!userCreateResult.Succeeded)
            {
                return false;
            }

            IdentityResult addRoleResult = null;

            addRoleResult = await this.userManager.AddToRoleAsync(user, "User");

            if (!addRoleResult.Succeeded)
            {
                return false;
            }

            return true;
        }

        private async Task<Address> CreateOrGetAddressAsync(Address userAddress)
        {
            var address = await this.context.Addresses
                .FirstOrDefaultAsync(x => x.Country == userAddress.Country &&
                                     x.Street == userAddress.Street &&
                                     x.PostCode == userAddress.PostCode &&
                                     x.City == userAddress.City);
            if (address == null)
            {
                this.context.Addresses.Add(userAddress);
                await this.context.SaveChangesAsync();
            }
            return userAddress;

        }

        public async Task<SignInResult> ExternalLoginCallback(ExternalLoginInfo info)
        {
            var result = await this.signInManager
                .ExternalLoginSignInAsync(
                    info.LoginProvider,
                    info.ProviderKey,
                    isPersistent: false,
                    bypassTwoFactor: true);

            return result;
        }

        public async Task<ExternalLoginInfo> GetExternalLoginInfo()
        {
            return await this.signInManager.GetExternalLoginInfoAsync();
        }

        public async Task<bool> ExternalLoginConfirmation(
            ExternalLoginInfo info,
            string email,
            string country,
            string city,
            string street,
            string postCode)
        {
            var user = new User
            {
                UserName = email,
                Email = email,
                Address = new Address
                {
                    Country = country,
                    Street = street,
                    City = city,
                    PostCode = postCode
                }
            };

            var result = await this.userManager.CreateAsync(user);

            if (result.Succeeded)
            {
                if (this.userManager.Users.Count() == 1)
                {
                    await this.userManager.AddToRoleAsync(user, "Admin");
                }
                else
                {
                    await this.userManager.AddToRoleAsync(user, "User");
                }

                result = await this.userManager.AddLoginAsync(user, info);

                if (result.Succeeded)
                {
                    await this.signInManager.SignInAsync(user, isPersistent: false);
                    return true;
                }
            }

            return false;
        }

        public AuthenticationProperties ConfigureExternalAuthenticationProperties(string provider, string redirectUrl)
        {
            return this.signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
        }
    }
}
