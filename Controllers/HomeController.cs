using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using CRMBytholod.Models;
using CRMBytholod.ViewModels;
using Microsoft.AspNetCore.Authorization;

namespace CRMBytholod.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }


        ////// ЗАЯВКИ
        /////////////

        public IActionResult Orders(int page, int step, string Adres, FiltrOrders filtrOrders)
        {
            if (!User.Identity.IsAuthenticated)// если неавторизован то редирект на авторизацию
            {
                Uri location = new Uri($"{Request.Scheme}://{Request.Host}/Account/Login");
                return Redirect(location.AbsoluteUri);
            }
            OrdersVM VM = new OrdersVM(User.Identity.Name, page, step, filtrOrders);


            return View(VM);
        }


        public IActionResult EditOrder(long ID_ZAKAZ)
        {
            if (!User.Identity.IsAuthenticated)// если неавторизован то редирект на авторизацию
            {
                Uri location = new Uri($"{Request.Scheme}://{Request.Host}/Account/Login");
                return Redirect(location.AbsoluteUri);
            }

            return View();
        }

        public IActionResult CreateOrder()
        {
            if (!User.Identity.IsAuthenticated)// если неавторизован то редирект на авторизацию
            {
                Uri location = new Uri($"{Request.Scheme}://{Request.Host}/Account/Login");
                return Redirect(location.AbsoluteUri);
            }

            return View();
        }

        public IActionResult DetailOrder(long ID_ZAKAZ)
        {
            if (!User.Identity.IsAuthenticated)// если неавторизован то редирект на авторизацию
            {
                Uri location = new Uri($"{Request.Scheme}://{Request.Host}/Account/Login");
                return Redirect(location.AbsoluteUri);
            }
            Order order = Order.GetOrderSite("", ID_ZAKAZ);

            return View(order);
        }


    }
}
