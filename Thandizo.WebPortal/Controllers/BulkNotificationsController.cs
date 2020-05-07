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
    public class BulkNotificationsController : Controller
    {
        private readonly IConfiguration _configuration;


        public BulkNotificationsController(IConfiguration configuration)
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
            string url = $"{NotificationsApiUrl}BulkNotifications/GetAll";
            var BulkNotifications = Enumerable.Empty<BulkNotificationResponse>();

            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var response = await HttpRequestFactory.Get(accessToken, url);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                BulkNotifications = response.ContentAsType<IEnumerable<BulkNotificationResponse>>();
            }
            else
            {
                ModelState.AddModelError("", HttpResponseHandler.Process(response));
            }
            return View(BulkNotifications);
        }

        [HandleExceptionFilter]
        public async Task<IActionResult> Create()
        {
            return View(new BulkNotificationViewModel
            {
                BulkNotification = new BulkNotificationDTO
                {
                    CreatedBy = AppContextHelper.GetStringValueClaim(HttpContext, JwtClaimTypes.Name),
                    SendDate = DateTime.UtcNow.AddHours(2)
                },
                Channels = await GetNotificationChannels(),
                SendNow = false
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [HandleExceptionFilter]
        public async Task<IActionResult> Create([Bind]BulkNotificationViewModel bulkNotificationViewModel)
        {
            BulkNotificationDTO bulkNotification = bulkNotificationViewModel.BulkNotification;

            if (bulkNotificationViewModel.SendNow == true)
            {
                bulkNotification.SendDate = DateTime.UtcNow.AddHours(2);
                SendBulkNotification(bulkNotification);
            }
            string url = $"{NotificationsApiUrl}BulkNotifications/Add";

            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var response = await HttpRequestFactory.Post(accessToken, url, bulkNotification);

            if (response.StatusCode == HttpStatusCode.Created)
            {
                AppContextHelper.SetToastMessage("Bulk notification escalation rule has been successfully created", MessageType.Success, 1, Response);
                return RedirectToAction(nameof(Index));
            }
            else
            {
                AppContextHelper.SetToastMessage("Failed to create bulk notification escalation rule", MessageType.Danger, 1, Response);
                ModelState.AddModelError("", HttpResponseHandler.Process(response));
            }

            return View(bulkNotification);
        }

        [HandleExceptionFilter]
        public async Task<IActionResult> Edit([FromQuery] int notificationId)
        {
            return View(new BulkNotificationViewModel
            {
                BulkNotification = await GetBulkNotification(notificationId),
                Channels = await GetNotificationChannels(),
                SendNow = false
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [HandleExceptionFilter]
        public async Task<IActionResult> Edit([Bind]BulkNotificationViewModel bulkNotificationViewModel)
        {
            BulkNotificationDTO bulkNotification = bulkNotificationViewModel.BulkNotification;

            if(bulkNotificationViewModel.SendNow == true)
            {
                bulkNotification.SendDate = DateTime.UtcNow.AddHours(2);
                SendBulkNotification(bulkNotification);
            }
            
            string url = $"{NotificationsApiUrl}BulkNotifications/Update";

            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var response = await HttpRequestFactory.Put(accessToken, url, bulkNotification);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                AppContextHelper.SetToastMessage("Bulk notification escalation rule has been successfully updated", MessageType.Success, 1, Response);
                return RedirectToAction(nameof(Index));
            }
            else
            {
                AppContextHelper.SetToastMessage("Failed to update bulk notification escalation rule", MessageType.Danger, 1, Response);
                ModelState.AddModelError("", HttpResponseHandler.Process(response));
            }
            return View(bulkNotification);
        }

        [HandleExceptionFilter]
        public async Task<IActionResult> Details([FromQuery] int notificationId)
        {
            return View(await GetBulkNotification(notificationId));
        }

        [HandleExceptionFilter]
        public async Task<IActionResult> Delete([FromQuery] int notificationId)
        {
            var BulkNotification = await GetBulkNotification(notificationId);
            return View(BulkNotification);
        }

        [HttpPost, ActionName("Delete")]
        [HandleExceptionFilter]
        public async Task<IActionResult> VerifyDelete(int notificationId)
        {
            string url = $"{NotificationsApiUrl}BulkNotifications/Delete?notificationId={notificationId}";
            var BulkNotification = new BulkNotificationDTO();

            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var response = await HttpRequestFactory.Delete(accessToken, url);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                AppContextHelper.SetToastMessage("Bulk notification escalation rule has been successfully deleted", MessageType.Success, 1, Response);
                return RedirectToAction(nameof(Index));
            }
            else
            {
                AppContextHelper.SetToastMessage("Failed to delete bulk notification escalation rule", MessageType.Danger, 1, Response);
                ModelState.AddModelError("", HttpResponseHandler.Process(response));
            }
            return View(await GetBulkNotification(notificationId));
        }

        private async Task<BulkNotificationResponse> GetBulkNotification(int notificationId)
        {
            string url = $"{NotificationsApiUrl}BulkNotifications/GetById?notificationId={notificationId}";
            var BulkNotification = new BulkNotificationResponse();

            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var response = await HttpRequestFactory.Get(accessToken, url);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                BulkNotification = response.ContentAsType<BulkNotificationResponse>();
            }
            else
            {
                ModelState.AddModelError("", HttpResponseHandler.Process(response));
            }
            return BulkNotification;
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

        public async void SendBulkNotification(BulkNotificationDTO bulkNotification)
        {
            switch (bulkNotification.ChannelId)
            {
                case 2:
                    //await SendSms();
                    break;
                case 4:
                    //SendEmails();
                    break;
                case 5:
                    //SendWhatsAppMessages();
                    break;
                default:
                    break;
            }
        }
    }
}