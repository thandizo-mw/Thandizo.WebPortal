using AngleDimension.Standard.Http.HttpServices;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Thandizo.DataModels.Patients;
using Thandizo.WebPortal.Filters;
using Thandizo.WebPortal.Helpers;
using Thandizo.WebPortal.Helpers.General;
using Thandizo.WebPortal.Services;
using IdentityModel;

namespace Thandizo.WebPortal.Controllers
{
    public class PatientSymptomsController : Controller
    {
        private readonly IConfiguration _configuration;
        

        public PatientSymptomsController(IConfiguration configuration)
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
            string url = $"{PatientsApiUrl}Symptoms/GetAll";
            var PatientSymptoms = Enumerable.Empty<PatientSymptomDTO>();

            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var response = await HttpRequestFactory.Get(accessToken, url);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                PatientSymptoms = response.ContentAsType<IEnumerable<PatientSymptomDTO>>();
            }
            else
            {
                ModelState.AddModelError("", HttpResponseHandler.Process(response));
            }
            return View(PatientSymptoms);
        }

        [HandleExceptionFilter]
        public IActionResult Create()
        {
            return View(new PatientSymptomDTO
            {
               CreatedBy = AppContextHelper.GetStringValueClaim(HttpContext, JwtClaimTypes.Name)
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [HandleExceptionFilter]
        public async Task<IActionResult> Create([Bind] PatientSymptomDTO patientSymptom)
        {
            string url = $"{PatientsApiUrl}Symptoms/Add";

            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var response = await HttpRequestFactory.Post(accessToken, url, patientSymptom);

            if (response.StatusCode == HttpStatusCode.Created)
            {
                AppContextHelper.SetToastMessage("Patient symptom has been successfully created", MessageType.Success, 1, Response);
                return RedirectToAction(nameof(Index));
            }
            else
            {
                AppContextHelper.SetToastMessage("Failed to create patient symptom", MessageType.Danger, 1, Response);
                ModelState.AddModelError("", HttpResponseHandler.Process(response));
            }

            return View(patientSymptom);
        }

        [HandleExceptionFilter]
        public async Task<IActionResult> Edit([FromQuery] int symptomId)
        {
            var patientSymptom = await GetPatientSymptom(symptomId);
            return View(patientSymptom);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [HandleExceptionFilter]
        public async Task<IActionResult> Edit([Bind]PatientSymptomDTO patientSymptom)
        {
            string url = $"{PatientsApiUrl}Symptoms/Update";

            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var response = await HttpRequestFactory.Put(accessToken, url,  patientSymptom);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                AppContextHelper.SetToastMessage("Patient symptom has been successfully updated", MessageType.Success, 1, Response);
                return RedirectToAction(nameof(Index));
            }
            else
            {
                AppContextHelper.SetToastMessage("Failed to update patient symptom", MessageType.Danger, 1, Response);
                ModelState.AddModelError("", HttpResponseHandler.Process(response));
            }
            return View(patientSymptom);
        }

        [HandleExceptionFilter]
        public async Task<IActionResult> Details([FromQuery] int symptomId)
        {
            var PatientSymptom = await GetPatientSymptom(symptomId);
            return View(PatientSymptom);
        }

        [HandleExceptionFilter]
        public async Task<IActionResult> Delete([FromQuery] int symptomId)
        {
            var PatientSymptom = await GetPatientSymptom(symptomId);
            return View(PatientSymptom);
        }

        [HttpPost, ActionName("Delete")]
        [HandleExceptionFilter]
        public async Task<IActionResult> VerifyDelete(int symptomId)
        {
            string url = $"{PatientsApiUrl}Symptoms/Delete?symptomId={symptomId}";
            var PatientSymptom = new PatientSymptomDTO();

            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var response = await HttpRequestFactory.Delete(accessToken, url); 

            if (response.StatusCode == HttpStatusCode.OK)
            {
                AppContextHelper.SetToastMessage("Patient symptom has been successfully deleted", MessageType.Success, 1, Response);
                return RedirectToAction(nameof(Index));
            }
            else
            {
                AppContextHelper.SetToastMessage("Failed to delete patient symptom", MessageType.Danger, 1, Response);
                TempData["ModelError"] = HttpResponseHandler.Process(response);
            }
            return RedirectToAction(nameof(Delete), new { symptomId });
        }

        private async Task<PatientSymptomDTO> GetPatientSymptom(int symptomId)
        {
            string url = $"{PatientsApiUrl}Symptoms/GetById?symptomId={symptomId}";
            var PatientSymptom = new PatientSymptomDTO();

            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var response = await HttpRequestFactory.Get(accessToken, url);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                PatientSymptom = response.ContentAsType<PatientSymptomDTO>();
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
            return PatientSymptom;
        }


        [HandleExceptionFilter]
        public async Task<IActionResult> GetPatientSymptoms()
        {
            string url = $"{PatientsApiUrl}Symptoms/GetAll";
            var PatientSymptoms = Enumerable.Empty<PatientSymptomDTO>();

            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var response = await HttpRequestFactory.Get(accessToken, url);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                PatientSymptoms = response.ContentAsType<IEnumerable<PatientSymptomDTO>>();
            }
            else
            {
                ModelState.AddModelError("", HttpResponseHandler.Process(response));
            }
            return PartialView(PatientSymptoms);
        }
    }
}