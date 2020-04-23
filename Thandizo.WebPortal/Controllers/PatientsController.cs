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
using Thandizo.DataModels.Patients.Responses;
using Thandizo.DataModels.ViewModels.Patients;
using Thandizo.WebPortal.Filters;
using Thandizo.WebPortal.Helpers;
using Thandizo.WebPortal.Helpers.General;
using Thandizo.WebPortal.Services;

namespace Thandizo.WebPortal.Controllers
{
    public class PatientsController : Controller
    {
        private readonly IConfiguration _configuration;

        public PatientsController(IConfiguration configuration)
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
        public string CoreApiUrl
        {
            get
            {
                return _configuration["CoreApiUrl"];
            }
        }

        [HandleExceptionFilter]
        public async Task<IActionResult> ConfirmPatients()
        {
            string valuesFilter = "false";
            string phoneNumber = HttpContext.User.Identity.Name;
            string url = $"{PatientsApiUrl}GetByResponseTeamMember?phoneNumber={phoneNumber}&valuesFilter={valuesFilter}";
            var patients = Enumerable.Empty<PatientResponse>();

            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var response = await HttpRequestFactory.Get(accessToken, url);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                patients = response.ContentAsType<IEnumerable<PatientResponse>>();
            }
            else
            {
                ModelState.AddModelError("", HttpResponseHandler.Process(response));
            }

            return View(patients);
        }

        [HandleExceptionFilter]
        public async Task<IActionResult> ConfirmPatient([FromQuery] long patientId)
        {
            string url = $"{PatientsApiUrl}GetById?patientId={patientId}";
            var patient = new PatientResponse();

            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var response = await HttpRequestFactory.Get(accessToken, url);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                patient = response.ContentAsType<PatientResponse>();
            }
            else
            {
                ModelState.AddModelError("", HttpResponseHandler.Process(response));

            }

            if (TempData["ModelError"] != null)
            {
                ModelState.AddModelError("", TempData["ModelError"].ToString());
            }

            return View(patient);
        }

        [HttpPost, ActionName("ConfirmPatient")]
        [ValidateAntiForgeryToken]
        [HandleExceptionFilter]
        public async Task<IActionResult> VerifyConfirmPatient(long patientId)
        {
            string url = $"{PatientsApiUrl}ConfirmPatient?patientId={patientId}";
            var patient = new PatientResponse();

            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var response = await HttpRequestFactory.Put(accessToken, url,  patientId);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                AppContextHelper.SetToastMessage("Patient has been successfully confirmed", MessageType.Success, 1, Response);
                return RedirectToAction(nameof(ConfirmPatients));
            }
            else
            {
                AppContextHelper.SetToastMessage("Failed to confirm patient", MessageType.Danger, 1, Response);
                TempData["ModelError"] = HttpResponseHandler.Process(response);
            }
            return RedirectToAction(nameof(ConfirmPatient), new { patientId });
        }

        [HandleExceptionFilter]
        public IActionResult Create()
        {
            return View(new PatientResponse
            {
                CreatedBy = HttpContext.User.Identity.Name
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [HandleExceptionFilter]
        public async Task<IActionResult> Create([Bind] PatientResponse patient)
        {
            return View(patient);
        }

        [HandleExceptionFilter]
        public async Task<IActionResult> Edit([FromQuery] long patientId)
        {
            return View(new PatientResponseViewModel
            {
                PatientResponse = await GetPatient(patientId),
                PatientStatuses = await GetPatientStatuses(),
                IdentificationTypes = await GetIdentificationTypes(),
                TransmissionClassifications = await GetClassifications()
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [HandleExceptionFilter]
        public async Task<IActionResult> Edit([Bind]PatientResponseViewModel patientResponseViewModel)
        {
            PatientDTO patient = patientResponseViewModel.PatientResponse;
            string url = $"{PatientsApiUrl}Update";

            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var response = await HttpRequestFactory.Put(accessToken, url,  patient);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                AppContextHelper.SetToastMessage("Patient has been successfully updated", MessageType.Success, 1, Response);
                return RedirectToAction(nameof(ConfirmPatients));
            }
            else
            {
                AppContextHelper.SetToastMessage("Failed to update patient", MessageType.Danger, 1, Response);
                ModelState.AddModelError("", HttpResponseHandler.Process(response));
            }

            patientResponseViewModel.PatientStatuses = await GetPatientStatuses();
            patientResponseViewModel.IdentificationTypes = await GetIdentificationTypes();
            patientResponseViewModel.TransmissionClassifications = await GetClassifications();
            return View(patientResponseViewModel);
        }

        [HandleExceptionFilter]
        public async Task<IActionResult> Details([FromQuery] long patientId)
        {
            return View(new PatientResponseViewModel
            {
               PatientResponse = await GetPatient(patientId),
               PatientStatuses = await GetPatientStatuses(),
               IdentificationTypes = await GetIdentificationTypes(),
               TransmissionClassifications = await GetClassifications()
            });
        }

        private async Task<PatientResponse> GetPatient(long patientId)
        {
            string url = $"{PatientsApiUrl}GetById?patientId={patientId}";
            var Patient = new PatientResponse();

            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var response = await HttpRequestFactory.Get(accessToken, url);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                Patient = response.ContentAsType<PatientResponse>();
            }
            else
            {
                ModelState.AddModelError("", HttpResponseHandler.Process(response));
            }
            if (TempData["ModelError"] != null)
            {
                ModelState.AddModelError("", TempData["ModelError"].ToString());
                TempData["ModelError"] = null;
            }
            return Patient;
        }

        public async Task<IEnumerable<PatientStatusDTO>> GetPatientStatuses()
        {
            string url = $"{PatientsApiUrl}PatientStatuses/GetAll";
            var patientStatuses = Enumerable.Empty<PatientStatusDTO>();

            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var response = await HttpRequestFactory.Get(accessToken, url);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                patientStatuses = response.ContentAsType<IEnumerable<PatientStatusDTO>>();
            }
            else
            {
                ModelState.AddModelError("", HttpResponseHandler.Process(response));
            }
            return (patientStatuses);
        }

        public async Task<IEnumerable<IdentificationTypeDTO>> GetIdentificationTypes()
        {
            string url = $"{CoreApiUrl}IdentificationTypes/GetAll";
            var identificationTypes = Enumerable.Empty<IdentificationTypeDTO>();

            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var response = await HttpRequestFactory.Get(accessToken, url);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                identificationTypes = response.ContentAsType<IEnumerable<IdentificationTypeDTO>>();
            }
            else
            {
                ModelState.AddModelError("", HttpResponseHandler.Process(response));
            }
            return (identificationTypes);
        }

        public async Task<IEnumerable<TransmissionClassificationDTO>> GetClassifications()
        {
            string url = $"{PatientsApiUrl}TransmissionClassifications/GetAll";
            var transmissionClassifications = Enumerable.Empty<TransmissionClassificationDTO>();

            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var response = await HttpRequestFactory.Get(accessToken, url);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                transmissionClassifications = response.ContentAsType<IEnumerable<TransmissionClassificationDTO>>();
            }
            else
            {
                ModelState.AddModelError("", HttpResponseHandler.Process(response));
            }
            return (transmissionClassifications);
        }
    }
}