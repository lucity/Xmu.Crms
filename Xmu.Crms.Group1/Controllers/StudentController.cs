using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Xmu.Crms.Shared.Service;

namespace Xmu.Crms.Group1.Controllers
{
    public class StudentController : Controller
    {
        private IUserService userService;
        private IHostingEnvironment hostingEnvironment;
        public StudentController(IUserService userService, IHostingEnvironment env)
        {
                        this.userService = userService;
            this.hostingEnvironment = env;
        }
        // GET: Class
        public IActionResult StudentRollCallUI()
        {
            return View();
        }
        public IActionResult CheckStudentInfoUI()
        {
            ViewData["Path"] = " ";
            return View();
        }
        [HttpPost]
        public IActionResult CheckStudentInfoUI(IList<IFormFile> files)
        {
            long size = 0;
            foreach (var file in files)
            {
                var filename = ContentDispositionHeaderValue
                        .Parse(file.ContentDisposition)
                        .FileName
                        .Trim('"');
                ViewData["Path"] = filename;
                //这个hostingEnv.WebRootPath就是要存的地址可以改下
                filename = "D://es//ManagementSystem//Xmu.Crms.Web.Group1//wwwroot//images//" + $@"\{filename}";
                size += file.Length;
                using (FileStream fs = System.IO.File.Create(filename))
                {
                    file.CopyTo(fs);
                    fs.Flush();
                }
            }
            return View();
        }
        public IActionResult RandomGroupChooseTopicUI()
        {
            return View();
        }
        public IActionResult CourseUI()
        {
            return View();
        }
        public IActionResult CourseInfoUI()
        {
            return View();
        }
        public IActionResult Seminar()
        {
            return View();
        }
        public IActionResult Score()
        {
            return View();
        }
        public IActionResult RandomGroupList()
        {
            return View();
        }
        public IActionResult FixedGroupList()
        {
            return View();
        }
        public IActionResult StudentMainUI()
        {
            return View();
        }
        public IActionResult StudentBindingUI()
        {
            return View();
        }
    }
}