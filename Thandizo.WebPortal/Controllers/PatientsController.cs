using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Thandizo.DataModels.Patients.Responses;
using Thandizo.WebPortal.Helpers;
using Thandizo.WebPortal.Helpers.General;
using Thandizo.WebPortal.Services;

namespace Thandizo.WebPortal.Controllers
{
    public class PatientsController : Controller
    {
        private readonly IConfiguration _configuration;
        IHttpRequestHandler _httpRequestHandler;

        public PatientsController(IConfiguration configuration, IHttpRequestHandler httpRequestHandler)
        {
            _configuration = configuration;
            _httpRequestHandler = httpRequestHandler;
        }

        public string PatientsApiUrl
        {
            get
            {
                return _configuration["PatientsApiUrl"];
            }
        }
        
        public async Task<IActionResult> ConfirmPatients()
        {
            string url = $"{PatientsApiUrl}Patients/GetAll";
            var patients = Enumerable.Empty<PatientResponse>();

            var response = await _httpRequestHandler.Get(url);
          
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

        public async Task<IActionResult> ConfirmPatient([FromQuery] long patientId)
        {
            string url = $"{PatientsApiUrl}Patients/GetById?patientId={patientId}";
            var patient = new PatientResponse();

            var response = await _httpRequestHandler.Get(url);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                patient = response.ContentAsType<PatientResponse>();
            }
            else
            {
                ModelState.AddModelError("", HttpResponseHandler.Process(response));

            }

            if(TempData["ModelError"] != null)
            {
                ModelState.AddModelError("", TempData["ModelError"].ToString());
            }

            return View(patient);
        }

        [HttpPost, ActionName("ConfirmPatient")]
        public async Task<IActionResult> VerifyConfirmPatient(long patientId)
        {
            string url = $"{PatientsApiUrl}Patients/ConfirmPatient?patientId={patientId}";
            var patient = new PatientResponse();

            var response = await _httpRequestHandler.Put(url, patientId);

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
    }
}