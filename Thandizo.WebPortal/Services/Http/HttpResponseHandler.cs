using AngleDimension.Standard.Http.HttpServices;
using System;
using System.Net;
using System.Net.Http;
using Thandizo.DataModels.General;

namespace Thandizo.WebPortal.Services
{
    public static class HttpResponseHandler
    {
        public static string Process(HttpResponseMessage httpResponse)
        {
            string friendlyMessage = "ResponceSystemError";
            var outputResponse = new OutputResponse();

            try
            {
                //try to convert the returned object to see it an output handler
                outputResponse = httpResponse.ContentAsType<OutputResponse>();
            }
            catch (Exception)
            {
                //it means it failed to convert to OutputResponse object, set to NULL
                outputResponse = null;
            }

            if (outputResponse != null)
            {
                if (!string.IsNullOrEmpty(outputResponse.Message))
                {
                    friendlyMessage = httpResponse.ContentAsType<OutputResponse>().Message;
                }
            }
            else //return type not OutputResponse
            {
                try
                {
                    var response = httpResponse.Content.ReadAsStringAsync().Result.ToString();
                    friendlyMessage = response.Replace("\"", "");
                }
                catch (Exception)               
                {
                    //provide generic and friendly responses to the client
                    switch (httpResponse.StatusCode)
                    {
                        case HttpStatusCode.BadGateway:
                            friendlyMessage = "BadGatewayError";
                            break;
                        case HttpStatusCode.Unauthorized:
                            friendlyMessage = "UnauthorizedError";
                            break;
                        case HttpStatusCode.NotFound:
                            friendlyMessage = "NotFoundError";
                            break;
                        case HttpStatusCode.Accepted:
                            friendlyMessage = "AcceptedResponse";
                            break;
                        case HttpStatusCode.Forbidden:
                            friendlyMessage = "ForbiddenError";
                            break;
                        case HttpStatusCode.InternalServerError:
                            friendlyMessage = "InternalServerError";
                            break;
                        case HttpStatusCode.ServiceUnavailable:
                            friendlyMessage = "ServiceUnavailable";
                            break;
                        case HttpStatusCode.BadRequest:
                            friendlyMessage = "BadRequest";
                            break;
                    }
                }
            }
            return friendlyMessage;
        }
    }
}
