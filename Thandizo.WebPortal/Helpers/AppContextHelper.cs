using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Thandizo.WebPortal.Helpers.General;

namespace Thandizo.WebPortal.Helpers
{
    public class AppContextHelper
    {
        public static readonly string ToastMessageCookie = "toast_message";

        public static readonly string ToastMessageTypeCookie = "toast_message_type";

        public static void SetToastMessage(string message, MessageType messageType, int? expireTime, HttpResponse httpResponse)
        {
            CookieOptions option = new CookieOptions();
            if (expireTime.HasValue)
                option.Expires = DateTime.Now.AddMinutes(expireTime.Value);
            else
                option.Expires = DateTime.Now.AddMilliseconds(10);
            httpResponse.Cookies.Append(ToastMessageCookie, message, option);
            httpResponse.Cookies.Append(ToastMessageTypeCookie, messageType.ToString(), option);
        }

        public static void RemoveToastMessage(HttpResponse httpResponse)
        {
            httpResponse.Cookies.Delete(ToastMessageCookie);
            httpResponse.Cookies.Delete(ToastMessageTypeCookie);
        }
    }
}
