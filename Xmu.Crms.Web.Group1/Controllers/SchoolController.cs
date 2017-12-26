using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Threading.Tasks;
using Microsoft.ApplicationInsights.Extensibility.Implementation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Xmu.Crms.Shared.Models;
using Xmu.Crms.Shared.Service;

namespace Xmu.Crms.Controllers
{
    [Produces("application/json")]
    [Route("/api/school")]
    public class SchoolController : Controller
    {
        ISchoolService schoolService;
        public SchoolController(ISchoolService schoolService)
        {
            this.schoolService = schoolService;
        }
        // GET: api/School
        [HttpGet]
        public IActionResult Get([FromQuery]string cityName)
        {
            IList<School> schools = schoolService.ListSchoolByCity(cityName);
            return Json(schools);
        }

        // POST: api/School
        [HttpPost]
        public IActionResult Post([FromBody]dynamic json)
        {
            School school = new School
            {
                Name = json.Name,
                Province = json.Province,
                City = json.City
            };
            schoolService.InsertSchool(school);
            return Json(new { status = 200 });
        }
    }
}
