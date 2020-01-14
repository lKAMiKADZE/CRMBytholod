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

        [HttpGet]
        public IActionResult EditOrder(long ID_ZAKAZ)
        {
            if (!User.Identity.IsAuthenticated)// если неавторизован то редирект на авторизацию
            {
                Uri location = new Uri($"{Request.Scheme}://{Request.Host}/Account/Login");
                return Redirect(location.AbsoluteUri);
            }
            OrderEditVM VM = new OrderEditVM(ID_ZAKAZ);


            return View(VM);
        }
        [HttpPost]
        public IActionResult EditOrder(OrderEditVM VM)
        {
            if (!User.Identity.IsAuthenticated)// если неавторизован то редирект на авторизацию
            {
                Uri location = new Uri($"{Request.Scheme}://{Request.Host}/Account/Login");
                return Redirect(location.AbsoluteUri);
            }
            
            VM.order.USER_ADD.ID_USER = Convert.ToInt64(User.Identity.Name);// присваиваем, кто изменяет данные

            VM.order.Update();

            Uri locat = new Uri($"{Request.Scheme}://{Request.Host}/Home/DetailOrder?ID_ZAKAZ={VM.order.ID_ZAKAZ}");
            return Redirect(locat.AbsoluteUri);

            
        }

        [HttpGet]
        public IActionResult CreateOrder()
        {
            if (!User.Identity.IsAuthenticated)// если неавторизован то редирект на авторизацию
            {
                Uri location = new Uri($"{Request.Scheme}://{Request.Host}/Account/Login");
                return Redirect(location.AbsoluteUri);
            }

            OrderCreateVM VM = new OrderCreateVM();


            return View(VM);
        }

        [HttpPost]
        public IActionResult CreateOrder(OrderCreateVM VM)
        {
            if (!User.Identity.IsAuthenticated)// если неавторизован то редирект на авторизацию
            {
                Uri location = new Uri($"{Request.Scheme}://{Request.Host}/Account/Login");
                return Redirect(location.AbsoluteUri);
            }

            VM.order.USER_ADD.ID_USER = Convert.ToInt64(User.Identity.Name);// присваиваем, кто добавляет пользователя

            long ID_ZAKAZ = VM.order.Save();
                        
            Uri locat = new Uri($"{Request.Scheme}://{Request.Host}/Home/DetailOrder?ID_ZAKAZ={ID_ZAKAZ}");
            return Redirect(locat.AbsoluteUri);
        }

        public IActionResult DetailOrder(long ID_ZAKAZ)
        {
            if (!User.Identity.IsAuthenticated)// если неавторизован то редирект на авторизацию
            {
                Uri location = new Uri($"{Request.Scheme}://{Request.Host}/Account/Login");
                return Redirect(location.AbsoluteUri);
            }

            OrderDetailVM VM = new OrderDetailVM(ID_ZAKAZ);
            //Order order = Order.GetOrderSite(ID_ZAKAZ);

            return View(VM);
        }
                       
        public IActionResult CloseOrder(long ID_ZAKAZ)
        {
            if (!User.Identity.IsAuthenticated)// если неавторизован то редирект на авторизацию
            {
                Uri location = new Uri($"{Request.Scheme}://{Request.Host}/Account/Login");
                return Redirect(location.AbsoluteUri);
            }

            Order.CloseOrderFromDispetcher(ID_ZAKAZ, Convert.ToInt64(User.Identity.Name));

            Uri locat = new Uri($"{Request.Scheme}://{Request.Host}/Home/DetailOrder?ID_ZAKAZ={ID_ZAKAZ}");
            return Redirect(locat.AbsoluteUri);
        }


        ////// Осуществление вызова
        ///////////////////////////

        public IActionResult CallClient(long ID_ZAKAZ)
        {

            CallClientOrderVM VM = new CallClientOrderVM(ID_ZAKAZ);
            return View(VM);
        }


        ////// СОЗДАНИЕ НОВЫХ ПОЛЬЗОВАТЕЛЕЙ
        ///////////////////////////////////


        public IActionResult Users()
        {
            if (!User.Identity.IsAuthenticated)// если неавторизован то редирект на авторизацию
            {
                Uri location = new Uri($"{Request.Scheme}://{Request.Host}/Account/Login");
                return Redirect(location.AbsoluteUri);
            }

            UsersVM VM = new UsersVM(Convert.ToInt64(User.Identity.Name));

            return View(VM);
        }


        [HttpGet]
        public IActionResult CreateUser()
        {
            if (!User.Identity.IsAuthenticated)// если неавторизован то редирект на авторизацию
            {
                Uri location = new Uri($"{Request.Scheme}://{Request.Host}/Account/Login");
                return Redirect(location.AbsoluteUri);
            }

            UserCreateVM VM = new UserCreateVM();


            return View(VM);
        }
        [HttpPost]
        public IActionResult CreateUser(UserCreateVM VM)
        {
            if (!User.Identity.IsAuthenticated)// если неавторизован то редирект на авторизацию
            {
                Uri location = new Uri($"{Request.Scheme}://{Request.Host}/Account/Login");
                return Redirect(location.AbsoluteUri);
            }
            VM.user.Save();

            Uri locat = new Uri($"{Request.Scheme}://{Request.Host}/Home/Users");
            return Redirect(locat.AbsoluteUri);


        }

        [HttpGet]
        public IActionResult EditUser(long ID_USER)
        {
            if (!User.Identity.IsAuthenticated)// если неавторизован то редирект на авторизацию
            {
                Uri location = new Uri($"{Request.Scheme}://{Request.Host}/Account/Login");
                return Redirect(location.AbsoluteUri);
            }

            UserEditVM VM = new UserEditVM(ID_USER);


            return View(VM);
        }
        [HttpPost]
        public IActionResult EditUser(UserEditVM VM)
        {
            if (!User.Identity.IsAuthenticated)// если неавторизован то редирект на авторизацию
            {
                Uri location = new Uri($"{Request.Scheme}://{Request.Host}/Account/Login");
                return Redirect(location.AbsoluteUri);
            }

            VM.user.Update();
            Uri locat = new Uri($"{Request.Scheme}://{Request.Host}/Home/Users");
            return Redirect(locat.AbsoluteUri);

        }

        public IActionResult DeleteUser(long ID_USER)
        {
            if (!User.Identity.IsAuthenticated)// если неавторизован то редирект на авторизацию
            {
                Uri location = new Uri($"{Request.Scheme}://{Request.Host}/Account/Login");
                return Redirect(location.AbsoluteUri);
            }

            CRMBytholod.Models.User.Delete(ID_USER);
            Uri locat = new Uri($"{Request.Scheme}://{Request.Host}/Home/Users");
            return Redirect(locat.AbsoluteUri);

        }




    }
}
