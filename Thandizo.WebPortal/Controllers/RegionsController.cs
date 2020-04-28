using AngleDimension.Standard.Http.HttpServices;
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
using IdentityModel;

namespace Thandizo.WebPortal.Controllers
{
    public class RegionsController : Controller
    {
        private readonly IConfiguration _configuration;

        public RegionsController(IConfiguration configuration)
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
            
            string url = $"{CoreApiUrl}Regions/GetAll";
            var Regions = Enumerable.Empty<RegionDTO>();

            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var response = await HttpRequestFactory.Get(accessToken, url);

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
               CreatedBy = AppContextHelper.GetStringValueClaim(HttpContext, JwtClaimTypes.Name)
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [HandleExceptionFilter]
        public async Task<IActionResult> Create([Bind] RegionDTO region)
        {
            string url = $"{CoreApiUrl}Regions/Add";

            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var response = await HttpRequestFactory.Post(accessToken,url, region);

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

            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var response = await HttpRequestFactory.Put(accessToken, url, region);

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
            return View(await GetRegion(regionId));
        }

        [HandleExceptionFilter]
        public async Task<IActionResult> Delete([FromQuery] int regionId)
        {
            return View(await GetRegion(regionId));
        }

        [HttpPost, ActionName("Delete")]
        [HandleExceptionFilter]
        public async Task<IActionResult> VerifyDelete(int regionId)
        {
            string url = $"{CoreApiUrl}Regions/Delete?regionId={regionId}";
            var Region = new RegionDTO();

            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var response = await HttpRequestFactory.Delete(accessToken, url); 

            if (response.StatusCode == HttpStatusCode.OK)
            {
                AppContextHelper.SetToastMessage("Region has been successfully deleted", MessageType.Success, 1, Response);
                return RedirectToAction(nameof(Index));
            }
            else
            {
                AppContextHelper.SetToastMessage("Failed to delete region", MessageType.Danger, 1, Response);
                ModelState.AddModelError("", HttpResponseHandler.Process(response));
            }
            return View(await GetRegion(regionId));
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
        public async Task<IActionResult> GetRegions()
        {
            string url = $"{CoreApiUrl}Regions/GetAll";
            var Regions = Enumerable.Empty<RegionDTO>();

            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var response = await HttpRequestFactory.Get(accessToken, url);

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