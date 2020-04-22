﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Thandizo.DataModels.Core;
using Thandizo.DataModels.Core.Responses;
using Thandizo.DataModels.Patients.Responses;
using Thandizo.WebPortal.Filters;
using Thandizo.WebPortal.Helpers;
using Thandizo.WebPortal.Helpers.General;
using Thandizo.WebPortal.Services;

namespace Thandizo.WebPortal.Controllers
{
    public class DistrictsController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly ICookieService _cookieService;
        IHttpRequestHandler _httpRequestHandler;

        static int _regionId;
        static string _regionName;

        public DistrictsController(IConfiguration configuration,ICookieService cookieService, IHttpRequestHandler httpRequestHandler)
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
        public async Task<IActionResult> Index(int regionId=0, string regionName="")
        {
            if (regionId == 0 && _regionId == 0)
            {
                return RedirectToAction("Index", "Regions");
            }

            if (regionId != 0)
            {
                _regionId = regionId;
                _regionName = regionName;
            }

            string url = $"{CoreApiUrl}Districts/GetByRegionId?regionId={_regionId}";
            var districts = Enumerable.Empty<DistrictResponse>();

            var response = await _httpRequestHandler.Get(url);

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
                CreatedBy = _cookieService.Get("UserName"),
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
            var response = await _httpRequestHandler.Post(url, district);

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

            var response = await _httpRequestHandler.Put(url, district);

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
            var district = await GetDistrict(districtCode);
            return View(district);
        }

        [HandleExceptionFilter]
        public async Task<IActionResult> Delete([FromQuery] string districtCode)
        {
            var district = await GetDistrict(districtCode);
            return View(district);
        }

        [HttpPost, ActionName("Delete")]
        [HandleExceptionFilter]
        public async Task<IActionResult> VerifyDelete(string districtCode)
        {
            string url = $"{CoreApiUrl}Districts/Delete?districtCode={districtCode}";
            var District = new DistrictDTO();

            var response = await _httpRequestHandler.Delete(url);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                AppContextHelper.SetToastMessage("District has been successfully deleted", MessageType.Success, 1, Response);
                return RedirectToAction(nameof(Index));
            }
            else
            {
                AppContextHelper.SetToastMessage("Failed to delete district", MessageType.Danger, 1, Response);
                TempData["ModelError"] = HttpResponseHandler.Process(response);
            }
            return RedirectToAction(nameof(Delete), new { districtCode });
        }

        private async Task<DistrictResponse> GetDistrict(string districtCode)
        {
            string url = $"{CoreApiUrl}Districts/GetById?districtCode={districtCode}";
            var district = new DistrictResponse();

            var response = await _httpRequestHandler.Get(url);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                district = response.ContentAsType<DistrictResponse>();
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
            return district;
        }


        [HandleExceptionFilter]
        public async Task<IActionResult> GetDistricts()
        {
            string url = $"{CoreApiUrl}Districts/GetAll";
            var districts = Enumerable.Empty<DistrictDTO>();

            var response = await _httpRequestHandler.Get(url);

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