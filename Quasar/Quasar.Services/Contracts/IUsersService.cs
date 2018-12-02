using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Quasar.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Quasar.Services.Contracts
{
    public interface IUsersService
    {
        Task<bool> Register(string username,
           string password,
           string confirmPassword,
           string email,
           string firstName,
           string lastName,
           string country,
           string city,
           string street,
           string postCode);

        Task<bool> Edit(string username,
          string email,
          string firstName,
          string lastName,
          string country,
          string city,
          string street,
          string postCode);

        Task<bool> Login(string username, string password, bool rememberMe);

        Task<T> GetUserByUsername<T>(string username);

        void Logout();

        Task<bool> AddToWishlist(string username, Guid productId);

        Task<bool> RemoveFromWishlist(string username, Guid productId);

        Task<ICollection<T>> GetWishlist<T>(string username);

        Task<SignInResult> ExternalLoginCallback(ExternalLoginInfo info);

        Task<bool> ExternalLoginConfirmation(
          ExternalLoginInfo info,
          string email,
          string country,
          string city,
          string street,
          string postCode);

        Task<ExternalLoginInfo> GetExternalLoginInfo();

        AuthenticationProperties ConfigureExternalAuthenticationProperties(string provider, string redirectUrl);
    }
}
