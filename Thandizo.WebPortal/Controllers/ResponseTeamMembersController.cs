using AngleDimension.Standard.Http.HttpServices;
using IdentityModel;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Thandizo.DataModels.Core;
using Thandizo.DataModels.Identity.DataTransfer;
using Thandizo.DataModels.Patients.Responses;
using Thandizo.WebPortal.Filters;
using Thandizo.WebPortal.Helpers;
using Thandizo.WebPortal.Helpers.General;
using Thandizo.WebPortal.Services;

namespace Thandizo.WebPortal.Controllers
{
    public class ResponseTeamMembersController : Controller
    {
        private readonly IConfiguration _configuration;

        public ResponseTeamMembersController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string IdentityServerAuthority
        {
            get
            {
                return _configuration["IdentityServerAuthority"];
            }
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
            string url = $"{CoreApiUrl}ResponseTeamMembers/GetAll";
            var ResponseTeamMembers = Enumerable.Empty<ResponseTeamMemberDTO>();

            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var response = await HttpRequestFactory.Get(accessToken, url);

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

        [HandleExceptionFilter]
        public IActionResult Create()
        {
            return View(new ResponseTeamMemberDTO
            {
                CreatedBy = AppContextHelper.GetStringValueClaim(HttpContext, JwtClaimTypes.Name)
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [HandleExceptionFilter]
        public async Task<IActionResult> Create([Bind] ResponseTeamMemberDTO responseTeamMember)
        {
            string url = $"{CoreApiUrl}ResponseTeamMembers/Add";

            //Clean up phoner number
            var sanitizedNumber = PhoneNumberSanitizer.Sanitize(responseTeamMember.PhoneNumber, "+265");
            responseTeamMember.PhoneNumber = sanitizedNumber;
            var fullName = $"{responseTeamMember.FirstName} {responseTeamMember.Surname}";

            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var response = await HttpRequestFactory.Post(accessToken, url, responseTeamMember);

            if (response.StatusCode == HttpStatusCode.Created)
            {
                var identityResponse =  await HttpRequestFactory.Post($"{IdentityServerAuthority}/api/Users/RegisterUser", new UserDTO { PhoneNumber = responseTeamMember.PhoneNumber, FullName = fullName });
                if (identityResponse.StatusCode == HttpStatusCode.Created)
                {
                    AppContextHelper.SetToastMessage("User account has been successfully created", MessageType.Danger, 1, Response);
                }
                AppContextHelper.SetToastMessage("Response team member has been successfully created", MessageType.Success, 1, Response);
                return RedirectToAction(nameof(Index));
            }
            else
            {
                AppContextHelper.SetToastMessage("Failed to create response team member", MessageType.Danger, 1, Response);
                ModelState.AddModelError("", HttpResponseHandler.Process(response));
            }

            return View(responseTeamMember);
        }

        [HandleExceptionFilter]
        public async Task<IActionResult> Edit([FromQuery] int teamMemberId)
        {
            var responseTeamMember = await GetResponseTeamMember(teamMemberId);
            return View(responseTeamMember);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [HandleExceptionFilter]
        public async Task<IActionResult> Edit([Bind]ResponseTeamMemberDTO responseTeamMember)
        {
            string url = $"{CoreApiUrl}ResponseTeamMembers/Update";

            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var response = await HttpRequestFactory.Put(accessToken, url,  responseTeamMember);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                AppContextHelper.SetToastMessage("Response team member has been successfully updated", MessageType.Success, 1, Response);
                return RedirectToAction(nameof(Index));
            }
            else
            {
                AppContextHelper.SetToastMessage("Failed to update response team member", MessageType.Danger, 1, Response);
                ModelState.AddModelError("", HttpResponseHandler.Process(response));
            }
            return View(responseTeamMember);
        }

        [HandleExceptionFilter]
        public async Task<IActionResult> Details([FromQuery] int teamMemberId)
        {
            return View(await GetResponseTeamMember(teamMemberId));
        }

        [HandleExceptionFilter]
        public async Task<IActionResult> Delete([FromQuery] int teamMemberId)
        {
            return View(await GetResponseTeamMember(teamMemberId));
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [HandleExceptionFilter]
        public async Task<IActionResult> VerifyDelete(int teamMemberId)
        {
            string url = $"{CoreApiUrl}ResponseTeamMembers/Delete?teamMemberId={teamMemberId}";

            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var response = await HttpRequestFactory.Delete(accessToken, url); 

            if (response.StatusCode == HttpStatusCode.OK)
            {
                AppContextHelper.SetToastMessage("Response team member has been successfully deleted", MessageType.Success, 1, Response);
                return RedirectToAction(nameof(Index));
            }
            else
            {
                AppContextHelper.SetToastMessage("Failed to delete response team member", MessageType.Danger, 1, Response);
                ModelState.AddModelError("", HttpResponseHandler.Process(response));
            }
            return View(await GetResponseTeamMember(teamMemberId));
        }

        private async Task<ResponseTeamMemberDTO> GetResponseTeamMember(int teamMemberId)
        {
            string url = $"{CoreApiUrl}ResponseTeamMembers/GetById?teamMemberId={teamMemberId}";
            var responseTeamMember = new ResponseTeamMemberDTO();

            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var response = await HttpRequestFactory.Get(accessToken, url);

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
        [HandleExceptionFilter]
        public async Task<IEnumerable<ResponseTeamMemberDTO>> GetResponseTeamMembers()
        {
            string url = $"{CoreApiUrl}ResponseTeamMembers/GetAll";
            var responseTeamMember = Enumerable.Empty<ResponseTeamMemberDTO>();

            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var response = await HttpRequestFactory.Get(accessToken, url);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                responseTeamMember = response.ContentAsType<IEnumerable<ResponseTeamMemberDTO>>();
            }
            else
            {
                ModelState.AddModelError("", HttpResponseHandler.Process(response));
            }
            return responseTeamMember;
        }
    }
}