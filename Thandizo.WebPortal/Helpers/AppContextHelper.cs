using Microsoft.AspNetCore.Http;
using System;
using System.Linq;
using System.Security.Claims;
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

        public static string GetUsername(HttpContext context)
        {
            string username = string.Empty;
            ClaimsIdentity identity = context.User.Identity as ClaimsIdentity;
            var claim = identity.Claims.FirstOrDefault(x => x.Type.Equals("name"));
            if (claim != null)
            {
                username = claim.Value;
            }

            return username;
        }

        public static string GetStringValueClaim(HttpContext context, string claimType)
        {
            string claimValue = string.Empty;
            ClaimsIdentity identity = context.User.Identity as ClaimsIdentity;
            var claim = identity.Claims.FirstOrDefault(x => x.Type.Equals(claimType));
            if (claim != null)
            {
                claimValue = claim.Value;
            }

            return claimValue;
        }

        public static long GetNumberValueClaim(HttpContext context, string claimType)
        {
            string value = string.Empty;
            ClaimsIdentity identity = context.User.Identity as ClaimsIdentity;
            var claim = identity.Claims.FirstOrDefault(x => x.Type.Equals(claimType));
            if (claim != null)
            {
                value = claim.Value;
            }
            return Convert.ToInt64(string.IsNullOrWhiteSpace(value) ? 0.ToString() : value);
        }
    }
}
