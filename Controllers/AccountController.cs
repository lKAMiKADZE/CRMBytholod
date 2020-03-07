using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using CRMBytholod.Models;
using CRMBytholod.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CRMBytholod.Controllers
{
    public class AccountController : Controller
    {
        /////////////////////
        //Авторизация Регистрация
        /////////////////////
        [HttpGet]
        public IActionResult Login()
        {
            if (HttpContext.User.Identity.IsAuthenticated)
                return RedirectToAction("Index", "Home");
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(AuthorizationVM model)
        {

            try
            {
                if (HttpContext.User.Identity.IsAuthenticated)
                    return RedirectToAction("Index", "Home");


                if (ModelState.IsValid)
                {
                    //model.Password = GethashPassword(model.Password);
                    User user = new User();
                    
                    if (user.AuthUser(model.Login, model.Password))
                    {
                        await Authenticate(user.Login, user.ID_USER.ToString()); // аутентификация

                        return RedirectToAction("Orders", "Home");
                    }
                    ModelState.AddModelError("", "Некорректные логин и(или) пароль");
                }

            }
            catch (Exception e) { ViewData["Message"] = e.Message; }

            return View(model);
        }





        private string GethashPassword(string password)
        {
            string salt = "notifi";// тут соль 

            var valueBytes = KeyDerivation.Pbkdf2(
                             password: password,
                             salt: Encoding.UTF8.GetBytes(salt),
                             prf: KeyDerivationPrf.HMACSHA512,
                             iterationCount: 9090, // итерации
                             numBytesRequested: 256 / 8);

            return Convert.ToBase64String(valueBytes);



        }

        private async Task Authenticate(string Login, string id_user)
        {
            // создаем один claim
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, id_user)
            };
            // создаем объект ClaimsIdentity
            ClaimsIdentity id = new ClaimsIdentity(claims, "ApplicationCookie", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);

            // установка времени действия кук 
            AuthenticationProperties AuthProp = new AuthenticationProperties();

            DateTime dexpire = DateTime.Now.AddYears(1);
            AuthProp.ExpiresUtc = DateTimeOffset.UtcNow.AddYears(1);
            AuthProp.IsPersistent = true;





            // установка аутентификационных куки
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(id), AuthProp);

            CookieOptions optCookie = new CookieOptions();

            optCookie.Expires = dexpire;

            if (HttpContext.Request.Cookies.ContainsKey("Login"))
            {
                HttpContext.Response.Cookies.Delete("Login");
                HttpContext.Response.Cookies.Append("Login", Login,optCookie);
            }
            else
                HttpContext.Response.Cookies.Append("Login", Login, optCookie);

            

        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);


            if (HttpContext.Request.Cookies.ContainsKey("Login"))
                HttpContext.Response.Cookies.Delete("Login");

            return RedirectToAction("Login", "Account");
        }
    }
}