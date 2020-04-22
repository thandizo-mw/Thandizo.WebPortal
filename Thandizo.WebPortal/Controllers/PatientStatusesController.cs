using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Thandizo.DataModels.Core;
using Thandizo.DataModels.Patients;
using Thandizo.WebPortal.Filters;
using Thandizo.WebPortal.Helpers;
using Thandizo.WebPortal.Helpers.General;
using Thandizo.WebPortal.Services;

namespace Thandizo.WebPortal.Controllers
{
    public class PatientStatusesController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly ICookieService _cookieService;
        IHttpRequestHandler _httpRequestHandler;

        public PatientStatusesController(IConfiguration configuration,ICookieService cookieService, IHttpRequestHandler httpRequestHandler)
        {
            _configuration = configuration;
            _httpRequestHandler = httpRequestHandler;
            _cookieService = cookieService;
        }

        public string PatientsApiUrl
        {
            get
            {
                return _configuration["PatientsApiUrl"];
            }
        }

        [HandleExceptionFilter]
        public async Task<IActionResult> Index()
        {
            string url = $"{PatientsApiUrl}PatientStatuses/GetAll";
            var PatientStatuses = Enumerable.Empty<PatientStatusDTO>();

            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var response = await _httpRequestHandler.Get(accessToken, url);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                PatientStatuses = response.ContentAsType<IEnumerable<PatientStatusDTO>>();
            }
            else
            {
                ModelState.AddModelError("", HttpResponseHandler.Process(response));
            }
            return View(PatientStatuses);
        }

        [HandleExceptionFilter]
        public IActionResult Create()
        {
            return View(new PatientStatusDTO
            {
                CreatedBy = _cookieService.Get("UserName")
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [HandleExceptionFilter]
        public async Task<IActionResult> Create([Bind] PatientStatusDTO patientStatus)
        {
            string url = $"{PatientsApiUrl}PatientStatuses/Add";

            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var response = await _httpRequestHandler.Post(accessToken, url, patientStatus);

            if (response.StatusCode == HttpStatusCode.Created)
            {
                AppContextHelper.SetToastMessage("Patient status has been successfully created", MessageType.Success, 1, Response);
                return RedirectToAction(nameof(Index));
            }
            else
            {
                AppContextHelper.SetToastMessage("Failed to create patient status", MessageType.Danger, 1, Response);
                ModelState.AddModelError("", HttpResponseHandler.Process(response));
            }

            return View(patientStatus);
        }

        [HandleExceptionFilter]
        public async Task<IActionResult> Edit([FromQuery] int statusId)
        {
            var patientStatus = await GetPatientStatus(statusId);
            return View(patientStatus);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [HandleExceptionFilter]
        public async Task<IActionResult> Edit([Bind]PatientStatusDTO patientStatus)
        {
            string url = $"{PatientsApiUrl}PatientStatuses/Update";

            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var response = await _httpRequestHandler.Put(accessToken, url,  patientStatus);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                AppContextHelper.SetToastMessage("Patient status has been successfully updated", MessageType.Success, 1, Response);
                return RedirectToAction(nameof(Index));
            }
            else
            {
                AppContextHelper.SetToastMessage("Failed to update patient status", MessageType.Danger, 1, Response);
                ModelState.AddModelError("", HttpResponseHandler.Process(response));
            }
            return View(patientStatus);
        }

        [HandleExceptionFilter]
        public async Task<IActionResult> Details([FromQuery] int statusId)
        {
            var PatientStatuse = await GetPatientStatus(statusId);
            return View(PatientStatuse);
        }

        [HandleExceptionFilter]
        public async Task<IActionResult> Delete([FromQuery] int statusId)
        {
            var PatientStatuse = await GetPatientStatus(statusId);
            return View(PatientStatuse);
        }

        [HttpPost, ActionName("Delete")]
        [HandleExceptionFilter]
        public async Task<IActionResult> VerifyDelete(int statusId)
        {
            string url = $"{PatientsApiUrl}PatientStatuses/Delete?statusId={statusId}";
            var PatientStatuse = new PatientStatusDTO();

            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var response = await _httpRequestHandler.Delete(accessToken, url); 

            if (response.StatusCode == HttpStatusCode.OK)
            {
                AppContextHelper.SetToastMessage("Patient status has been successfully deleted", MessageType.Success, 1, Response);
                return RedirectToAction(nameof(Index));
            }
            else
            {
                AppContextHelper.SetToastMessage("Failed to delete patient status", MessageType.Danger, 1, Response);
                TempData["ModelError"] = HttpResponseHandler.Process(response);
            }
            return RedirectToAction(nameof(Delete), new { statusId });
        }

        private async Task<PatientStatusDTO> GetPatientStatus(int statusId)
        {
            string url = $"{PatientsApiUrl}PatientStatuses/GetById?statusId={statusId}";
            var PatientStatuse = new PatientStatusDTO();

            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var response = await _httpRequestHandler.Get(accessToken, url);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                PatientStatuse = response.ContentAsType<PatientStatusDTO>();
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
            return PatientStatuse;
        }


        [HandleExceptionFilter]
        public async Task<IActionResult> GetPatientStatuses()
        {
            string url = $"{PatientsApiUrl}PatientStatuses/GetAll";
            var PatientStatuses = Enumerable.Empty<PatientStatusDTO>();

            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var response = await _httpRequestHandler.Get(accessToken, url);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                PatientStatuses = response.ContentAsType<IEnumerable<PatientStatusDTO>>();
            }
            else
            {
                ModelState.AddModelError("", HttpResponseHandler.Process(response));
            }
            return PartialView(PatientStatuses);
        }
    }
}