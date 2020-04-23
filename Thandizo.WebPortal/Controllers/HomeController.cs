using IdentityModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using Thandizo.WebPortal.Helpers;
using Thandizo.WebPortal.Models;
using Thandizo.WebPortal.Services;

namespace Thandizo.WebPortal.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        
        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        [Authorize]
        public IActionResult Dashboard()
        {
            var user = HttpContext.User;
            var fullname = AppContextHelper.GetStringValueClaim(HttpContext, JwtClaimTypes.PreferredUserName);
            //_cookieService.Add("UserName", "vvinkhumbo");
            // _cookieService.Add("UserId", "vvin");
            //_cookieService.Add("PhoneNumber", "0884776533");
            //var accessToken = await HttpContext.GetTokenAsync("access_token");
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult Logout()
        {
            return SignOut("Cookies", "oidc");
        }
    }
}
