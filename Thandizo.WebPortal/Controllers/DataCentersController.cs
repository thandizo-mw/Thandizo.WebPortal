using AngleDimension.Standard.Http.HttpServices;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Thandizo.DataModels.Core;
using Thandizo.DataModels.DataCenters;
using Thandizo.DataModels.DataCenters.Responses;
using Thandizo.DataModels.ViewModels.DataCenters;
using Thandizo.WebPortal.Filters;
using Thandizo.WebPortal.Helpers;
using Thandizo.WebPortal.Helpers.General;
using Thandizo.WebPortal.Services;

namespace Thandizo.WebPortal.Controllers
{
    public class DataCentersController : Controller
    {
        private readonly IConfiguration _configuration;

        public DataCentersController(IConfiguration configuration)
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
        public async Task<IActionResult> Index()
        {
            string url = $"{CoreApiUrl}DataCenters/GetAll";
            var DataCenters = Enumerable.Empty<DataCenterResponse>();

            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var response = await HttpRequestFactory.Get(accessToken, url);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                DataCenters = response.ContentAsType<IEnumerable<DataCenterResponse>>();
            }
            else
            {
                ModelState.AddModelError("", HttpResponseHandler.Process(response));
            }
            return View(DataCenters);
        }

        [HandleExceptionFilter]
        public async Task<IActionResult> Create()
        {
            return View(new DataCenterResponseViewModel
            {
                DataCenterResponse = new DataCenterResponse
                {
                    CreatedBy = HttpContext.User.Identity.Name,
                    IsHealthFacility = false
                },
                Districts = await GetDistricts(),
                FacilityTypes = await GetFacilityTypes()
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [HandleExceptionFilter]
        public async Task<IActionResult> Create([Bind] DataCenterResponseViewModel dataCenterResponseViewModel)
        {
            DataCenterDTO dataCenter = dataCenterResponseViewModel.DataCenterResponse;
            string url = $"{CoreApiUrl}DataCenters/Add";

            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var response = await HttpRequestFactory.Post(accessToken, url, dataCenter);

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
            dataCenterResponseViewModel.Districts = await GetDistricts();
            dataCenterResponseViewModel.FacilityTypes = await GetFacilityTypes();
            return View(dataCenterResponseViewModel);
        }

        [HandleExceptionFilter]
        public async Task<IActionResult> Edit([FromQuery] int centerId)
        {
            var dataCenter = await GetDataCenter(centerId);
            return View(new DataCenterResponseViewModel
            {
                DataCenterResponse = dataCenter,
                Districts = await GetDistricts(),
                FacilityTypes = await GetFacilityTypes()
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [HandleExceptionFilter]
        public async Task<IActionResult> Edit([Bind]DataCenterResponseViewModel dataCenterResponseViewModel)
        {
            DataCenterDTO dataCenter = dataCenterResponseViewModel.DataCenterResponse;
            string url = $"{CoreApiUrl}DataCenters/Update";

            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var response = await HttpRequestFactory.Put(accessToken, url,  dataCenter);

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
            dataCenterResponseViewModel.Districts = await GetDistricts();
            dataCenterResponseViewModel.FacilityTypes = await GetFacilityTypes();
            return View(dataCenter);
        }

        [HandleExceptionFilter]
        public async Task<IActionResult> Details([FromQuery] int centerId)
        {
            var DataCenter = await GetDataCenter(centerId);
            return View(DataCenter);
        }

        [HandleExceptionFilter]
        public async Task<IActionResult> Delete([FromQuery] int centerId)
        {
            var DataCenter = await GetDataCenter(centerId);
            return View(DataCenter);
        }

        [HttpPost, ActionName("Delete")]
        [HandleExceptionFilter]
        public async Task<IActionResult> VerifyDelete(int centerId)
        {
            string url = $"{CoreApiUrl}DataCenters/Delete?centerId={centerId}";
            var DataCenter = new DataCenterDTO();

            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var response = await HttpRequestFactory.Delete(accessToken, url); 

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
            return RedirectToAction(nameof(Delete), new { centerId });
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
            if (TempData["ModelError"] != null)
            {
                ModelState.AddModelError("", TempData["ModelError"].ToString());
                TempData["ModelError"] = null;
            }
            return dataCenter;
        }


        [HandleExceptionFilter]
        public async Task<IActionResult> GetDataCenters()
        {
            string url = $"{CoreApiUrl}DataCenters/GetAll";
            var DataCenters = Enumerable.Empty<DataCenterDTO>();

            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var response = await HttpRequestFactory.Get(accessToken, url);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                DataCenters = response.ContentAsType<IEnumerable<DataCenterDTO>>();
            }
            else
            {
                ModelState.AddModelError("", HttpResponseHandler.Process(response));
            }
            return PartialView(DataCenters);
        }

        public async Task<IEnumerable<DistrictDTO>> GetDistricts()
        {
            string url = $"{CoreApiUrl}Districts/GetAll";
            var districts = Enumerable.Empty<DistrictDTO>();

            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var response = await HttpRequestFactory.Get(accessToken, url);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                districts = response.ContentAsType<IEnumerable<DistrictDTO>>();
            }
            else
            {
                ModelState.AddModelError("", HttpResponseHandler.Process(response));
            }
            return districts;
        }

        public async Task<IEnumerable<FacilityTypeDTO>> GetFacilityTypes()
        {
            string url = $"{CoreApiUrl}FacilityTypes/GetAll";
            var facilityTypes = Enumerable.Empty<FacilityTypeDTO>();

            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var response = await HttpRequestFactory.Get(accessToken, url);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                facilityTypes = response.ContentAsType<IEnumerable<FacilityTypeDTO>>();
            }
            else
            {
                ModelState.AddModelError("", HttpResponseHandler.Process(response));
            }
            return facilityTypes;
        }

    }

}