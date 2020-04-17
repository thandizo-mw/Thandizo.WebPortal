using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
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

namespace Thandizo.WebPortal.Controllers
{
    public class ResponseTeamMappingsController : Controller
    {
        private readonly IConfiguration _configuration;
        IHttpRequestHandler _httpRequestHandler;
        static int _teamMemberId;
        static string _teamMemberName;

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
        [HandleExceptionFilter]
        public async Task<IActionResult> Index(int teamMemberId, string teamMemberName)
        {
            if (teamMemberId == 0 && (teamMemberName.Equals("") || teamMemberName.Equals(null)))
            {
                return RedirectToAction("Index", "ResponseTeamMembers");
            }

            ViewBag.TeamMemberName = teamMemberName;
            _teamMemberName = teamMemberName;
            _teamMemberId = teamMemberId;
            string url = $"{CoreApiUrl}ResponseTeamMappings/GetByMember?teamMemberId={teamMemberId}";
            var ResponseTeamMembers = Enumerable.Empty<TeamMappingResponse>();

            var response = await _httpRequestHandler.Get(url);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                ResponseTeamMembers = response.ContentAsType<IEnumerable<TeamMappingResponse>>();
            }
            else
            {
                ModelState.AddModelError("", HttpResponseHandler.Process(response));
            }
            return View(ResponseTeamMembers);
        }

        [HandleExceptionFilter]
        public IActionResult Create()
        {
            ViewBag.TeamMemberName = _teamMemberName;
            if (_teamMemberId == 0)
            {
                return RedirectToAction("Index", "ResponseTeamMembers");
            }
            return View(new TeamMappingResponse
            {
                MappingId = _teamMemberId,
                CreatedBy = "SYS"
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [HandleExceptionFilter]
        public async Task<IActionResult> Create([Bind] TeamMappingResponse teamMappingResponse)
        {
            ResponseTeamMappingDTO responseTeamMember = new ResponseTeamMappingDTO
            {
                CreatedBy = teamMappingResponse.CreatedBy,
                DistrictCode = teamMappingResponse.DistrictCode,
                MappingId = teamMappingResponse.MappingId,
                TeamMemberId = teamMappingResponse.TeamMemberId
            };

            string url = $"{CoreApiUrl}ResponseTeamMappings/Add";
            var response = await _httpRequestHandler.Post(url, responseTeamMember);

            if (response.StatusCode == HttpStatusCode.Created)
            {
                AppContextHelper.SetToastMessage("Response team member has been successfully mapped", MessageType.Success, 1, Response);
                return RedirectToAction(nameof(Index), new { teamMemberId = _teamMemberId, teamMemberName = _teamMemberName });
            }
            else
            {
                AppContextHelper.SetToastMessage("Failed to map response team member", MessageType.Danger, 1, Response);
                ModelState.AddModelError("", HttpResponseHandler.Process(response));
            }

            return View(responseTeamMember);
        }

        [HandleExceptionFilter]
        public async Task<IActionResult> Edit([FromQuery] int mappingId)
        {
            ViewBag.TeamMemberName = _teamMemberName;
            var responseTeamMember = await getResponseTeamMapping(mappingId);
            return View(responseTeamMember);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [HandleExceptionFilter]
        public async Task<IActionResult> Edit([Bind] TeamMappingResponse teamMappingResponse)
        {
            ResponseTeamMappingDTO responseTeamMember = new ResponseTeamMappingDTO
            {
                CreatedBy = teamMappingResponse.CreatedBy,
                DistrictCode = teamMappingResponse.DistrictCode,
                MappingId = teamMappingResponse.MappingId,
                TeamMemberId = teamMappingResponse.TeamMemberId
            };

            string url = $"{CoreApiUrl}ResponseTeamMappings/Update";

            var response = await _httpRequestHandler.Put(url, responseTeamMember);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                AppContextHelper.SetToastMessage("Response team member mapping has been successfully updated", MessageType.Success, 1, Response);
                return RedirectToAction(nameof(Index), new { teamMemberId = _teamMemberId, teamMemberName = _teamMemberName });
            }
            else
            {
                AppContextHelper.SetToastMessage("Failed to update response team member mapping", MessageType.Danger, 1, Response);
                ModelState.AddModelError("", HttpResponseHandler.Process(response));
                ViewBag.TeamMemberName = _teamMemberName;
            }
            return View(teamMappingResponse);
        }

        [HandleExceptionFilter]
        public async Task<IActionResult> Details([FromQuery] int mappingId)
        {
            ViewBag.TeamMemberName = _teamMemberName;
            var responseTeamMember = await getResponseTeamMapping(mappingId);
            return View(responseTeamMember);
        }

        [HandleExceptionFilter]
        public async Task<IActionResult> Delete([FromQuery] int mappingId)
        {
            ViewBag.TeamMemberName = _teamMemberName;
            var responseTeamMember = await getResponseTeamMapping(mappingId);
            return View(responseTeamMember);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [HandleExceptionFilter]
        public async Task<IActionResult> VerifyDelete(int mappingId)
        {
            string url = $"{CoreApiUrl}ResponseTeamMappings/Delete?mappingId={mappingId}";

            var response = await _httpRequestHandler.Delete(url);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                AppContextHelper.SetToastMessage("Response team member mapping has been successfully deleted", MessageType.Success, 1, Response);
                return RedirectToAction(nameof(Index), new { teamMemberId = _teamMemberId, teamMemberName = _teamMemberName });
            }
            else
            {
                AppContextHelper.SetToastMessage("Failed to delete response team member mapping", MessageType.Danger, 1, Response);
                TempData["ModelError"] = HttpResponseHandler.Process(response);
            }
            return RedirectToAction(nameof(Delete), new { mappingId });
        }

        private async Task<TeamMappingResponse> getResponseTeamMapping(int mappingId)
        {
            string url = $"{CoreApiUrl}ResponseTeamMappings/GetById?mappingId={mappingId}";
            var responseTeamMember = new TeamMappingResponse();

            var response = await _httpRequestHandler.Get(url);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                responseTeamMember = response.ContentAsType<TeamMappingResponse>();
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