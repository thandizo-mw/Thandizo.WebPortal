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
using Thandizo.DataModels.Resources;
using Thandizo.WebPortal.Filters;
using Thandizo.WebPortal.Helpers;
using Thandizo.WebPortal.Helpers.General;
using Thandizo.WebPortal.Services;

namespace Thandizo.WebPortal.Controllers
{
    public class ResourcesController : Controller
    {
        private readonly IConfiguration _configuration;
        static int _regionId;
        static string _regionName;

        public ResourcesController(IConfiguration configuration)
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

        [HandleExceptionFilter]
        public async Task<IActionResult> Index()
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
            return View(resources);
        }

        [HandleExceptionFilter]
        public IActionResult Create()
        {
            return View(new ResourceDTO
            {
                CreatedBy = AppContextHelper.GetStringValueClaim(HttpContext, JwtClaimTypes.Name)               
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [HandleExceptionFilter]
        public async Task<IActionResult> Create([Bind] ResourceDTO resourceResponse)
        {
            ResourceDTO resource = resourceResponse;
            string url = $"{ResourcesApiUrl}Add";

            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var response = await HttpRequestFactory.Post(accessToken, url, resource);

            if (response.StatusCode == HttpStatusCode.Created)
            {
                AppContextHelper.SetToastMessage("Resource has been successfully created", MessageType.Success, 1, Response);
                return RedirectToAction(nameof(Index));
            }
            else
            {
                AppContextHelper.SetToastMessage("Failed to create resource", MessageType.Danger, 1, Response);
                ModelState.AddModelError("", HttpResponseHandler.Process(response));
            }

            return View(resourceResponse);
        }

        [HandleExceptionFilter]
        public async Task<IActionResult> Edit([FromQuery] string resourceId)
        {
            var resource = await GetResource(resourceId);
            return View(resource);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [HandleExceptionFilter]
        public async Task<IActionResult> Edit([Bind]ResourceDTO resource)
        {
            
            string url = $"{ResourcesApiUrl}Update";

            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var response = await HttpRequestFactory.Put(accessToken, url, resource);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                AppContextHelper.SetToastMessage("Resource has been successfully updated", MessageType.Success, 1, Response);
                return RedirectToAction(nameof(Index));
            }
            else
            {
                AppContextHelper.SetToastMessage("Failed to update resource", MessageType.Danger, 1, Response);
                ModelState.AddModelError("", HttpResponseHandler.Process(response));
            }
            return View(resource);
        }

        [HandleExceptionFilter]
        public async Task<IActionResult> Details([FromQuery] string resourceId)
        {
            return View(await GetResource(resourceId));
        }

        [HandleExceptionFilter]
        public async Task<IActionResult> Delete([FromQuery] string resourceId)
        {
            return View(await GetResource(resourceId));
        }

        [HttpPost, ActionName("Delete")]
        [HandleExceptionFilter]
        public async Task<IActionResult> VerifyDelete(string resourceId)
        {
            string url = $"{ResourcesApiUrl}Delete?resourceId={resourceId}";
            var Resource = new ResourceDTO();

            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var response = await HttpRequestFactory.Delete(accessToken, url);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                AppContextHelper.SetToastMessage("Resource has been successfully deleted", MessageType.Success, 1, Response);
                return RedirectToAction(nameof(Index));
            }
            else
            {
                AppContextHelper.SetToastMessage("Failed to delete resource", MessageType.Danger, 1, Response);
                ModelState.AddModelError("", HttpResponseHandler.Process(response));
            }
            return View(await GetResource(resourceId));
        }

        private async Task<ResourceDTO> GetResource(string resourceId)
        {
            string url = $"{ResourcesApiUrl}GetById?resourceId={resourceId}";
            var resource = new ResourceDTO();

            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var response = await HttpRequestFactory.Get(accessToken, url);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                resource = response.ContentAsType<ResourceDTO>();
            }
            else
            {
                ModelState.AddModelError("", HttpResponseHandler.Process(response));
            }
            return resource;
        }

    }
}