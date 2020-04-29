using AngleDimension.Standard.Http.HttpServices;
using IdentityModel;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Thandizo.DataModels.Patients;
using Thandizo.DataModels.Resources;
using Thandizo.DataModels.Resources.Responses;
using Thandizo.DataModels.ViewModels.Resources;
using Thandizo.WebPortal.Filters;
using Thandizo.WebPortal.Helpers;
using Thandizo.WebPortal.Helpers.General;
using Thandizo.WebPortal.Services;

namespace Thandizo.WebPortal.Controllers
{
    public class ResourcesAllocationController : Controller
    {
        private readonly IConfiguration _configuration;
        static int _statusId;
        static string _patientStatusName;

        public ResourcesAllocationController(IConfiguration configuration)
        {
            _configuration = configuration;


        }

        public string ResourcesApiUrl
        {
            get
            {
                return _configuration["ResourcesApiUrl"];
            }
        }
        
        public string PatientsApiUrl
        {
            get
            {
                return _configuration["PatientsApiUrl"];
            }
        }

        [HandleExceptionFilter]
        public async Task<IActionResult> Index(int statusId)
        {
            if (statusId == 0 && _statusId == 0)
            {
                return RedirectToAction("Index", "PatientStatuses");
            }

            if (statusId != 0)
            {
                _statusId = statusId;
                var patientStatus = await GetPatientStatus(statusId);
                _patientStatusName = patientStatus.PatientStatusName;
            }
            ViewBag.PatientStatusName = _patientStatusName;

            string url = $"{ResourcesApiUrl}ResourcesAllocation/GetByPatientStatusId?statusId={_statusId}";
            var resourcesAllocation = Enumerable.Empty<ResourceAllocationResponse>();

            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var response = await HttpRequestFactory.Get(accessToken, url);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                resourcesAllocation = response.ContentAsType<IEnumerable<ResourceAllocationResponse>>();
            }
            else
            {
                ModelState.AddModelError("", HttpResponseHandler.Process(response));
            }
            return View(resourcesAllocation);
        }

