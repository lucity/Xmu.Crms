 using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Xmu.Crms.Group1.Controllers
{
    public class HomeController : Controller
    {
        private IHostingEnvironment hostingEnvironment;
        public HomeController(IHostingEnvironment env)
        {
            this.hostingEnvironment = env;
        }
        // GET: Home
        public IActionResult ChooseCharacter()
        {
            return View();
        }
        public IActionResult ChooseSchool()
        {
            return View();
        }
        public IActionResult LoginUI()
        {
            return View();
        }
        public IActionResult RegisterUI()
        {
            return View();
        }
    }
}