﻿using AngleDimension.Standard.Http.HttpServices;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Thandizo.DataModels.Core;
using Thandizo.DataModels.Core.Responses;
using Thandizo.WebPortal.Filters;
using Thandizo.WebPortal.Helpers;
using Thandizo.WebPortal.Helpers.General;
using Thandizo.WebPortal.Services;
using IdentityModel;

namespace Thandizo.WebPortal.Controllers
{
    public class DistrictsController : Controller
    {
        private readonly IConfiguration _configuration;
        static int _regionId;
        static string _regionName;

        public DistrictsController(IConfiguration configuration)
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
        public async Task<IActionResult> Index(int regionId)
        {
            if (regionId == 0)
            {
                if(_regionId == 0)
                {
                    return RedirectToAction("Index", "Regions");
                }
            }
            else
            {
                _regionId = regionId;
                var region = await GetRegion(regionId);
                _regionName = region.RegionName;
            }

           

            ViewBag.RegionName = _regionName;

            string url = $"{CoreApiUrl}Districts/GetByRegionId?regionId={_regionId}";
            var districts = Enumerable.Empty<DistrictResponse>();

            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var response = await HttpRequestFactory.Get(accessToken, url);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                districts = response.ContentAsType<IEnumerable<DistrictResponse>>();
            }
            else
            {
                ModelState.AddModelError("", HttpResponseHandler.Process(response));
            }
            return View(districts);
        }

        [HandleExceptionFilter]
        public IActionResult Create()
        {
            return View(new DistrictResponse
            {
                CreatedBy = AppContextHelper.GetStringValueClaim(HttpContext, JwtClaimTypes.Name),
                RegionId = _regionId,
                RegionName = _regionName
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [HandleExceptionFilter]
        public async Task<IActionResult> Create([Bind] DistrictResponse districtResponse)
        {
            DistrictDTO district = districtResponse;
            string url = $"{CoreApiUrl}Districts/Add";

            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var response = await HttpRequestFactory.Post(accessToken, url, district);

            if (response.StatusCode == HttpStatusCode.Created)
            {
                AppContextHelper.SetToastMessage("District has been successfully created", MessageType.Success, 1, Response);
                return RedirectToAction(nameof(Index));
            }
            else
            {
                AppContextHelper.SetToastMessage("Failed to create district", MessageType.Danger, 1, Response);
                ModelState.AddModelError("", HttpResponseHandler.Process(response));
            }

            return View(districtResponse);
        }

        [HandleExceptionFilter]
        public async Task<IActionResult> Edit([FromQuery] string districtCode)
        {
            var district = await GetDistrict(districtCode);
            return View(district);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [HandleExceptionFilter]
        public async Task<IActionResult> Edit([Bind]DistrictResponse districtResponse)
        {
            DistrictDTO district = districtResponse;
            string url = $"{CoreApiUrl}Districts/Update";

            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var response = await HttpRequestFactory.Put(accessToken, url, district);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                AppContextHelper.SetToastMessage("District has been successfully updated", MessageType.Success, 1, Response);
                return RedirectToAction(nameof(Index));
            }
            else
            {
                AppContextHelper.SetToastMessage("Failed to update district", MessageType.Danger, 1, Response);
                ModelState.AddModelError("", HttpResponseHandler.Process(response));
            }
            return View(districtResponse);
        }

        [HandleExceptionFilter]
        public async Task<IActionResult> Details([FromQuery] string districtCode)
        {
            return View(await GetDistrict(districtCode));
        }

        [HandleExceptionFilter]
        public async Task<IActionResult> Delete([FromQuery] string districtCode)
        {
            return View(await GetDistrict(districtCode));
        }

        [HttpPost, ActionName("Delete")]
        [HandleExceptionFilter]
        public async Task<IActionResult> VerifyDelete(string districtCode)
        {
            string url = $"{CoreApiUrl}Districts/Delete?districtCode={districtCode}";
            var District = new DistrictDTO();

            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var response = await HttpRequestFactory.Delete(accessToken, url);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                AppContextHelper.SetToastMessage("District has been successfully deleted", MessageType.Success, 1, Response);
                return RedirectToAction(nameof(Index));
            }
            else
            {
                AppContextHelper.SetToastMessage("Failed to delete district", MessageType.Danger, 1, Response);
                ModelState.AddModelError("", HttpResponseHandler.Process(response));
            }
            return View(await GetDistrict(districtCode));
        }

        private async Task<DistrictResponse> GetDistrict(string districtCode)
        {
            string url = $"{CoreApiUrl}Districts/GetById?districtCode={districtCode}";
            var district = new DistrictResponse();

            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var response = await HttpRequestFactory.Get(accessToken, url);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                district = response.ContentAsType<DistrictResponse>();
            }
            else
            {
                ModelState.AddModelError("", HttpResponseHandler.Process(response));
            }
            return district;
        }

        private async Task<RegionDTO> GetRegion(int regionId)
        {
            string url = $"{CoreApiUrl}Regions/GetById?regionId={regionId}";
            var Region = new RegionDTO();

            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var response = await HttpRequestFactory.Get(accessToken, url);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                Region = response.ContentAsType<RegionDTO>();
            }
            else
            {
                ModelState.AddModelError("", HttpResponseHandler.Process(response));
            }
            return Region;
        }

        [HandleExceptionFilter]
        public async Task<IActionResult> GetDistricts()
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
            return PartialView(districts);
        }
    }
}