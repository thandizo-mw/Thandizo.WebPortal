using AngleDimension.Standard.Http.HttpServices;
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
using IdentityModel;

namespace Thandizo.WebPortal.Controllers
{
    public class TransmissionClassificationsController : Controller
    {
        private readonly IConfiguration _configuration;

        public TransmissionClassificationsController(IConfiguration configuration)
        {
            _configuration = configuration;
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
            string url = $"{PatientsApiUrl}TransmissionClassifications/GetAll";
            var TransmissionClassifications = Enumerable.Empty<TransmissionClassificationDTO>();

            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var response = await HttpRequestFactory.Get(accessToken, url);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                TransmissionClassifications = response.ContentAsType<IEnumerable<TransmissionClassificationDTO>>();
            }
            else
            {
                ModelState.AddModelError("", HttpResponseHandler.Process(response));
            }
            return View(TransmissionClassifications);
        }

        [HandleExceptionFilter]
        public IActionResult Create()
        {
            return View(new TransmissionClassificationDTO
            {
               CreatedBy = AppContextHelper.GetStringValueClaim(HttpContext, JwtClaimTypes.Name)
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [HandleExceptionFilter]
        public async Task<IActionResult> Create([Bind] TransmissionClassificationDTO classification)
        {
            string url = $"{PatientsApiUrl}TransmissionClassifications/Add";

            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var response = await HttpRequestFactory.Post(accessToken, url, classification);

            if (response.StatusCode == HttpStatusCode.Created)
            {
                AppContextHelper.SetToastMessage("Classification has been successfully created", MessageType.Success, 1, Response);
                return RedirectToAction(nameof(Index));
            }
            else
            {
                AppContextHelper.SetToastMessage("Failed to create classification", MessageType.Danger, 1, Response);
                ModelState.AddModelError("", HttpResponseHandler.Process(response));
            }

            return View(classification);
        }

        [HandleExceptionFilter]
        public async Task<IActionResult> Edit([FromQuery] int classificationId)
        {
            var TransmissionClassification = await GetTransmissionClassification(classificationId);
            return View(TransmissionClassification);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [HandleExceptionFilter]
        public async Task<IActionResult> Edit([Bind]TransmissionClassificationDTO classification)
        {
            string url = $"{PatientsApiUrl}TransmissionClassifications/Update";

            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var response = await HttpRequestFactory.Put(accessToken, url,  classification);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                AppContextHelper.SetToastMessage("Classification has been successfully updated", MessageType.Success, 1, Response);
                return RedirectToAction(nameof(Index));
            }
            else
            {
                AppContextHelper.SetToastMessage("Failed to update classification", MessageType.Danger, 1, Response);
                ModelState.AddModelError("", HttpResponseHandler.Process(response));
            }
            return View(classification);
        }

        [HandleExceptionFilter]
        public async Task<IActionResult> Details([FromQuery] int classificationId)
        {
            var TransmissionClassification = await GetTransmissionClassification(classificationId);
            return View(TransmissionClassification);
        }

        [HandleExceptionFilter]
        public async Task<IActionResult> Delete([FromQuery] int classificationId)
        {
            var TransmissionClassification = await GetTransmissionClassification(classificationId);
            return View(TransmissionClassification);
        }

        [HttpPost, ActionName("Delete")]
        [HandleExceptionFilter]
        public async Task<IActionResult> VerifyDelete(int classificationId)
        {
            string url = $"{PatientsApiUrl}TransmissionClassifications/Delete?classificationId={classificationId}";
            var TransmissionClassification = new TransmissionClassificationDTO();

            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var response = await HttpRequestFactory.Delete(accessToken, url); 

            if (response.StatusCode == HttpStatusCode.OK)
            {
                AppContextHelper.SetToastMessage("Classification has been successfully deleted", MessageType.Success, 1, Response);
                return RedirectToAction(nameof(Index));
            }
            else
            {
                AppContextHelper.SetToastMessage("Failed to delete classification", MessageType.Danger, 1, Response);
                TempData["ModelError"] = HttpResponseHandler.Process(response);
            }
            return RedirectToAction(nameof(Delete), new { classificationId });
        }

        private async Task<TransmissionClassificationDTO> GetTransmissionClassification(int classificationId)
        {
            string url = $"{PatientsApiUrl}TransmissionClassifications/GetById?classificationId={classificationId}";
            var TransmissionClassification = new TransmissionClassificationDTO();

            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var response = await HttpRequestFactory.Get(accessToken, url);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                TransmissionClassification = response.ContentAsType<TransmissionClassificationDTO>();
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
            return TransmissionClassification;
        }


        [HandleExceptionFilter]
        public async Task<IActionResult> GetTransmissionClassifications()
        {
            string url = $"{PatientsApiUrl}TransmissionClassifications/GetAll";
            var TransmissionClassifications = Enumerable.Empty<TransmissionClassificationDTO>();

            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var response = await HttpRequestFactory.Get(accessToken, url);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                TransmissionClassifications = response.ContentAsType<IEnumerable<TransmissionClassificationDTO>>();
            }
            else
            {
                ModelState.AddModelError("", HttpResponseHandler.Process(response));
            }
            return PartialView(TransmissionClassifications);
        }
    }
}