using System;
using System.Net;
using System.Net.Http;
using Thandizo.WebPortal.Helpers;

namespace Thandizo.WebPortal.Services
{
    public static class HttpResponseHandler
    {
        public static string Process(HttpResponseMessage httpResponse)
        {
            string friendlyMessage = "ResponceSystemError";
            var outputHandler = new OutputHandler();

            try
            {
                //try to convert the returned object to see it an output handler
                outputHandler = httpResponse.ContentAsType<OutputHandler>();
            }
            catch (Exception)
            {
                //it means it failed to convert to OutputHandler object, set to NULL
                outputHandler = null;
            }

            if (outputHandler != null)
            {
                if (!string.IsNullOrEmpty(outputHandler.Message))
                {
                    friendlyMessage = httpResponse.ContentAsType<OutputHandler>().Message;
                }
            }
            else //return type not OutputHandler
            {
                var parsedJsonData = JsonConvertor.ConvertObjectToMessages(httpResponse.Content.ReadAsStringAsync().Result).ToString();

                if (!parsedJsonData.Equals("ERROR"))//the json data was parsed and read successfully
                {
                    friendlyMessage = parsedJsonData;
                }
                else //provide generic and friendly responses to the client
                {
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
