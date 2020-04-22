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
    public class IdentificationTypesController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly ICookieService _cookieService;
        IHttpRequestHandler _httpRequestHandler;

        public IdentificationTypesController(IConfiguration configuration,ICookieService cookieService, IHttpRequestHandler httpRequestHandler)
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
            string url = $"{CoreApiUrl}IdentificationTypes/GetAll";
            var IdentificationTypes = Enumerable.Empty<IdentificationTypeDTO>();

            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var response = await _httpRequestHandler.Get(accessToken, url);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                IdentificationTypes = response.ContentAsType<IEnumerable<IdentificationTypeDTO>>();
            }
            else
            {
                ModelState.AddModelError("", HttpResponseHandler.Process(response));
            }
            return View(IdentificationTypes);
        }

        [HandleExceptionFilter]
        public IActionResult Create()
        {
            return View(new IdentificationTypeDTO
            {
                CreatedBy = _cookieService.Get("UserName")
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [HandleExceptionFilter]
        public async Task<IActionResult> Create([Bind] IdentificationTypeDTO identificationType)
        {
            string url = $"{CoreApiUrl}IdentificationTypes/Add";

            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var response = await _httpRequestHandler.Post(accessToken, url, identificationType);

            if (response.StatusCode == HttpStatusCode.Created)
            {
                AppContextHelper.SetToastMessage("Identification type has been successfully created", MessageType.Success, 1, Response);
                return RedirectToAction(nameof(Index));
            }
            else
            {
                AppContextHelper.SetToastMessage("Failed to create identification type", MessageType.Danger, 1, Response);
                ModelState.AddModelError("", HttpResponseHandler.Process(response));
            }

            return View(identificationType);
        }

        [HandleExceptionFilter]
        public async Task<IActionResult> Edit([FromQuery] int identificationTypeId)
        {
            var IdentificationType = await GetIdentificationType(identificationTypeId);
            return View(IdentificationType);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [HandleExceptionFilter]
        public async Task<IActionResult> Edit([Bind]IdentificationTypeDTO identificationType)
        {
            string url = $"{CoreApiUrl}IdentificationTypes/Update";

            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var response = await _httpRequestHandler.Put(accessToken, url,  identificationType);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                AppContextHelper.SetToastMessage("Identification type has been successfully updated", MessageType.Success, 1, Response);
                return RedirectToAction(nameof(Index));
            }
            else
            {
                AppContextHelper.SetToastMessage("Failed to update identification type", MessageType.Danger, 1, Response);
                ModelState.AddModelError("", HttpResponseHandler.Process(response));
            }
            return View(identificationType);
        }

        [HandleExceptionFilter]
        public async Task<IActionResult> Details([FromQuery] int identificationTypeId)
        {
            var IdentificationType = await GetIdentificationType(identificationTypeId);
            return View(IdentificationType);
        }

        [HandleExceptionFilter]
        public async Task<IActionResult> Delete([FromQuery] int identificationTypeId)
        {
            var IdentificationType = await GetIdentificationType(identificationTypeId);
            return View(IdentificationType);
        }

        [HttpPost, ActionName("Delete")]
        [HandleExceptionFilter]
        public async Task<IActionResult> VerifyDelete(int identificationTypeId)
        {
            string url = $"{CoreApiUrl}IdentificationTypes/Delete?identificationTypeId={identificationTypeId}";
            var IdentificationType = new IdentificationTypeDTO();

            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var response = await _httpRequestHandler.Delete(accessToken, url); 

            if (response.StatusCode == HttpStatusCode.OK)
            {
                AppContextHelper.SetToastMessage("Identification type has been successfully deleted", MessageType.Success, 1, Response);
                return RedirectToAction(nameof(Index));
            }
            else
            {
                AppContextHelper.SetToastMessage("Failed to delete identification Type", MessageType.Danger, 1, Response);
                TempData["ModelError"] = HttpResponseHandler.Process(response);
            }
            return RedirectToAction(nameof(Delete), new { identificationTypeId });
        }

        private async Task<IdentificationTypeDTO> GetIdentificationType(int identificationTypeId)
        {
            string url = $"{CoreApiUrl}IdentificationTypes/GetById?identificationTypeId={identificationTypeId}";
            var IdentificationType = new IdentificationTypeDTO();

            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var response = await _httpRequestHandler.Get(accessToken, url);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                IdentificationType = response.ContentAsType<IdentificationTypeDTO>();
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
            return IdentificationType;
        }


        [HandleExceptionFilter]
        public async Task<IActionResult> GetIdentificationTypes()
        {
            string url = $"{CoreApiUrl}IdentificationTypes/GetAll";
            var IdentificationTypes = Enumerable.Empty<IdentificationTypeDTO>();

            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var response = await _httpRequestHandler.Get(accessToken, url);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                IdentificationTypes = response.ContentAsType<IEnumerable<IdentificationTypeDTO>>();
            }
            else
            {
                ModelState.AddModelError("", HttpResponseHandler.Process(response));
            }
            return PartialView(IdentificationTypes);
        }
    }
}