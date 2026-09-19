using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.IO;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using HadesWeb.Models;
using System.Text.Json;
using HadesWeb.Services;

namespace HadesWeb.Controllers
{
    [Authorize]
    public class DefaultController(IWebHostEnvironment env) : Controller
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

        // POST: /Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Invalid login attempt");
                ModelState.Remove("Email");
                model.Email = string.Empty;
                return View();
            }

            var Users = GetUsers();
            var user = Users.FirstOrDefault(u => u.Email.Equals(model.Email));

            if (user != null && PasswordManager.ValidatePassword(model.Password, user.Password))
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

            ModelState.AddModelError("", "Invalid login attempt");
            ModelState.Remove("Email");
            model.Email = string.Empty;
            return View();

        }

        //Post: /Logout
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Logout()
        {
            HttpContext.SignOutAsync("Cookies").Wait();
            return RedirectToAction("Login", "Default");
        }


        private List<UsersList> GetUsers()
        {
            var Path = System.IO.Path.Combine(env.ContentRootPath, "App_Data", "users.json");
            using var Reader = new StreamReader(System.IO.File.OpenRead(Path));
            return JsonSerializer.Deserialize<List<UsersList>>(Reader.ReadToEnd());
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
