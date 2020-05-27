using AngleDimension.Standard.Http.HttpServices;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Thandizo.DataModels.Core;
using Thandizo.DataModels.Patients;
using Thandizo.WebPortal.Filters;
using Thandizo.WebPortal.Helpers;
using Thandizo.WebPortal.Helpers.General;
using Thandizo.WebPortal.Services;
using IdentityModel;
using System;
using Thandizo.DataModels.Patients.Responses;
using Thandizo.DataModels.Statistics;

namespace Thandizo.WebPortal.Controllers
{
    public class PatientDailyStatusesController : Controller
    {
        private readonly IConfiguration _configuration;

        public PatientDailyStatusesController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string PatientsApiUrl
        {
            get
            {
                return _configuration["PatientsApiUrl"];
            }
        }

        [HandleExceptionFilter]
        public async Task<IActionResult> Index()
        {
            string url = $"{PatientsApiUrl}GetPatientsByDate?fromSubmissionDate={DateTime.UtcNow.Date}&toSubmissionDate={DateTime.UtcNow}";
            var Patients = Enumerable.Empty<PatientDTO>();

            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var response = await HttpRequestFactory.Get(accessToken, url);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                Patients = response.ContentAsType<IEnumerable<PatientDTO>>();
            }
            else
            {
                ModelState.AddModelError("", HttpResponseHandler.Process(response));
            }
            return View(Patients);
        }
        


        [HandleExceptionFilter]
        public async Task<IActionResult> Details([FromQuery] long patientId)
        {
            var PatientDailyStatuses = await GetPatientDailyStatuses(patientId);
            return View(PatientDailyStatuses);
        }


        public async Task<IActionResult> PendingStatusSubmission()
        {
            string url = $"{PatientsApiUrl}GetUnSubmittedPatientsByDate?fromSubmissionDate={DateTime.UtcNow.Date}&toSubmissionDate={DateTime.UtcNow}";
            var patients = Enumerable.Empty<PatientDTO>();

            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var response = await HttpRequestFactory.Get(accessToken, url);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                patients = response.ContentAsType<IEnumerable<PatientDTO>>();
            }
            else
            {
                ModelState.AddModelError("", HttpResponseHandler.Process(response));
            }
            return View(patients);
        }

        [HandleExceptionFilter]
        public async Task<JsonResult> GetPatientsByDate(DateTime fromDate, DateTime toDate)
        {
            string url = $"{PatientsApiUrl}GetPatientsByDate?fromSubmissionDate={fromDate}&toSubmissionDate={toDate}";
            var patients = Enumerable.Empty<PatientDTO>();

            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var response = await HttpRequestFactory.Get(accessToken, url);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                patients = response.ContentAsType<IEnumerable<PatientDTO>>();
                return Json(patients);
            }
            else
            {
                if (response.StatusCode != HttpStatusCode.NoContent)
                {
                    return Json(HttpResponseHandler.Process(response));
                }
                else
                {
                    return Json(patients);
                }
            }
        }
        
        [HandleExceptionFilter]
        public async Task<JsonResult> GetSymptomStatisticsByDate(DateTime fromDate, DateTime toDate)
        {
            string url = $"{PatientsApiUrl}PatientDailyStatuses/GetSymptomStatisticsByDate?fromSubmissionDate={fromDate}&toSubmissionDate={toDate}";
            var symptoms = Enumerable.Empty<SymptomStatisticsDTO>();

            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var response = await HttpRequestFactory.Get(accessToken, url);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                symptoms = response.ContentAsType<IEnumerable<SymptomStatisticsDTO>>();
                return Json(symptoms);
            }
            else
            {
                if (response.StatusCode != HttpStatusCode.NoContent)
                {
                    return Json(HttpResponseHandler.Process(response));
                }
                else
                {
                    return Json(symptoms);
                }
            }
        }
        
        [HandleExceptionFilter]
        public async Task<JsonResult> GetPatientSymptomStatsByDate(DateTime fromDate, DateTime toDate)
        {
            string url = $"{PatientsApiUrl}PatientDailyStatuses/GetPatientSymptomStatsByDate?fromSubmissionDate={fromDate}&toSubmissionDate={toDate}";
            var patientSymptoms = Enumerable.Empty<PatientDTO>();

            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var response = await HttpRequestFactory.Get(accessToken, url);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                patientSymptoms = response.ContentAsType<IEnumerable<PatientDTO>>();
                return Json(patientSymptoms);
            }
            else
            {
                if (response.StatusCode != HttpStatusCode.NoContent)
                {
                    return Json(HttpResponseHandler.Process(response));
                }
                else
                {
                    return Json(patientSymptoms);
                }
            }
        }

        [HandleExceptionFilter]
        public async Task<IEnumerable<PatientDailyStatusResponse>> GetPatientDailyStatuses(long patientId)
        {
            string url = $"{PatientsApiUrl}PatientDailyStatuses/GetByPatient?patientId={patientId}";
            var PatientDailyStatuses = Enumerable.Empty<PatientDailyStatusResponse>();

            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var response = await HttpRequestFactory.Get(accessToken, url);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                PatientDailyStatuses = response.ContentAsType<IEnumerable<PatientDailyStatusResponse>>();
            }
            else
            {
                ModelState.AddModelError("", HttpResponseHandler.Process(response));
            }
            return PatientDailyStatuses;
        }
    }
}