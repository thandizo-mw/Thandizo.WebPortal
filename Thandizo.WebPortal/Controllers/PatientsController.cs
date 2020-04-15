using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Thandizo.DataModels.Patients;
using Thandizo.DataModels.Patients.Responses;
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
        
        public async Task<IActionResult> IndexAsync()
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

        public async Task<IActionResult> ConfirmPatient([FromQuery] long pid)
        {
            string url = $"{PatientsApiUrl}/api/Patients/GetById?patientId={pid}";
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
            return View(patient);
        }
    }
}