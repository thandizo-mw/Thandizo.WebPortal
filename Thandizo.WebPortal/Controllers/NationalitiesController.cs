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
    public class NationalitiesController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly ICookieService _cookieService;
        IHttpRequestHandler _httpRequestHandler;

        public NationalitiesController(IConfiguration configuration,ICookieService cookieService, IHttpRequestHandler httpRequestHandler)
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
            string url = $"{CoreApiUrl}Nationalities/GetAll";
            var Nationalities = Enumerable.Empty<NationalityDTO>();

            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var response = await _httpRequestHandler.Get(accessToken, url);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                Nationalities = response.ContentAsType<IEnumerable<NationalityDTO>>();
            }
            else
            {
                ModelState.AddModelError("", HttpResponseHandler.Process(response));
            }
            return View(Nationalities);
        }

        [HandleExceptionFilter]
        public IActionResult Create()
        {
            return View(new NationalityDTO
            {
                CreatedBy = _cookieService.Get("UserName")
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [HandleExceptionFilter]
        public async Task<IActionResult> Create([Bind] NationalityDTO nationality)
        {
            string url = $"{CoreApiUrl}Nationalities/Add";

            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var response = await _httpRequestHandler.Post(accessToken, url, nationality);

            if (response.StatusCode == HttpStatusCode.Created)
            {
                AppContextHelper.SetToastMessage("Nationality has been successfully created", MessageType.Success, 1, Response);
                return RedirectToAction(nameof(Index));
            }
            else
            {
                AppContextHelper.SetToastMessage("Failed to create nationality", MessageType.Danger, 1, Response);
                ModelState.AddModelError("", HttpResponseHandler.Process(response));
            }

            return View(nationality);
        }

        [HandleExceptionFilter]
        public async Task<IActionResult> Edit([FromQuery] string nationalityCode)
        {
            var Nationality = await GetNationality(nationalityCode);
            return View(Nationality);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [HandleExceptionFilter]
        public async Task<IActionResult> Edit([Bind]NationalityDTO nationality)
        {
            string url = $"{CoreApiUrl}Nationalities/Update";

            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var response = await _httpRequestHandler.Put(accessToken, url,  nationality);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                AppContextHelper.SetToastMessage("Nationality has been successfully updated", MessageType.Success, 1, Response);
                return RedirectToAction(nameof(Index));
            }
            else
            {
                AppContextHelper.SetToastMessage("Failed to update nationality", MessageType.Danger, 1, Response);
                ModelState.AddModelError("", HttpResponseHandler.Process(response));
            }
            return View(nationality);
        }

        [HandleExceptionFilter]
        public async Task<IActionResult> Details([FromQuery] string nationalityCode)
        {
            var Nationality = await GetNationality(nationalityCode);
            return View(Nationality);
        }

        [HandleExceptionFilter]
        public async Task<IActionResult> Delete([FromQuery] string nationalityCode)
        {
            var Nationality = await GetNationality(nationalityCode);
            return View(Nationality);
        }

        [HttpPost, ActionName("Delete")]
        [HandleExceptionFilter]
        public async Task<IActionResult> VerifyDelete(string nationalityCode)
        {
            string url = $"{CoreApiUrl}Nationalities/Delete?nationalityCode={nationalityCode}";
            var Nationality = new NationalityDTO();

            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var response = await _httpRequestHandler.Delete(accessToken, url); 

            if (response.StatusCode == HttpStatusCode.OK)
            {
                AppContextHelper.SetToastMessage("Nationality has been successfully deleted", MessageType.Success, 1, Response);
                return RedirectToAction(nameof(Index));
            }
            else
            {
                AppContextHelper.SetToastMessage("Failed to delete nationality", MessageType.Danger, 1, Response);
                TempData["ModelError"] = HttpResponseHandler.Process(response);
            }
            return RedirectToAction(nameof(Delete), new { nationalityCode });
        }

        private async Task<NationalityDTO> GetNationality(string nationalityCode)
        {
            string url = $"{CoreApiUrl}Nationalities/GetByCode?nationalityCode={nationalityCode}";
            var Nationality = new NationalityDTO();

            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var response = await _httpRequestHandler.Get(accessToken, url);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                Nationality = response.ContentAsType<NationalityDTO>();
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
            return Nationality;
        }


        [HandleExceptionFilter]
        public async Task<IActionResult> GetNationalities()
        {
            string url = $"{CoreApiUrl}Nationalities/GetAll";
            var Nationalities = Enumerable.Empty<NationalityDTO>();

            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var response = await _httpRequestHandler.Get(accessToken, url);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                Nationalities = response.ContentAsType<IEnumerable<NationalityDTO>>();
            }
            else
            {
                ModelState.AddModelError("", HttpResponseHandler.Process(response));
            }
            return PartialView(Nationalities);
        }
    }
}