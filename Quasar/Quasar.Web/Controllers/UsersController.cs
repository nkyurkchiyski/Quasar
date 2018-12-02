using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Quasar.Models;
using Quasar.Services.Contracts;
using Quasar.Web.Models.Products;
using Quasar.Web.Models.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Quasar.Web.Controllers
{
    public class UsersController : Controller
    {
        private readonly IUsersService usersService;

        public UsersController(IUsersService usersService)
        {
            this.usersService = usersService;
        }

        public IActionResult Login() => View();

        [HttpPost]
        public IActionResult Login(LoginUserViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            var loginOutcome = this.usersService.Login(
                model.Username,
                model.Password,
                model.RememberMe);

            if (!loginOutcome.Result)
            {
                return View();
            }

            return Redirect("/");
        }

        [Authorize]
        public IActionResult Logout()
        {
            this.usersService.Logout();
            return Redirect("/");
        }

        public IActionResult Register() => View();

        [HttpPost]
        public IActionResult Register(RegisterUserViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            var registerOutcome = this.usersService.Register(
                model.Username,
                model.Password,
                model.ConfirmPassword,
                model.Email,
                model.FirstName,
                model.LastName,
                model.Country,
                model.City,
                model.Street,
                model.PostCode);

            if (!registerOutcome.Result)
            {
                return View();
            }

            var loginViewModel = new LoginUserViewModel
            {
                Username = model.Username,
                Password = model.Password,
                RememberMe = false
            };

            return Login(loginViewModel);
        }

        [Authorize]
        public async Task<IActionResult> Details()
        {
            var wishlist = await this.usersService
                .GetUserByUsername<DetailsUserViewModel>(this.User.Identity.Name);

            return this.View(wishlist);
        }

        [Authorize]
        public async Task<IActionResult> Edit()
        {
            var user = await this.usersService
                .GetUserByUsername<EditUserViewModel>(User.Identity.Name);

            return this.View(user);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Edit(EditUserViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var editOutcome = await this.usersService.Edit(
                model.UserName,
                model.Email,
                model.FirstName,
                model.LastName,
                model.Address.Country,
                model.Address.City,
                model.Address.Street,
                model.Address.PostCode);

            if (!editOutcome)
            {
                return View(model);
            }

            return RedirectToAction(nameof(Details));
        }

        [Authorize]
        public async Task<IActionResult> AddToWishlist(Guid id)
        {
            var addOutcome = await this.usersService.AddToWishlist(this.User.Identity.Name, id);

            if (!addOutcome)
            {
                return NotFound();
            }

            return this.RedirectToAction("Details");
        }

        [Authorize]
        public async Task<IActionResult> RemoveFromWishlist(Guid id)
        {
            var removeOutcome = await this.usersService.RemoveFromWishlist(this.User.Identity.Name, id);

            if (!removeOutcome)
            {
                return NotFound();
            }

            return this.RedirectToAction("Details");
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Lockout()
        {
            return View();
        }

        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public IActionResult ExternalLogin(string provider)
        {
            var returnUrl = "/";

            var redirectUrl = Url.Action(nameof(ExternalLoginCallback), "Users", new { returnUrl });

            var properties = this.usersService.ConfigureExternalAuthenticationProperties(provider, redirectUrl);

            return Challenge(properties, provider);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ExternalLoginCallback(string returnUrl = null, string remoteError = null)
        {
            if (remoteError != null)
            {
                return RedirectToAction(nameof(Login));
            }

            ExternalLoginInfo info = await this.usersService
                .GetExternalLoginInfo();

            if (info == null)
            {
                return RedirectToAction(nameof(Login));
            }

            var result = await this.usersService.ExternalLoginCallback(info);

            if (result.Succeeded)
            {
                return this.Redirect("/");
            }

            if (result.IsLockedOut)
            {
                return RedirectToAction(nameof(Lockout));
            }
            else
            {
                ViewData["ReturnUrl"] = returnUrl;
                ViewData["LoginProvider"] = info.LoginProvider;
                string email = info.Principal.FindFirstValue(ClaimTypes.Email);
                return View("ExternalLogin", new ExternalLoginViewModel { Email = email });
            }
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ExternalLoginConfirmation(ExternalLoginViewModel model, string returnUrl = null)
        {
            if (ModelState.IsValid)
            {
                // Get the information about the user from the external login provider
                var info = await this.usersService
                    .GetExternalLoginInfo();

                if (info == null)
                {
                    throw new ApplicationException("Error loading external login information during confirmation.");
                }

                var result = await this.usersService
                    .ExternalLoginConfirmation(
                        info,
                        model.Email,
                        model.Country,
                        model.City,
                        model.Street,
                        model.PostCode);

                if (!result)
                {
                    return RedirectToAction(nameof(Login));
                }

            }

            ViewData["ReturnUrl"] = returnUrl;
            return View(nameof(ExternalLogin), model);
        }

        private IActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }
        }
    }
}