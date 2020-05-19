using AngleDimension.Standard.Http.HttpServices;
using CNPD.Core.ImportationEngine;
using IdentityModel;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Thandizo.DataModels.Notifications;
using Thandizo.DataModels.Notifications.Requests;
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
            return View(new BulkNotificationRequestViewModel
            {
                BulkNotificationRequest = new BulkNotificationRequest
                {
                    CreatedBy = AppContextHelper.GetStringValueClaim(HttpContext, JwtClaimTypes.Name),
                    SendDate = DateTime.UtcNow,
                    SendNow = false
                },
                Channels = await GetNotificationChannels()
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [HandleExceptionFilter]
        public async Task<IActionResult> Create(IFormFile formfile, BulkNotificationRequestViewModel bulkNotificationRequsestViewModel)
        {
            var numberList = new List<string>();
            if(formfile != null)
            {
                var phoneNumbersUploadedFile = formfile;
                               
                IWorkbook book;

                var filePath = Path.GetTempFileName();

                //using var stream = System.IO.File.Create(filePath);

                //read workbook as XLSX
                try
                {
                    book = new XSSFWorkbook(phoneNumbersUploadedFile.OpenReadStream());

                }
                catch (Exception exception)
                {
                    var e = exception;
                    book = null;
                }

                // If reading fails, try to read workbook as XLS:
                if (book == null)
                {
                    book = new HSSFWorkbook(phoneNumbersUploadedFile.OpenReadStream());
                }
                
                for (int i = 0; i < book.NumberOfSheets; i++)
                {
                    var currentSheet = book.GetSheetAt(i);
                    for (int row = 0; row <= currentSheet.LastRowNum; row++)
                    {
                        if (currentSheet.GetRow(row) != null) //null is when the row only contains empty cells 
                        {
                            var number = currentSheet.GetRow(row).GetCell(0).ToString();
                            //Clean up phone number
                            var sanitizedNumber = PhoneNumberSanitizer.Sanitize(currentSheet.GetRow(row).GetCell(0).ToString(), "+265");
                            numberList.Add(sanitizedNumber);
                        }
                    }
                }
            }


            if (!String.IsNullOrEmpty(bulkNotificationRequsestViewModel.BulkNotificationRequest.CustomNumbers))
            {
                var customNumbers = bulkNotificationRequsestViewModel.BulkNotificationRequest.CustomNumbers.Split(",");
                numberList.InsertRange(customNumbers.Length, customNumbers);
            }
            
            bulkNotificationRequsestViewModel.BulkNotificationRequest.UploadedPhoneNumbers = numberList;
            
            string url = $"{NotificationsApiUrl}BulkNotifications/Add";

            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var response = await HttpRequestFactory.Post(accessToken, url, bulkNotificationRequsestViewModel.BulkNotificationRequest);

            if (response.StatusCode == HttpStatusCode.Created)
            {
                AppContextHelper.SetToastMessage("Bulk notification has been successfully created", MessageType.Success, 1, Response);
                return RedirectToAction(nameof(Index));
            }
            else
            {
                AppContextHelper.SetToastMessage("Failed to create bulk notification", MessageType.Danger, 1, Response);
                ModelState.AddModelError("", HttpResponseHandler.Process(response));
            }

            bulkNotificationRequsestViewModel.Channels = await GetNotificationChannels();
            return View(bulkNotificationRequsestViewModel);
        }

        [HandleExceptionFilter]
        public async Task<IActionResult> Edit([FromQuery] int notificationId)
        {
            var bulkNotificationResponse = await GetBulkNotification(notificationId);
            return View(new BulkNotificationRequestViewModel
            {
                BulkNotificationRequest = new BulkNotificationRequest
                {
                    CreatedBy = AppContextHelper.GetStringValueClaim(HttpContext, JwtClaimTypes.Name),
                    Message = bulkNotificationResponse.Message,
                    NotificationId = bulkNotificationResponse.NotificationId,
                    SendDate = bulkNotificationResponse.SendDate
                },
                Channels = await GetNotificationChannels(),
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [HandleExceptionFilter]
        public async Task<IActionResult> Edit([Bind]BulkNotificationViewModel bulkNotificationViewModel)
        {
            BulkNotificationDTO bulkNotification = bulkNotificationViewModel.BulkNotification;

            string url = $"{NotificationsApiUrl}BulkNotifications/Update";

            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var response = await HttpRequestFactory.Put(accessToken, url, bulkNotification);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                AppContextHelper.SetToastMessage("Bulk notification  has been successfully updated", MessageType.Success, 1, Response);
                return RedirectToAction(nameof(Index));
            }
            else
            {
                AppContextHelper.SetToastMessage("Failed to update bulk notification ", MessageType.Danger, 1, Response);
                ModelState.AddModelError("", HttpResponseHandler.Process(response));
            }

            bulkNotificationViewModel.Channels = await GetNotificationChannels();
            return View(bulkNotificationViewModel);
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
                AppContextHelper.SetToastMessage("Bulk notification has been successfully deleted", MessageType.Success, 1, Response);
                return RedirectToAction(nameof(Index));
            }
            else
            {
                AppContextHelper.SetToastMessage("Failed to delete bulk notification", MessageType.Danger, 1, Response);
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
    }
}