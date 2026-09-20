using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using HadesWeb.Models;
using HadesWeb.Services;
using HadesWeb.Data;

namespace HadesWeb.Controllers
{
    [Authorize]
    public class DefaultController(IUserRepository userRepository) : Controller
    {
        //GET: Home page
        [AllowAnonymous]
        public ActionResult Index()
        {
            return View();
        }

        // GET: /Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        // GET: /Register
        [AllowAnonymous]
        public ActionResult Register(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        // POST: /Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Register(RegisterModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var existingUser = userRepository.GetByEmail(model.Email);
            if (existingUser != null)
            {
                ModelState.AddModelError("Email", "Email already registered");
                return View(model);
            }

            var passwordHash = PasswordManager.HashPassword(model.Password);
            var user = userRepository.Create(model.Email, passwordHash, "Web Users");

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Email),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var identity = new ClaimsIdentity(claims, "ApplicationCookie");
            var principal = new ClaimsPrincipal(identity);

            HttpContext.SignInAsync("Cookies", principal, new AuthenticationProperties
            {
                IsPersistent = false,
                ExpiresUtc = DateTimeOffset.UtcNow.AddDays(7)
            }).Wait();

            return RedirectToLocal(returnUrl);
        }

        // POST: /Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginModel model, string returnUrl)
        {
            if (!ModelState.IsValid){
                return LoginError(model);
            }

            var user = userRepository.GetByEmail(model.Email);

            if (user != null && PasswordManager.ValidatePassword(model.Password, user.PasswordHash))
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.Email),
                    new Claim(ClaimTypes.Role, user.Role)
                };

                var identity = new ClaimsIdentity(claims, "ApplicationCookie");
                var principal = new ClaimsPrincipal(identity);

                HttpContext.SignInAsync("Cookies", principal, new AuthenticationProperties
                {
                    IsPersistent = model.RememberMe,
                    ExpiresUtc = DateTimeOffset.UtcNow.AddDays(7)
                }).Wait();

                return RedirectToLocal(returnUrl);
            }

            return LoginError(model);
        }

        //Post: /Logout
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Logout()
        {
            HttpContext.SignOutAsync("Cookies").Wait();
            return RedirectToAction("Login", "Default");
        }

        private ViewResult LoginError(LoginModel model)
        {
            ModelState.AddModelError("", "Invalid login attempt");
            ModelState.Remove("Email");
            model.Email = string.Empty;
            return View();
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }
    }
}
