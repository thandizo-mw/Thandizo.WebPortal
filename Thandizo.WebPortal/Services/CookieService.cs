using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using NETCore.Encrypt;
using Thandizo.WebPortal.Models;

namespace Thandizo.WebPortal.Services
{
    public class CookieService : ICookieService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        public string SecretVectorKey
        {
            get
            {
                return "DbtyzhN8h0Tdit21";
            }
        }

        public string SecretKey
        {
            get
            {
                return "DbtyzhN8h0Tdit21tf6fHiyEsKaxulKa";
            }
        }

        public CookieService()
        {
            _httpContextAccessor = new HttpContextAccessor();
        }

        public string Get(string friendlyName)
        {
            var mappedCookie = GetMappedName(friendlyName);
            if (mappedCookie == null)
            {
                throw new ArgumentException("Friendly name provided does not exist");
            }

            string value = _httpContextAccessor.HttpContext.Request.Cookies[mappedCookie.CookieKey];
            value = value == null ? string.Empty : EncryptProvider.AESDecrypt(value, SecretKey, SecretVectorKey);
            return value;
        }

        public void Add(string friendlyName, string value)
        {
            var mappedCookie = GetMappedName(friendlyName);
            if (mappedCookie == null)
            {
                throw new ArgumentException("Friendly name provided does not exist");
            }

            value = EncryptProvider.AESEncrypt(value, SecretKey, SecretVectorKey);
            _httpContextAccessor.HttpContext.Response.Cookies.Append(mappedCookie.CookieKey, value);
        }

        public void Delete(string friendlyName)
        {
            var mappedCookie = GetMappedName(friendlyName);
            if (mappedCookie == null)
            {
                throw new ArgumentException("Friendly name provided does not exist");
            }

            _httpContextAccessor.HttpContext.Response.Cookies.Delete(mappedCookie.CookieKey);
        }

        private MappedCookie GetMappedName(string friendlyName)
        {
            var mappedCookies = new List<MappedCookie>()
            {
                 new MappedCookie { CookieKey = "Thandizo.props.5200100", FriendlyName = "PhoneNumber" },
                 new MappedCookie { CookieKey = "Thandizo.props.5200200", FriendlyName = "Password" },
                 new MappedCookie { CookieKey = "Thandizo.props.5200300", FriendlyName = "UserId" },
                 new MappedCookie { CookieKey = "Thandizo.props.5200400", FriendlyName = "AccessToken" },
                 new MappedCookie { CookieKey = "Thandizo.props.7300100", FriendlyName = "UserName" }
            };

            var mappedCookie = mappedCookies.FirstOrDefault(x => x.FriendlyName.ToLower().Equals(friendlyName.ToLower()));
            return mappedCookie;
        }
    }
}
