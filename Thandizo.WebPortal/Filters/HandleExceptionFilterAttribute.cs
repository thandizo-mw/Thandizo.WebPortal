using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Thandizo.WebPortal.Filters
{
    public class HandleExceptionFilterAttribute : ExceptionFilterAttribute
    {
        public override void OnException(ExceptionContext context)
        {
            context.ExceptionHandled = true;
            var descriptor = context.ActionDescriptor;
            var displayName = descriptor.DisplayName;

            var displayNameArray = displayName.Split(new char[] { '.', ' ' }, StringSplitOptions.RemoveEmptyEntries);
            int lastElement = displayNameArray.Length - 2;

            var viewName = displayNameArray[lastElement];

            var result = new ViewResult { ViewName = viewName };
            var metadataProvider = new EmptyModelMetadataProvider();

            if (context.Exception.Message == null)
            {
                result = new ViewResult { ViewName = "File" };
            }

            result.ViewData = new ViewDataDictionary(metadataProvider, context.ModelState);
            result.ViewData.Add("Exception", "true");
            result.ViewData.Add("ExceptionMessage", context.Exception.Message);
            context.Result = result;
        }
    }
}
