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
    public class RegionsController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly ICookieService _cookieService;
        IHttpRequestHandler _httpRequestHandler;

        public RegionsController(IConfiguration configuration,ICookieService cookieService, IHttpRequestHandler httpRequestHandler)
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
            string url = $"{CoreApiUrl}Regions/GetAll";
            var Regions = Enumerable.Empty<RegionDTO>();

            var response = await _httpRequestHandler.Get(url);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                Regions = response.ContentAsType<IEnumerable<RegionDTO>>();
            }
            else
            {
                ModelState.AddModelError("", HttpResponseHandler.Process(response));
            }
            return View(Regions);
        }

        [HandleExceptionFilter]
        public IActionResult Create()
        {
            return View(new RegionDTO
            {
                CreatedBy = _cookieService.Get("UserName")
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [HandleExceptionFilter]
        public async Task<IActionResult> Create([Bind] RegionDTO region)
        {
            string url = $"{CoreApiUrl}Regions/Add";
            var response = await _httpRequestHandler.Post(url, region);

            if (response.StatusCode == HttpStatusCode.Created)
            {
                AppContextHelper.SetToastMessage("Region has been successfully created", MessageType.Success, 1, Response);
                return RedirectToAction(nameof(Index));
            }
            else
            {
                AppContextHelper.SetToastMessage("Failed to create region", MessageType.Danger, 1, Response);
                ModelState.AddModelError("", HttpResponseHandler.Process(response));
            }

            return View(region);
        }

        [HandleExceptionFilter]
        public async Task<IActionResult> Edit([FromQuery] int regionId)
        {
            var Region = await GetRegion(regionId);
            return View(Region);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [HandleExceptionFilter]
        public async Task<IActionResult> Edit([Bind]RegionDTO region)
        {
            string url = $"{CoreApiUrl}Regions/Update";

            var response = await _httpRequestHandler.Put(url, region);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                AppContextHelper.SetToastMessage("Region has been successfully updated", MessageType.Success, 1, Response);
                return RedirectToAction(nameof(Index));
            }
            else
            {
                AppContextHelper.SetToastMessage("Failed to update region", MessageType.Danger, 1, Response);
                ModelState.AddModelError("", HttpResponseHandler.Process(response));
            }
            return View(region);
        }

        [HandleExceptionFilter]
        public async Task<IActionResult> Details([FromQuery] int regionId)
        {
            var Region = await GetRegion(regionId);
            return View(Region);
        }

        [HandleExceptionFilter]
        public async Task<IActionResult> Delete([FromQuery] int regionId)
        {
            var Region = await GetRegion(regionId);
            return View(Region);
        }

        [HttpPost, ActionName("Delete")]
        [HandleExceptionFilter]
        public async Task<IActionResult> VerifyDelete(int regionId)
        {
            string url = $"{CoreApiUrl}Regions/Delete?regionId={regionId}";
            var Region = new RegionDTO();

            var response = await _httpRequestHandler.Delete(url);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                AppContextHelper.SetToastMessage("Region has been successfully deleted", MessageType.Success, 1, Response);
                return RedirectToAction(nameof(Index));
            }
            else
            {
                AppContextHelper.SetToastMessage("Failed to delete region", MessageType.Danger, 1, Response);
                TempData["ModelError"] = HttpResponseHandler.Process(response);
            }
            return RedirectToAction(nameof(Delete), new { regionId });
        }

        private async Task<RegionDTO> GetRegion(int regionId)
        {
            string url = $"{CoreApiUrl}Regions/GetById?regionId={regionId}";
            var Region = new RegionDTO();

            var response = await _httpRequestHandler.Get(url);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                Region = response.ContentAsType<RegionDTO>();
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
            return Region;
        }


        [HandleExceptionFilter]
        public async Task<IActionResult> GetRegions()
        {
            string url = $"{CoreApiUrl}Regions/GetAll";
            var Regions = Enumerable.Empty<RegionDTO>();

            var response = await _httpRequestHandler.Get(url);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                Regions = response.ContentAsType<IEnumerable<RegionDTO>>();
            }
            else
            {
                ModelState.AddModelError("", HttpResponseHandler.Process(response));
            }
            return PartialView(Regions);
        }
    }
}