using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Thandizo.WebPortal.Models;
using Thandizo.WebPortal.Services;

namespace Thandizo.WebPortal.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ICookieService _cookieService;

        public HomeController(ILogger<HomeController> logger, ICookieService cookieService)
        {
            _logger = logger;
            _cookieService = cookieService;
        }
        
        public IActionResult Dashboard()
        {
            _cookieService.Add("UserName", "vvinkhumbo");
           // _cookieService.Add("UserId", "vvin");
           // _cookieService.Add("PhoneNumber", "0884776533");
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
