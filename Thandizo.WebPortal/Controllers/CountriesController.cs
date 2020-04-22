using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Thandizo.DataModels.Core;
using Thandizo.WebPortal.Filters;
using Thandizo.WebPortal.Helpers;
using Thandizo.WebPortal.Helpers.General;
using Thandizo.WebPortal.Services;

namespace Thandizo.WebPortal.Controllers
{
    public class CountriesController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly ICookieService _cookieService;
        IHttpRequestHandler _httpRequestHandler;

        public CountriesController(IConfiguration configuration,ICookieService cookieService, IHttpRequestHandler httpRequestHandler)
        {
            _configuration = configuration;
            _httpRequestHandler = httpRequestHandler;
            _cookieService = cookieService;
        }

        public string CoreApiUrl
        {
            get
            {
                return _configuration["CoreApiUrl"];
            }
        }

        [HandleExceptionFilter]
        public async Task<IActionResult> Index()
        {
            string url = $"{CoreApiUrl}Countries/GetAll";
            var Countries = Enumerable.Empty<CountryDTO>();

            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var response = await _httpRequestHandler.Get(accessToken, url);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                Countries = response.ContentAsType<IEnumerable<CountryDTO>>();
            }
            else
            {
                ModelState.AddModelError("", HttpResponseHandler.Process(response));
            }
            return View(Countries);
        }

        [HandleExceptionFilter]
        public IActionResult Create()
        {
            return View(new CountryDTO
            {
                CreatedBy = _cookieService.Get("UserName")
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [HandleExceptionFilter]
        public async Task<IActionResult> Create([Bind] CountryDTO country)
        {
            string url = $"{CoreApiUrl}Countries/Add";

            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var response = await _httpRequestHandler.Post(accessToken, url, country);

            if (response.StatusCode == HttpStatusCode.Created)
            {
                AppContextHelper.SetToastMessage("Country has been successfully created", MessageType.Success, 1, Response);
                return RedirectToAction(nameof(Index));
            }
            else
            {
                AppContextHelper.SetToastMessage("Failed to create country", MessageType.Danger, 1, Response);
                ModelState.AddModelError("", HttpResponseHandler.Process(response));
            }

            return View(country);
        }

        [HandleExceptionFilter]
        public async Task<IActionResult> Edit([FromQuery] string countryCode)
        {
            var Country = await GetCountry(countryCode);
            return View(Country);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [HandleExceptionFilter]
        public async Task<IActionResult> Edit([Bind]CountryDTO country)
        {
            string url = $"{CoreApiUrl}Countries/Update";

            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var response = await _httpRequestHandler.Put(accessToken, url,  country);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                AppContextHelper.SetToastMessage("Country has been successfully updated", MessageType.Success, 1, Response);
                return RedirectToAction(nameof(Index));
            }
            else
            {
                AppContextHelper.SetToastMessage("Failed to update country", MessageType.Danger, 1, Response);
                ModelState.AddModelError("", HttpResponseHandler.Process(response));
            }
            return View(country);
        }

        [HandleExceptionFilter]
        public async Task<IActionResult> Details([FromQuery] string countryCode)
        {
            var Country = await GetCountry(countryCode);
            return View(Country);
        }

        [HandleExceptionFilter]
        public async Task<IActionResult> Delete([FromQuery] string countryCode)
        {
            var Country = await GetCountry(countryCode);
            return View(Country);
        }

        [HttpPost, ActionName("Delete")]
        [HandleExceptionFilter]
        public async Task<IActionResult> VerifyDelete(string countryCode)
        {
            string url = $"{CoreApiUrl}Countries/Delete?countryCode={countryCode}";
            var Country = new CountryDTO();

            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var response = await _httpRequestHandler.Delete(accessToken, url); 

            if (response.StatusCode == HttpStatusCode.OK)
            {
                AppContextHelper.SetToastMessage("Country has been successfully deleted", MessageType.Success, 1, Response);
                return RedirectToAction(nameof(Index));
            }
            else
            {
                AppContextHelper.SetToastMessage("Failed to delete country", MessageType.Danger, 1, Response);
                TempData["ModelError"] = HttpResponseHandler.Process(response);
            }
            return RedirectToAction(nameof(Delete), new { countryCode });
        }

        private async Task<CountryDTO> GetCountry(string countryCode)
        {
            string url = $"{CoreApiUrl}Countries/GetByCode?countryCode={countryCode}";
            var Country = new CountryDTO();

            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var response = await _httpRequestHandler.Get(accessToken, url);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                Country = response.ContentAsType<CountryDTO>();
            }
            else
            {
                ModelState.AddModelError("", HttpResponseHandler.Process(response));
            }
            if (TempData["ModelError"] != null)
            {
                ModelState.AddModelError("", TempData["ModelError"].ToString());
                TempData["ModelError"] = null;
            }
            return Country;
        }


        [HandleExceptionFilter]
        public async Task<IActionResult> GetCountries()
        {
            string url = $"{CoreApiUrl}Countries/GetAll";
            var Countries = Enumerable.Empty<CountryDTO>();

            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var response = await _httpRequestHandler.Get(accessToken, url);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                Countries = response.ContentAsType<IEnumerable<CountryDTO>>();
            }
            else
            {
                ModelState.AddModelError("", HttpResponseHandler.Process(response));
            }
            return PartialView(Countries);
        }
    }
}