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
    public class ResponseTeamMembersController : Controller
    {
        private readonly IConfiguration _configuration;
        IHttpRequestHandler _httpRequestHandler;

        public ResponseTeamMembersController(IConfiguration configuration, IHttpRequestHandler httpRequestHandler)
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
            string url = $"{CoreApiUrl}ResponseTeamMembers/GetAll";
            var ResponseTeamMembers = Enumerable.Empty<ResponseTeamMemberDTO>();

            var response = await _httpRequestHandler.Get(url);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                ResponseTeamMembers = response.ContentAsType<IEnumerable<ResponseTeamMemberDTO>>();
            }
            else
            {
                ModelState.AddModelError("", HttpResponseHandler.Process(response));

            }

            return View(ResponseTeamMembers);
        }

        public IActionResult Create()
        {
            return View(new ResponseTeamMemberDTO
            {
                CreatedBy = "SYS"
            });
        }

        [HttpPost]
        public async Task<IActionResult> Create([Bind] ResponseTeamMemberDTO responseTeamMember)
        {
            string url = $"{CoreApiUrl}ResponseTeamMembers/Add";

            var response = await _httpRequestHandler.Post(url, responseTeamMember);

            if (response.StatusCode == HttpStatusCode.Created)
            {
                AppContextHelper.SetToastMessage("ResponseTeamMember has been successfully created", MessageType.Success, 1, Response);
                return RedirectToAction(nameof(Index));
            }
            else
            {
                AppContextHelper.SetToastMessage("Failed to create response team member", MessageType.Danger, 1, Response);
                TempData["ModelError"] = HttpResponseHandler.Process(response);
            }
            return RedirectToAction(nameof(Create));
        }

        public async Task<IActionResult> Edit([FromQuery] int teamMemberId)
        {
            var responseTeamMember = await getResponseTeamMember(teamMemberId);
            return View(responseTeamMember);
        }

        [HttpPost]
        public async Task<IActionResult> Edit([Bind]ResponseTeamMemberDTO responseTeamMember)
        {
            string url = $"{CoreApiUrl}ResponseTeamMembers/Update";

            var response = await _httpRequestHandler.Put(url, responseTeamMember);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                AppContextHelper.SetToastMessage("Response team member has been successfully updated", MessageType.Success, 1, Response);
                return RedirectToAction(nameof(Index));
            }
            else
            {
                AppContextHelper.SetToastMessage("Failed to update response team member", MessageType.Danger, 1, Response);
                TempData["ModelError"] = HttpResponseHandler.Process(response);
            }
            return RedirectToAction(nameof(Edit), new { teamMemberId = responseTeamMember.TeamMemberId});
        }

        public async Task<IActionResult> Details([FromQuery] int teamMemberId)
        {
            var responseTeamMember = await getResponseTeamMember(teamMemberId);
            return View(responseTeamMember);
        }

        public async Task<IActionResult> Delete([FromQuery] int teamMemberId)
        {
            var responseTeamMember = await getResponseTeamMember(teamMemberId);
            return View(responseTeamMember);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> VerifyDelete(long teamMemberId)
        {
            string url = $"{CoreApiUrl}ResponseTeamMembers/Delete?teamMemberId={teamMemberId}";
            var ResponseTeamMember = new ResponseTeamMemberDTO();

            var response = await _httpRequestHandler.Delete(url);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                AppContextHelper.SetToastMessage("Response team tember has been successfully deleted", MessageType.Success, 1, Response);
                return RedirectToAction(nameof(Delete));
            }
            else
            {
                AppContextHelper.SetToastMessage("Failed to delete response team member", MessageType.Danger, 1, Response);
                TempData["ModelError"] = HttpResponseHandler.Process(response);
            }
            return RedirectToAction(nameof(Delete), new { teamMemberId });
        }

        private async Task<ResponseTeamMemberDTO> getResponseTeamMember(int teamMemberId)
        {
            string url = $"{CoreApiUrl}ResponseTeamMembers/GetById?teamMemberId={teamMemberId}";
            var responseTeamMember = new ResponseTeamMemberDTO();

            var response = await _httpRequestHandler.Get(url);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                responseTeamMember = response.ContentAsType<ResponseTeamMemberDTO>();
            }
            else
            {
                ModelState.AddModelError("", HttpResponseHandler.Process(response));

            }
            return responseTeamMember;
        }
    }
}