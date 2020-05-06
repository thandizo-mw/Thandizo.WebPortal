using AngleDimension.Standard.Http.HttpServices;
using IdentityModel;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Thandizo.DataModels.Notifications;
using Thandizo.WebPortal.Filters;
using Thandizo.WebPortal.Helpers;
using Thandizo.WebPortal.Helpers.General;
using Thandizo.WebPortal.Services;

namespace Thandizo.WebPortal.Controllers
{
    public class ScheduledNotificationEscalationRulesController : Controller
    {
        private readonly IConfiguration _configuration;
        

        public ScheduledNotificationEscalationRulesController(IConfiguration configuration)
        {
            _configuration = configuration;
            
        }

        public string NotificationsApiUrl
        {
            get
            {
                return _configuration["NotificationsApiUrl"];
            }
        }

        [HandleExceptionFilter]
        public async Task<IActionResult> Index()
        {
            string url = $"{NotificationsApiUrl}ScheduledNotificationEscalationRules/GetAll";
            var ScheduledNotificationEscalationRules = Enumerable.Empty<ScheduledNotificationEscalationRuleDTO>();

            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var response = await HttpRequestFactory.Get(accessToken, url);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                ScheduledNotificationEscalationRules = response.ContentAsType<IEnumerable<ScheduledNotificationEscalationRuleDTO>>();
            }
            else
            {
                ModelState.AddModelError("", HttpResponseHandler.Process(response));
            }
            return View(ScheduledNotificationEscalationRules);
        }

        [HandleExceptionFilter]
        public IActionResult Create()
        {
            return View(new ScheduledNotificationEscalationRuleDTO
            {
                CreatedBy = AppContextHelper.GetStringValueClaim(HttpContext, JwtClaimTypes.Name)
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [HandleExceptionFilter]
        public async Task<IActionResult> Create([Bind] ScheduledNotificationEscalationRuleDTO escalationRule)
        {
            string url = $"{NotificationsApiUrl}ScheduledNotificationEscalationRules/Add";

            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var response = await HttpRequestFactory.Post(accessToken, url, escalationRule);

            if (response.StatusCode == HttpStatusCode.Created)
            {
                AppContextHelper.SetToastMessage("Scheduled notification escalation rule has been successfully created", MessageType.Success, 1, Response);
                return RedirectToAction(nameof(Index));
            }
            else
            {
                AppContextHelper.SetToastMessage("Failed to create scheduled notification escalation rule", MessageType.Danger, 1, Response);
                ModelState.AddModelError("", HttpResponseHandler.Process(response));
            }

            return View(escalationRule);
        }

        [HandleExceptionFilter]
        public async Task<IActionResult> Edit([FromQuery] int ruleId)
        {
            return View(await GetScheduledNotificationEscalationRule(ruleId));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [HandleExceptionFilter]
        public async Task<IActionResult> Edit([Bind]ScheduledNotificationEscalationRuleDTO escalationRule)
        {
            string url = $"{NotificationsApiUrl}ScheduledNotificationEscalationRules/Update";

            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var response = await HttpRequestFactory.Put(accessToken, url,  escalationRule);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                AppContextHelper.SetToastMessage("Scheduled notification escalation rule has been successfully updated", MessageType.Success, 1, Response);
                return RedirectToAction(nameof(Index));
            }
            else
            {
                AppContextHelper.SetToastMessage("Failed to update scheduled notification escalation rule", MessageType.Danger, 1, Response);
                ModelState.AddModelError("", HttpResponseHandler.Process(response));
            }
            return View(escalationRule);
        }

        [HandleExceptionFilter]
        public async Task<IActionResult> Details([FromQuery] int ruleId)
        {
            return View(await GetScheduledNotificationEscalationRule(ruleId));
        }

        [HandleExceptionFilter]
        public async Task<IActionResult> Delete([FromQuery] int ruleId)
        {
            var ScheduledNotificationEscalationRule = await GetScheduledNotificationEscalationRule(ruleId);
            return View(ScheduledNotificationEscalationRule);
        }

        [HttpPost, ActionName("Delete")]
        [HandleExceptionFilter]
        public async Task<IActionResult> VerifyDelete(int ruleId)
        {
            string url = $"{NotificationsApiUrl}ScheduledNotificationEscalationRules/Delete?ruleId={ruleId}";
            var ScheduledNotificationEscalationRule = new ScheduledNotificationEscalationRuleDTO();

            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var response = await HttpRequestFactory.Delete(accessToken, url); 

            if (response.StatusCode == HttpStatusCode.OK)
            {
                AppContextHelper.SetToastMessage("Scheduled notification escalation rule has been successfully deleted", MessageType.Success, 1, Response);
                return RedirectToAction(nameof(Index));
            }
            else
            {
                AppContextHelper.SetToastMessage("Failed to delete scheduled notification escalation rule", MessageType.Danger, 1, Response);
                ModelState.AddModelError("", HttpResponseHandler.Process(response));
            }
            return View(await GetScheduledNotificationEscalationRule(ruleId));
        }

        private async Task<ScheduledNotificationEscalationRuleDTO> GetScheduledNotificationEscalationRule(int ruleId)
        {
            string url = $"{NotificationsApiUrl}ScheduledNotificationEscalationRules/GetById?ruleId={ruleId}";
            var ScheduledNotificationEscalationRule = new ScheduledNotificationEscalationRuleDTO();

            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var response = await HttpRequestFactory.Get(accessToken, url);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                ScheduledNotificationEscalationRule = response.ContentAsType<ScheduledNotificationEscalationRuleDTO>();
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
            return ScheduledNotificationEscalationRule;
        }


        [HandleExceptionFilter]
        public async Task<IActionResult> GetScheduledNotificationEscalationRules()
        {
            string url = $"{NotificationsApiUrl}ScheduledNotificationEscalationRules/GetAll";
            var ScheduledNotificationEscalationRules = Enumerable.Empty<ScheduledNotificationEscalationRuleDTO>();

            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var response = await HttpRequestFactory.Get(accessToken, url);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                ScheduledNotificationEscalationRules = response.ContentAsType<IEnumerable<ScheduledNotificationEscalationRuleDTO>>();
            }
            else
            {
                ModelState.AddModelError("", HttpResponseHandler.Process(response));
            }
            return PartialView(ScheduledNotificationEscalationRules);
        }
    }
}