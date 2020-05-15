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
using Thandizo.DataModels.Notifications;
using Thandizo.DataModels.Notifications.Responses;
using Thandizo.DataModels.ViewModels.Notifications;
using Thandizo.WebPortal.Filters;
using Thandizo.WebPortal.Helpers;
using Thandizo.WebPortal.Helpers.General;
using Thandizo.WebPortal.Services;

namespace Thandizo.WebPortal.Controllers
{
    public class ScheduledNotificationsController : Controller
    {
        private readonly IConfiguration _configuration;
        

        public ScheduledNotificationsController(IConfiguration configuration)
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
            string url = $"{NotificationsApiUrl}ScheduledNotifications/GetAll";
            var scheduledNotifications = Enumerable.Empty<ScheduledNotificationResponse>();

            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var response = await HttpRequestFactory.Get(accessToken, url);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                scheduledNotifications = response.ContentAsType<IEnumerable<ScheduledNotificationResponse>>();
            }
            else
            {
                ModelState.AddModelError("", HttpResponseHandler.Process(response));
            }
            return View(scheduledNotifications);
        }

        [HandleExceptionFilter]
        public async Task<IActionResult> Create()
        {
            return View(new ScheduledNotificationViewModel
            {
                ScheduledNotification = new ScheduledNotificationDTO
                {
                    CreatedBy = AppContextHelper.GetStringValueClaim(HttpContext, JwtClaimTypes.Name),
                    IsActive = true,
                    StartDate = DateTime.UtcNow.AddHours(2)
                },
                EscalationRules = await GetEscalationRules(),
                NotificationTemplates = await GetNotificationTemplates(),
                NotificationChannels = await GetNotificationChannels()
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [HandleExceptionFilter]
        public async Task<IActionResult> Create([Bind] ScheduledNotificationViewModel scheduledNotificationViewModel)
        {
            ScheduledNotificationDTO scheduledNotification = scheduledNotificationViewModel.ScheduledNotification;

            string url = $"{NotificationsApiUrl}ScheduledNotifications/Add";

            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var response = await HttpRequestFactory.Post(accessToken, url, scheduledNotification);

            if (response.StatusCode == HttpStatusCode.Created)
            {
                AppContextHelper.SetToastMessage("Scheduled notification has been successfully created", MessageType.Success, 1, Response);
                return RedirectToAction(nameof(Index));
            }
            else
            {
                AppContextHelper.SetToastMessage("Failed to create scheduled notification", MessageType.Danger, 1, Response);
                ModelState.AddModelError("", HttpResponseHandler.Process(response));
            }

            return View(scheduledNotification);
        }

        [HandleExceptionFilter]
        public async Task<IActionResult> Edit([FromQuery] int notificationId)
        {
            return View(new ScheduledNotificationViewModel 
            {
                ScheduledNotification = await GetScheduledNotification(notificationId),
                NotificationTemplates = await GetNotificationTemplates(),
                EscalationRules = await GetEscalationRules(),
                NotificationChannels = await GetNotificationChannels()
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [HandleExceptionFilter]
        public async Task<IActionResult> Edit([Bind]ScheduledNotificationViewModel scheduledNotificationViewModel)
        {
            ScheduledNotificationDTO scheduledNotification = scheduledNotificationViewModel.ScheduledNotification;

            string url = $"{NotificationsApiUrl}ScheduledNotifications/Update";

            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var response = await HttpRequestFactory.Put(accessToken, url,  scheduledNotification);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                AppContextHelper.SetToastMessage("Scheduled notification has been successfully updated", MessageType.Success, 1, Response);
                return RedirectToAction(nameof(Index));
            }
            else
            {
                AppContextHelper.SetToastMessage("Failed to update scheduled notification", MessageType.Danger, 1, Response);
                ModelState.AddModelError("", HttpResponseHandler.Process(response));
            }

            scheduledNotificationViewModel.EscalationRules = await GetEscalationRules();
            scheduledNotificationViewModel.NotificationTemplates = await GetNotificationTemplates();
            scheduledNotificationViewModel.NotificationChannels = await GetNotificationChannels();
            return View(scheduledNotificationViewModel);
        }

        [HandleExceptionFilter]
        public async Task<IActionResult> Details([FromQuery] int notificationId)
        {
            return View(await GetScheduledNotification(notificationId));
        }

        [HandleExceptionFilter]
        public async Task<IActionResult> Delete([FromQuery] int notificationId)
        {
            var ScheduledNotification = await GetScheduledNotification(notificationId);
            return View(ScheduledNotification);
        }

        [HttpPost, ActionName("Delete")]
        [HandleExceptionFilter]
        public async Task<IActionResult> VerifyDelete(int notificationId)
        {
            string url = $"{NotificationsApiUrl}ScheduledNotifications/Delete?notificationId={notificationId}";
            var ScheduledNotification = new ScheduledNotificationDTO();

            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var response = await HttpRequestFactory.Delete(accessToken, url); 

            if (response.StatusCode == HttpStatusCode.OK)
            {
                AppContextHelper.SetToastMessage("Scheduled notification has been successfully deleted", MessageType.Success, 1, Response);
                return RedirectToAction(nameof(Index));
            }
            else
            {
                AppContextHelper.SetToastMessage("Failed to delete scheduled notification", MessageType.Danger, 1, Response);
                ModelState.AddModelError("", HttpResponseHandler.Process(response));
            }
            return View(await GetScheduledNotification(notificationId));
        }

        private async Task<ScheduledNotificationResponse> GetScheduledNotification(int notificationId)
        {
            string url = $"{NotificationsApiUrl}ScheduledNotifications/GetById?notificationId={notificationId}";
            var scheduledNotification = new ScheduledNotificationResponse();

            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var response = await HttpRequestFactory.Get(accessToken, url);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                scheduledNotification = response.ContentAsType<ScheduledNotificationResponse>();
            }
            else
            {
                ModelState.AddModelError("", HttpResponseHandler.Process(response));
            }
            return scheduledNotification;
        }


        [HandleExceptionFilter]
        public async Task<IActionResult> GetScheduledNotifications()
        {
            string url = $"{NotificationsApiUrl}ScheduledNotifications/GetAll";
            var ScheduledNotifications = Enumerable.Empty<ScheduledNotificationDTO>();

            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var response = await HttpRequestFactory.Get(accessToken, url);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                ScheduledNotifications = response.ContentAsType<IEnumerable<ScheduledNotificationDTO>>();
            }
            else
            {
                ModelState.AddModelError("", HttpResponseHandler.Process(response));
            }
            return PartialView(ScheduledNotifications);
        }

        public async Task<IEnumerable<ScheduledNotificationEscalationRuleDTO>> GetEscalationRules()
        {
            string url = $"{NotificationsApiUrl}ScheduledNotificationEscalationRules/GetAll";
            var scheduledNotificationEscalationRules = Enumerable.Empty<ScheduledNotificationEscalationRuleDTO>();

            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var response = await HttpRequestFactory.Get(accessToken, url);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                scheduledNotificationEscalationRules = response.ContentAsType<IEnumerable<ScheduledNotificationEscalationRuleDTO>>();
            }
            else
            {
                ModelState.AddModelError("", HttpResponseHandler.Process(response));
            }
            return scheduledNotificationEscalationRules;
        }

        public async Task<IEnumerable<NotificationTemplateDTO>> GetNotificationTemplates()
        {
            string url = $"{NotificationsApiUrl}NotificationTemplates/GetAll";
            var notificationTemplateDTO = Enumerable.Empty<NotificationTemplateDTO>();

            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var response = await HttpRequestFactory.Get(accessToken, url);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                notificationTemplateDTO = response.ContentAsType<IEnumerable<NotificationTemplateDTO>>();
            }
            else
            {
                ModelState.AddModelError("", HttpResponseHandler.Process(response));
            }
            return notificationTemplateDTO;
        }

        public async Task<IEnumerable<NotificationChannelDTO>> GetNotificationChannels()
        {
            string url = $"{NotificationsApiUrl}NotificationChannels/GetAll";
            var NotificationChannels = Enumerable.Empty<NotificationChannelDTO>();

            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var response = await HttpRequestFactory.Get(accessToken, url);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                NotificationChannels = response.ContentAsType<IEnumerable<NotificationChannelDTO>>();
            }
            else
            {
                ModelState.AddModelError("", HttpResponseHandler.Process(response));
            }
            return NotificationChannels;
        }
    }
}