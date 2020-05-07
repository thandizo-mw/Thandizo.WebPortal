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
    public class NotificationTemplatesController : Controller
    {
        private readonly IConfiguration _configuration;
        

        public NotificationTemplatesController(IConfiguration configuration)
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
            string url = $"{NotificationsApiUrl}NotificationTemplates/GetAll";
            var NotificationTemplates = Enumerable.Empty<NotificationTemplateDTO>();

            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var response = await HttpRequestFactory.Get(accessToken, url);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                NotificationTemplates = response.ContentAsType<IEnumerable<NotificationTemplateDTO>>();
            }
            else
            {
                ModelState.AddModelError("", HttpResponseHandler.Process(response));
            }
            return View(NotificationTemplates);
        }

        [HandleExceptionFilter]
        public IActionResult Create()
        {
            return View(new NotificationTemplateDTO
            {
                CreatedBy = AppContextHelper.GetStringValueClaim(HttpContext, JwtClaimTypes.Name)
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [HandleExceptionFilter]
        public async Task<IActionResult> Create([Bind] NotificationTemplateDTO notificationTemplate)
        {
            string url = $"{NotificationsApiUrl}NotificationTemplates/Add";

            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var response = await HttpRequestFactory.Post(accessToken, url, notificationTemplate);

            if (response.StatusCode == HttpStatusCode.Created)
            {
                AppContextHelper.SetToastMessage("Notification template has been successfully created", MessageType.Success, 1, Response);
                return RedirectToAction(nameof(Index));
            }
            else
            {
                AppContextHelper.SetToastMessage("Failed to create notification template", MessageType.Danger, 1, Response);
                ModelState.AddModelError("", HttpResponseHandler.Process(response));
            }

            return View(notificationTemplate);
        }

        [HandleExceptionFilter]
        public async Task<IActionResult> Edit([FromQuery] int templateId)
        {
            return View(await GetNotificationTemplate(templateId));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [HandleExceptionFilter]
        public async Task<IActionResult> Edit([Bind]NotificationTemplateDTO notificationTemplate)
        {
            string url = $"{NotificationsApiUrl}NotificationTemplates/Update";

            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var response = await HttpRequestFactory.Put(accessToken, url,  notificationTemplate);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                AppContextHelper.SetToastMessage("Notification template has been successfully updated", MessageType.Success, 1, Response);
                return RedirectToAction(nameof(Index));
            }
            else
            {
                AppContextHelper.SetToastMessage("Failed to update notification template", MessageType.Danger, 1, Response);
                ModelState.AddModelError("", HttpResponseHandler.Process(response));
            }
            return View(notificationTemplate);
        }

        [HandleExceptionFilter]
        public async Task<IActionResult> Details([FromQuery] int templateId)
        {
            return View(await GetNotificationTemplate(templateId));
        }

        [HandleExceptionFilter]
        public async Task<IActionResult> Delete([FromQuery] int templateId)
        {
            var NotificationTemplate = await GetNotificationTemplate(templateId);
            return View(NotificationTemplate);
        }

        [HttpPost, ActionName("Delete")]
        [HandleExceptionFilter]
        public async Task<IActionResult> VerifyDelete(int templateId)
        {
            string url = $"{NotificationsApiUrl}NotificationTemplates/Delete?templateId={templateId}";
            var NotificationTemplate = new NotificationTemplateDTO();

            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var response = await HttpRequestFactory.Delete(accessToken, url); 

            if (response.StatusCode == HttpStatusCode.OK)
            {
                AppContextHelper.SetToastMessage("Notification template has been successfully deleted", MessageType.Success, 1, Response);
                return RedirectToAction(nameof(Index));
            }
            else
            {
                AppContextHelper.SetToastMessage("Failed to delete notification template", MessageType.Danger, 1, Response);
                ModelState.AddModelError("", HttpResponseHandler.Process(response));
            }
            return View(await GetNotificationTemplate(templateId));
        }

        private async Task<NotificationTemplateDTO> GetNotificationTemplate(int templateId)
        {
            string url = $"{NotificationsApiUrl}NotificationTemplates/GetById?templateId={templateId}";
            var NotificationTemplate = new NotificationTemplateDTO();

            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var response = await HttpRequestFactory.Get(accessToken, url);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                NotificationTemplate = response.ContentAsType<NotificationTemplateDTO>();
            }
            else
            {
                ModelState.AddModelError("", HttpResponseHandler.Process(response));
            }
            return NotificationTemplate;
        }


        [HandleExceptionFilter]
        public async Task<IActionResult> GetNotificationTemplates()
        {
            string url = $"{NotificationsApiUrl}NotificationTemplates/GetAll";
            var NotificationTemplates = Enumerable.Empty<NotificationTemplateDTO>();

            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var response = await HttpRequestFactory.Get(accessToken, url);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                NotificationTemplates = response.ContentAsType<IEnumerable<NotificationTemplateDTO>>();
            }
            else
            {
                ModelState.AddModelError("", HttpResponseHandler.Process(response));
            }
            return PartialView(NotificationTemplates);
        }
    }
}