using AngleDimension.Standard.Http.HttpServices;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Thandizo.WebPortal.Filters;
using Thandizo.WebPortal.Helpers;
using Thandizo.WebPortal.Helpers.General;
using Thandizo.WebPortal.Services;
using IdentityModel;
using Thandizo.DataModels.DataCenters;
using Thandizo.DataModels.ViewModels.DataCenters;
using Thandizo.DataModels.DataCenters.Responses;
using Thandizo.DataModels.Core;

namespace Thandizo.WebPortal.Controllers
{
    public class HealthCareWorkersController : Controller
    {
        private readonly IConfiguration _configuration;
        private static int _centerId;
        private static string _centerName;
        
        public HealthCareWorkersController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string CoreApiUrl
        {
            get
            {
                return _configuration["CoreApiUrl"];
            }
        }

        [HandleExceptionFilter]
        public async Task<IActionResult> Index(int centerId)
        {
            if(centerId == 0 && _centerId == 0)
            {
                return RedirectToAction("Index", "DataCenters");
            }
            if(centerId != 0)
            {
                _centerId = centerId;
                var dataCenter = await GetDataCenter(centerId);
                _centerName = dataCenter.CenterName;
            }
            ViewBag.CenterName = _centerName;
            string url = $"{CoreApiUrl}HealthCareWorkers/GetByDataCenter?centerId={_centerId}";
            var healthCareWorkers = Enumerable.Empty<HealthCareWorkerDTO>();

            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var response = await HttpRequestFactory.Get(accessToken, url);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                healthCareWorkers = response.ContentAsType<IEnumerable<HealthCareWorkerDTO>>();
            }
            else
            {
                ModelState.AddModelError("", HttpResponseHandler.Process(response));
            }
            return View(healthCareWorkers);
        }

        [HandleExceptionFilter]
        public async Task<IActionResult> Create()
        {
            if(_centerId == 0)
            {
                return RedirectToAction("Index", "DataCenters");
            }
           
            return View(new HealthCareWorkerResponseViewModel
            {
                HealthCareWorkerResponse = new HealthCareWorkerResponse
                {
                    //CreatedBy = AppContextHelper.GetStringValueClaim(HttpContext, JwtClaimTypes.Name),
                    CreatedBy = "SYS",
                    DataCenterId = _centerId,
                    CenterName = _centerName
                },
                IdentificationTypes = await GetIdentificationTypes()
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [HandleExceptionFilter]
        public async Task<IActionResult> Create([Bind] HealthCareWorkerResponseViewModel healthCareWorkerViewModel)
        {
            HealthCareWorkerDTO healthCareWorker = healthCareWorkerViewModel.HealthCareWorkerResponse;
            string url = $"{CoreApiUrl}HealthCareWorkers/Add";

            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var response = await HttpRequestFactory.Post(accessToken, url, healthCareWorker);

            if (response.StatusCode == HttpStatusCode.Created)
            {
                AppContextHelper.SetToastMessage("Health care worker has been successfully created", MessageType.Success, 1, Response);
                return RedirectToAction(nameof(Index));
            }
            else
            {
                AppContextHelper.SetToastMessage("Failed to create health care worker", MessageType.Danger, 1, Response);
                ModelState.AddModelError("", HttpResponseHandler.Process(response));
            }
            healthCareWorkerViewModel.IdentificationTypes = await GetIdentificationTypes();
            return View(healthCareWorkerViewModel);
        }

        [HandleExceptionFilter]
        public async Task<IActionResult> Edit([FromQuery] int workerId)
        {
            return View(new HealthCareWorkerResponseViewModel
            {
                HealthCareWorkerResponse = await GetHealthCareWorker(workerId),
                IdentificationTypes = await GetIdentificationTypes()
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [HandleExceptionFilter]
        public async Task<IActionResult> Edit([Bind] HealthCareWorkerResponseViewModel healthCareWorkerViewModel)
        {
            HealthCareWorkerDTO healthCareWorker = healthCareWorkerViewModel.HealthCareWorkerResponse;
            string url = $"{CoreApiUrl}HealthCareWorkers/Update";

            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var response = await HttpRequestFactory.Put(accessToken, url,  healthCareWorker);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                AppContextHelper.SetToastMessage("Health care worker has been successfully updated", MessageType.Success, 1, Response);
                return RedirectToAction(nameof(Index));
            }
            else
            {
                AppContextHelper.SetToastMessage("Failed to update health care worker", MessageType.Danger, 1, Response);
                ModelState.AddModelError("", HttpResponseHandler.Process(response));
            }
            healthCareWorkerViewModel.IdentificationTypes = await GetIdentificationTypes();
            return View(healthCareWorkerViewModel);
        }

        [HandleExceptionFilter]
        public async Task<IActionResult> Details([FromQuery] int workerId)
        {
            var HealthCareWorker = await GetHealthCareWorker(workerId);
            return View(HealthCareWorker);
        }

        [HandleExceptionFilter]
        public async Task<IActionResult> Delete([FromQuery] int workerId)
        {
            var HealthCareWorker = await GetHealthCareWorker(workerId);
            return View(HealthCareWorker);
        }

        [HttpPost, ActionName("Delete")]
        [HandleExceptionFilter]
        public async Task<IActionResult> VerifyDelete(int workerId)
        {
            string url = $"{CoreApiUrl}HealthCareWorkers/Delete?workerId={workerId}";
            
            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var response = await HttpRequestFactory.Delete(accessToken, url); 

            if (response.StatusCode == HttpStatusCode.OK)
            {
                AppContextHelper.SetToastMessage("Health care worker has been successfully deleted", MessageType.Success, 1, Response);
                return RedirectToAction(nameof(Index));
            }
            else
            {
                AppContextHelper.SetToastMessage("Failed to delete health care worker", MessageType.Danger, 1, Response);
                ModelState.AddModelError("", HttpResponseHandler.Process(response));
            }
            var HealthCareWorker = await GetHealthCareWorker(workerId);
            return View(HealthCareWorker);
        }

        private async Task<HealthCareWorkerResponse> GetHealthCareWorker(int workerId)
        {
            string url = $"{CoreApiUrl}HealthCareWorkers/GetById?workerId={workerId}";
            var HealthCareWorker = new HealthCareWorkerResponse();

            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var response = await HttpRequestFactory.Get(accessToken, url);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                HealthCareWorker = response.ContentAsType<HealthCareWorkerResponse>();
            }
            else
            {
                ModelState.AddModelError("", HttpResponseHandler.Process(response));
            }
            return HealthCareWorker;
        }

        private async Task<DataCenterResponse> GetDataCenter(int centerId)
        {
            string url = $"{CoreApiUrl}DataCenters/GetById?centerId={centerId}";
            var dataCenter = new DataCenterResponse();

            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var response = await HttpRequestFactory.Get(accessToken, url);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                dataCenter = response.ContentAsType<DataCenterResponse>();
            }
            else
            {
                ModelState.AddModelError("", HttpResponseHandler.Process(response));
            }
            return dataCenter;
        }

        public async Task<IEnumerable<IdentificationTypeDTO>> GetIdentificationTypes()
        {
            string url = $"{CoreApiUrl}IdentificationTypes/GetAll";
            var identificationTypes = Enumerable.Empty<IdentificationTypeDTO>();

            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var response = await HttpRequestFactory.Get(accessToken, url);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                identificationTypes = response.ContentAsType<IEnumerable<IdentificationTypeDTO>>();
            }
            else
            {
                ModelState.AddModelError("", HttpResponseHandler.Process(response));
            }
            return (identificationTypes);
        }
    }
}