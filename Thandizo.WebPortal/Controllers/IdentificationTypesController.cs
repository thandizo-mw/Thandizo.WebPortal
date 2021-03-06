﻿using AngleDimension.Standard.Http.HttpServices;
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
    public class IdentificationTypesController : Controller
    {
        private readonly IConfiguration _configuration;
        
        public IdentificationTypesController(IConfiguration configuration)
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
            string url = $"{CoreApiUrl}IdentificationTypes/GetAll";
            var IdentificationTypes = Enumerable.Empty<IdentificationTypeDTO>();

            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var response = await HttpRequestFactory.Get(accessToken, url);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                IdentificationTypes = response.ContentAsType<IEnumerable<IdentificationTypeDTO>>();
            }
            else
            {
                ModelState.AddModelError("", HttpResponseHandler.Process(response));
            }
            return View(IdentificationTypes);
        }

        [HandleExceptionFilter]
        public IActionResult Create()
        {
            return View(new IdentificationTypeDTO
            {
               CreatedBy = AppContextHelper.GetStringValueClaim(HttpContext, JwtClaimTypes.Name)
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [HandleExceptionFilter]
        public async Task<IActionResult> Create([Bind] IdentificationTypeDTO identificationType)
        {
            string url = $"{CoreApiUrl}IdentificationTypes/Add";

            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var response = await HttpRequestFactory.Post(accessToken, url, identificationType);

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

            return View(identificationType);
        }

        [HandleExceptionFilter]
        public async Task<IActionResult> Edit([FromQuery] int identificationTypeId)
        {
            return View(await GetIdentificationType(identificationTypeId));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [HandleExceptionFilter]
        public async Task<IActionResult> Edit([Bind]IdentificationTypeDTO identificationType)
        {
            string url = $"{CoreApiUrl}IdentificationTypes/Update";

            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var response = await HttpRequestFactory.Put(accessToken, url,  identificationType);

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
            return View(identificationType);
        }

        [HandleExceptionFilter]
        public async Task<IActionResult> Details([FromQuery] int identificationTypeId)
        {
            var IdentificationType = await GetIdentificationType(identificationTypeId);
            return View(IdentificationType);
        }

        [HandleExceptionFilter]
        public async Task<IActionResult> Delete([FromQuery] int identificationTypeId)
        {
            var IdentificationType = await GetIdentificationType(identificationTypeId);
            return View(IdentificationType);
        }

        [HttpPost, ActionName("Delete")]
        [HandleExceptionFilter]
        public async Task<IActionResult> VerifyDelete(int identificationTypeId)
        {
            string url = $"{CoreApiUrl}IdentificationTypes/Delete?identificationTypeId={identificationTypeId}";
            var IdentificationType = new IdentificationTypeDTO();

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
                ModelState.AddModelError("", HttpResponseHandler.Process(response));
            }
            return View(await GetIdentificationType(identificationTypeId));
        }

        private async Task<IdentificationTypeDTO> GetIdentificationType(int identificationTypeId)
        {
            string url = $"{CoreApiUrl}IdentificationTypes/GetById?identificationTypeId={identificationTypeId}";
            var IdentificationType = new IdentificationTypeDTO();

            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var response = await HttpRequestFactory.Get(accessToken, url);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                IdentificationType = response.ContentAsType<IdentificationTypeDTO>();
            }
            else
            {
                ModelState.AddModelError("", HttpResponseHandler.Process(response));
            }
            return IdentificationType;
        }


        [HandleExceptionFilter]
        public async Task<IActionResult> GetIdentificationTypes()
        {
            string url = $"{CoreApiUrl}IdentificationTypes/GetAll";
            var IdentificationTypes = Enumerable.Empty<IdentificationTypeDTO>();

            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var response = await HttpRequestFactory.Get(accessToken, url);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                IdentificationTypes = response.ContentAsType<IEnumerable<IdentificationTypeDTO>>();
            }
            else
            {
                ModelState.AddModelError("", HttpResponseHandler.Process(response));
            }
            return PartialView(IdentificationTypes);
        }
    }
}