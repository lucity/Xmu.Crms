using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Xmu.Crms.Group1.Controllers
{
    public class TeacherController : Controller
    {
        // GET: Teacher
        public IActionResult ClassManage()
        {
            return View();
        }
        public IActionResult TeacherMainUI()
        {
            return View();
        }
        public IActionResult CreateSchool()
        {
            return View();
        }
        public IActionResult CheckTeacherInfoUI()
        {
            ViewData["Path"] = " ";
            return View();
        }
        [HttpPost]
        public IActionResult CheckTeacherInfoUI(IList<IFormFile> files)
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
        public IActionResult FixedGroupInfoUI()
        {
            return View();
        }
        public IActionResult RandomGroupInfoUI()
        {
            return View();
        }
        public IActionResult RollCallListUI()
        {
            return View();
        }
        public IActionResult FixedRollCallUI()
        {
            return View();
        }
        public IActionResult RandomRollCallUI()
        {
            return View();
        }
        public IActionResult TeacherBindingUI()
        {
            return View();
        }
    }
}