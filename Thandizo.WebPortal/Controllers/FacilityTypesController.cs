using AngleDimension.Standard.Http.HttpServices;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Thandizo.DataModels.DataCenters;
using Thandizo.WebPortal.Filters;
using Thandizo.WebPortal.Helpers;
using Thandizo.WebPortal.Helpers.General;
using Thandizo.WebPortal.Services;
using IdentityModel;

namespace Thandizo.WebPortal.Controllers
{
    public class FacilityTypesController : Controller
    {
        private readonly IConfiguration _configuration;
        
        public FacilityTypesController(IConfiguration configuration)
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
            string url = $"{CoreApiUrl}FacilityTypes/GetAll";
            var FacilityTypes = Enumerable.Empty<FacilityTypeDTO>();

            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var response = await HttpRequestFactory.Get(accessToken, url);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                FacilityTypes = response.ContentAsType<IEnumerable<FacilityTypeDTO>>();
            }
            else
            {
                ModelState.AddModelError("", HttpResponseHandler.Process(response));
            }
            return View(FacilityTypes);
        }

        [HandleExceptionFilter]
        public IActionResult Create()
        {
            return View(new FacilityTypeDTO
            {
                CreatedBy = AppContextHelper.GetStringValueClaim(HttpContext, JwtClaimTypes.Name)
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [HandleExceptionFilter]
        public async Task<IActionResult> Create([Bind] FacilityTypeDTO facilityType)
        {
            string url = $"{CoreApiUrl}FacilityTypes/Add";

            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var response = await HttpRequestFactory.Post(accessToken, url, facilityType);

            if (response.StatusCode == HttpStatusCode.Created)
            {
                AppContextHelper.SetToastMessage("Facility type has been successfully created", MessageType.Success, 1, Response);
                return RedirectToAction(nameof(Index));
            }
            else
            {
                AppContextHelper.SetToastMessage("Failed to create facility type", MessageType.Danger, 1, Response);
                ModelState.AddModelError("", HttpResponseHandler.Process(response));
            }

            return View(facilityType);
        }

        [HandleExceptionFilter]
        public async Task<IActionResult> Edit([FromQuery] int facilityTypeId)
        {
            return View(await GetFacilityType(facilityTypeId));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [HandleExceptionFilter]
        public async Task<IActionResult> Edit([Bind]FacilityTypeDTO facilityType)
        {
            string url = $"{CoreApiUrl}FacilityTypes/Update";

            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var response = await HttpRequestFactory.Put(accessToken, url,  facilityType);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                AppContextHelper.SetToastMessage("Facility type has been successfully updated", MessageType.Success, 1, Response);
                return RedirectToAction(nameof(Index));
            }
            else
            {
                AppContextHelper.SetToastMessage("Failed to update facility type", MessageType.Danger, 1, Response);
                ModelState.AddModelError("", HttpResponseHandler.Process(response));
            }
            return View(facilityType);
        }

        [HandleExceptionFilter]
        public async Task<IActionResult> Details([FromQuery] int facilityTypeId)
        {;
            return View(await GetFacilityType(facilityTypeId));
        }

        [HandleExceptionFilter]
        public async Task<IActionResult> Delete([FromQuery] int facilityTypeId)
        {
            var FacilityType = await GetFacilityType(facilityTypeId);
            return View(FacilityType);
        }

        [HttpPost, ActionName("Delete")]
        [HandleExceptionFilter]
        public async Task<IActionResult> VerifyDelete(int facilityTypeId)
        {
            string url = $"{CoreApiUrl}FacilityTypes/Delete?facilityTypeId={facilityTypeId}";
            var FacilityType = new FacilityTypeDTO();

            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var response = await HttpRequestFactory.Delete(accessToken, url); 

            if (response.StatusCode == HttpStatusCode.OK)
            {
                AppContextHelper.SetToastMessage("Facility type has been successfully deleted", MessageType.Success, 1, Response);
                return RedirectToAction(nameof(Index));
            }
            else
            {
                AppContextHelper.SetToastMessage("Failed to delete identification Type", MessageType.Danger, 1, Response);
                ModelState.AddModelError("", HttpResponseHandler.Process(response));
            }
            return View(await GetFacilityType(facilityTypeId));
        }

        private async Task<FacilityTypeDTO> GetFacilityType(int facilityTypeId)
        {
            string url = $"{CoreApiUrl}FacilityTypes/GetById?facilityTypeId={facilityTypeId}";
            var FacilityType = new FacilityTypeDTO();

            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var response = await HttpRequestFactory.Get(accessToken, url);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                FacilityType = response.ContentAsType<FacilityTypeDTO>();
            }
            else
            {
                ModelState.AddModelError("", HttpResponseHandler.Process(response));
            }
            return FacilityType;
        }


        [HandleExceptionFilter]
        public async Task<IActionResult> GetFacilityTypes()
        {
            string url = $"{CoreApiUrl}FacilityTypes/GetAll";
            var FacilityTypes = Enumerable.Empty<FacilityTypeDTO>();

            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var response = await HttpRequestFactory.Get(accessToken, url);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                FacilityTypes = response.ContentAsType<IEnumerable<FacilityTypeDTO>>();
            }
            else
            {
                ModelState.AddModelError("", HttpResponseHandler.Process(response));
            }
            return PartialView(FacilityTypes);
        }
    }
}