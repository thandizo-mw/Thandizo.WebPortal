﻿using AngleDimension.Standard.Http.HttpServices;
using IdentityModel;
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

namespace Thandizo.WebPortal.Controllers
{
    public class CountriesController : Controller
    {
        private readonly IConfiguration _configuration;
        

        public CountriesController(IConfiguration configuration)
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
            string url = $"{CoreApiUrl}Countries/GetAll";
            var Countries = Enumerable.Empty<CountryDTO>();

            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var response = await HttpRequestFactory.Get(accessToken, url);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                Countries = response.ContentAsType<IEnumerable<CountryDTO>>();
            }
            else
            {
                ModelState.AddModelError("", HttpResponseHandler.Process(response));
            }
            return View(Countries);
        }

        [HandleExceptionFilter]
        public IActionResult Create()
        {
            return View(new CountryDTO
            {
                CreatedBy = AppContextHelper.GetStringValueClaim(HttpContext, JwtClaimTypes.Name)
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [HandleExceptionFilter]
        public async Task<IActionResult> Create([Bind] CountryDTO country)
        {
            string url = $"{CoreApiUrl}Countries/Add";

            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var response = await HttpRequestFactory.Post(accessToken, url, country);

            if (response.StatusCode == HttpStatusCode.Created)
            {
                AppContextHelper.SetToastMessage("Country has been successfully created", MessageType.Success, 1, Response);
                return RedirectToAction(nameof(Index));
            }
            else
            {
                AppContextHelper.SetToastMessage("Failed to create country", MessageType.Danger, 1, Response);
                ModelState.AddModelError("", HttpResponseHandler.Process(response));
            }

            return View(country);
        }

        [HandleExceptionFilter]
        public async Task<IActionResult> Edit([FromQuery] string countryCode)
        {
            return View(await GetCountry(countryCode));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [HandleExceptionFilter]
        public async Task<IActionResult> Edit([Bind]CountryDTO country)
        {
            string url = $"{CoreApiUrl}Countries/Update";

            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var response = await HttpRequestFactory.Put(accessToken, url,  country);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                AppContextHelper.SetToastMessage("Country has been successfully updated", MessageType.Success, 1, Response);
                return RedirectToAction(nameof(Index));
            }
            else
            {
                AppContextHelper.SetToastMessage("Failed to update country", MessageType.Danger, 1, Response);
                ModelState.AddModelError("", HttpResponseHandler.Process(response));
            }
            return View(country);
        }

        [HandleExceptionFilter]
        public async Task<IActionResult> Details([FromQuery] string countryCode)
        {
            return View(await GetCountry(countryCode));
        }

        [HandleExceptionFilter]
        public async Task<IActionResult> Delete([FromQuery] string countryCode)
        {
            var Country = await GetCountry(countryCode);
            return View(Country);
        }

        [HttpPost, ActionName("Delete")]
        [HandleExceptionFilter]
        public async Task<IActionResult> VerifyDelete(string countryCode)
        {
            string url = $"{CoreApiUrl}Countries/Delete?countryCode={countryCode}";
            var Country = new CountryDTO();

            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var response = await HttpRequestFactory.Delete(accessToken, url); 

            if (response.StatusCode == HttpStatusCode.OK)
            {
                AppContextHelper.SetToastMessage("Country has been successfully deleted", MessageType.Success, 1, Response);
                return RedirectToAction(nameof(Index));
            }
            else
            {
                AppContextHelper.SetToastMessage("Failed to delete country", MessageType.Danger, 1, Response);
                ModelState.AddModelError("", HttpResponseHandler.Process(response));
            }
            return View(await GetCountry(countryCode));
        }

        private async Task<CountryDTO> GetCountry(string countryCode)
        {
            string url = $"{CoreApiUrl}Countries/GetByCode?countryCode={countryCode}";
            var Country = new CountryDTO();

            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var response = await HttpRequestFactory.Get(accessToken, url);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                Country = response.ContentAsType<CountryDTO>();
            }
            else
            {
                ModelState.AddModelError("", HttpResponseHandler.Process(response));
            }
            return Country;
        }


        [HandleExceptionFilter]
        public async Task<IActionResult> GetCountries()
        {
            string url = $"{CoreApiUrl}Countries/GetAll";
            var Countries = Enumerable.Empty<CountryDTO>();

            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var response = await HttpRequestFactory.Get(accessToken, url);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                Countries = response.ContentAsType<IEnumerable<CountryDTO>>();
            }
            else
            {
                ModelState.AddModelError("", HttpResponseHandler.Process(response));
            }
            return PartialView(Countries);
        }
    }
}