        [HandleExceptionFilter]
        public async Task<IActionResult> Create()
        {
            return View(new ResourceAllocationResponseViewModel
            {
                ResourceAllocationResponse = new ResourceAllocationResponse
                {
                    CreatedBy = AppContextHelper.GetStringValueClaim(HttpContext, JwtClaimTypes.Name),
                    PatientStatusId = _statusId,
                    PatientStatusName = _patientStatusName
                },
                Resources = await GetResources()
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [HandleExceptionFilter]
        public async Task<IActionResult> Create([Bind] ResourceAllocationResponseViewModel resourceAllocationResponseView)
        {
            ResourceAllocationDTO resourceAllocation = resourceAllocationResponseView.ResourceAllocationResponse;
            string url = $"{ResourcesApiUrl}ResourcesAllocation/Add";

            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var response = await HttpRequestFactory.Post(accessToken, url, resourceAllocation);

            if (response.StatusCode == HttpStatusCode.Created)
            {
                AppContextHelper.SetToastMessage("ResourcesAllocation has been successfully created", MessageType.Success, 1, Response);
                return RedirectToAction(nameof(Index));
            }
            else
            {
                AppContextHelper.SetToastMessage("Failed to create resourceAllocation", MessageType.Danger, 1, Response);
                ModelState.AddModelError("", HttpResponseHandler.Process(response));
            }
            resourceAllocationResponseView.Resources = await GetResources();
            return View(resourceAllocationResponseView);
        }

        [HandleExceptionFilter]
        public async Task<IActionResult> Edit([FromQuery] string resourceAllocationId)
        {
            return View(new ResourceAllocationResponseViewModel
            {
                ResourceAllocationResponse = await GetResourceAllocation(resourceAllocationId),
                Resources = await GetResources()
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [HandleExceptionFilter]
        public async Task<IActionResult> Edit([Bind] ResourceAllocationResponseViewModel resourceAllocationResponseView)
        {
            ResourceAllocationDTO resourceAllocation = resourceAllocationResponseView.ResourceAllocationResponse;
            string url = $"{ResourcesApiUrl}ResourcesAllocation/Update";

            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var response = await HttpRequestFactory.Put(accessToken, url, resourceAllocation);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                AppContextHelper.SetToastMessage("ResourcesAllocation has been successfully updated", MessageType.Success, 1, Response);
                return RedirectToAction(nameof(Index));
            }
            else
            {
                AppContextHelper.SetToastMessage("Failed to update resourceAllocation", MessageType.Danger, 1, Response);
                ModelState.AddModelError("", HttpResponseHandler.Process(response));
            }
            resourceAllocationResponseView.Resources = await GetResources();
            return View(resourceAllocationResponseView);
        }

        [HandleExceptionFilter]
        public async Task<IActionResult> Details([FromQuery] string resourceAllocationId)
        {
            return View(await GetResourceAllocation(resourceAllocationId));
        }

        [HandleExceptionFilter]
        public async Task<IActionResult> Delete([FromQuery] string resourceAllocationId)
        {
            return View(await GetResourceAllocation(resourceAllocationId));
        }

        [HttpPost, ActionName("Delete")]
        [HandleExceptionFilter]
        public async Task<IActionResult> VerifyDelete(string resourceAllocationId)
        {
            string url = $"{ResourcesApiUrl}ResourcesAllocation/Delete?resourceAllocationId={resourceAllocationId}";
            var ResourcesAllocation = new ResourceAllocationDTO();

            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var response = await HttpRequestFactory.Delete(accessToken, url);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                AppContextHelper.SetToastMessage("ResourcesAllocation has been successfully deleted", MessageType.Success, 1, Response);
                return RedirectToAction(nameof(Index));
            }
            else
            {
                AppContextHelper.SetToastMessage("Failed to delete resourceAllocation", MessageType.Danger, 1, Response);
                ModelState.AddModelError("", HttpResponseHandler.Process(response));
            }
            return View(await GetResourceAllocation(resourceAllocationId));
        }

        private async Task<ResourceAllocationResponse> GetResourceAllocation(string resourceAllocationId)
        {
            string url = $"{ResourcesApiUrl}ResourcesAllocation/GetById?resourceAllocationId={resourceAllocationId}";
            var resourceAllocation = new ResourceAllocationResponse();

            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var response = await HttpRequestFactory.Get(accessToken, url);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                resourceAllocation = response.ContentAsType<ResourceAllocationResponse>();
            }
            else
            {
                ModelState.AddModelError("", HttpResponseHandler.Process(response));
            }
            return resourceAllocation;
        }

        private async Task<PatientStatusDTO> GetPatientStatus(int statusId)
        {
            string url = $"{PatientsApiUrl}PatientStatuses/GetById?statusId={statusId}";
            var PatientStatus = new PatientStatusDTO();

            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var response = await HttpRequestFactory.Get(accessToken, url);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                PatientStatus = response.ContentAsType<PatientStatusDTO>();
            }
            else
            {
                ModelState.AddModelError("", HttpResponseHandler.Process(response));
            }
            return PatientStatus;
        }

        private async Task<IEnumerable<ResourceDTO>> GetResources()
        {
            string url = $"{ResourcesApiUrl}GetAll";
            var resources = Enumerable.Empty<ResourceDTO>();

            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var response = await HttpRequestFactory.Get(accessToken, url);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                resources = response.ContentAsType<IEnumerable<ResourceDTO>>();
            }
            else
            {
                ModelState.AddModelError("", HttpResponseHandler.Process(response));
            }
            return resources;
        }

        [HandleExceptionFilter]
        public async Task<IActionResult> GetResourcesAllocation()
        {
            string url = $"{ResourcesApiUrl}ResourcesAllocation/GetAll";
            var resourcesAllocation = Enumerable.Empty<ResourceAllocationDTO>();

            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var response = await HttpRequestFactory.Get(accessToken, url);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                resourcesAllocation = response.ContentAsType<IEnumerable<ResourceAllocationDTO>>();
            }
            else
            {
                ModelState.AddModelError("", HttpResponseHandler.Process(response));
            }
            return PartialView(resourcesAllocation);
        }
    }
}