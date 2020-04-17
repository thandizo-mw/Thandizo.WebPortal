using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Thandizo.DataModels.Core;
using Thandizo.DataModels.Patients.Responses;
using Thandizo.WebPortal.Helpers;
using Thandizo.WebPortal.Helpers.General;
using Thandizo.WebPortal.Services;

namespace Thandizo.WebPortal.Controllers
{
    public class ResponseTeamMappingsController : Controller
    {
        private readonly IConfiguration _configuration;
        IHttpRequestHandler _httpRequestHandler;

        public ResponseTeamMappingsController(IConfiguration configuration, IHttpRequestHandler httpRequestHandler)
        {
            _configuration = configuration;
            _httpRequestHandler = httpRequestHandler;
        }

        public string CoreApiUrl
        {
            get
            {
                return _configuration["CoreApiUrl"];
            }
        }

        public async Task<IActionResult> Index()
        {
            string url = $"{CoreApiUrl}ResponseTeamMappings/GetAll";
            var ResponseTeamMappings = Enumerable.Empty<ResponseTeamMappingDTO>();

            var response = await _httpRequestHandler.Get(url);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                ResponseTeamMappings = response.ContentAsType<IEnumerable<ResponseTeamMappingDTO>>();
            }
            else
            {
                ModelState.AddModelError("", HttpResponseHandler.Process(response));
            }
            return View(ResponseTeamMappings);
        }

        public IActionResult Create()
        {
            return View(new ResponseTeamMappingDTO
            {
                CreatedBy = "SYS"
            });
        }

        [HttpPost]
        public async Task<IActionResult> Create([Bind] ResponseTeamMappingDTO responseTeamMember)
        {
            string url = $"{CoreApiUrl}ResponseTeamMappings/Add";
            var response = await _httpRequestHandler.Post(url, responseTeamMember);

            if (response.StatusCode == HttpStatusCode.Created)
            {
                AppContextHelper.SetToastMessage("Response team mapping has been successfully created", MessageType.Success, 1, Response);
                return RedirectToAction(nameof(Index));
            }
            else
            {
                AppContextHelper.SetToastMessage("Failed to create response team mapping", MessageType.Danger, 1, Response);
                ModelState.AddModelError("", HttpResponseHandler.Process(response));
            }

            return View(responseTeamMember);
        }

        public async Task<IActionResult> Edit([FromQuery] int teamMemberId)
        {
            var responseTeamMember = await getResponseTeamMapping(teamMemberId);
            return View(responseTeamMember);
        }

        [HttpPost]
        public async Task<IActionResult> Edit([Bind]ResponseTeamMappingDTO responseTeamMember)
        {
            string url = $"{CoreApiUrl}ResponseTeamMappings/Update";

            var response = await _httpRequestHandler.Put(url, responseTeamMember);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                AppContextHelper.SetToastMessage("Response team mapping has been successfully updated", MessageType.Success, 1, Response);
                return RedirectToAction(nameof(Index));
            }
            else
            {
                AppContextHelper.SetToastMessage("Failed to update response team mapping", MessageType.Danger, 1, Response);
                ModelState.AddModelError("", HttpResponseHandler.Process(response));
            }
            return View(responseTeamMember);
        }

        public async Task<IActionResult> Details([FromQuery] int teamMemberId)
        {
            var responseTeamMember = await getResponseTeamMapping(teamMemberId);
            return View(responseTeamMember);
        }

        public async Task<IActionResult> Delete([FromQuery] int teamMemberId)
        {
            var responseTeamMember = await getResponseTeamMapping(teamMemberId);
            return View(responseTeamMember);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> VerifyDelete(long teamMemberId)
        {
            string url = $"{CoreApiUrl}ResponseTeamMappings/Delete?teamMemberId={teamMemberId}";
            var ResponseTeamMapping = new ResponseTeamMappingDTO();

            var response = await _httpRequestHandler.Delete(url);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                AppContextHelper.SetToastMessage("Response team tember has been successfully deleted", MessageType.Success, 1, Response);
                return RedirectToAction(nameof(Index));
            }
            else
            {
                AppContextHelper.SetToastMessage("Failed to delete response team mapping", MessageType.Danger, 1, Response);
                TempData["ModelError"] = HttpResponseHandler.Process(response);
            }
            return RedirectToAction(nameof(Delete), new { teamMemberId });
        }

        private async Task<ResponseTeamMappingDTO> getResponseTeamMapping(int teamMemberId)
        {
            string url = $"{CoreApiUrl}ResponseTeamMappings/GetById?teamMemberId={teamMemberId}";
            var responseTeamMember = new ResponseTeamMappingDTO();

            var response = await _httpRequestHandler.Get(url);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                responseTeamMember = response.ContentAsType<ResponseTeamMappingDTO>();
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
            return responseTeamMember;
        }
    }
}