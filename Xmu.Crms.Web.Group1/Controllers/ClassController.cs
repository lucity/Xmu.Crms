using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Xmu.Crms.Shared.Service;

namespace Xmu.Crms.Controllers
{
    [Produces("application/json")]
    [Route("/api/class")]
    public class ClassController : Controller
    {
        private IClassService classService;
        private IUserService userService;
        public ClassController(IClassService classService, IUserService userService)
        {
            this.classService = classService;
            this.userService = userService;
        }
        //根据班级id获取班级信息
        //GET api/class/{classId}
        [HttpGet("{classid}")]
        public IActionResult getClassById(long classid)
        {
            var c = classService.GetClassByClassId(classid);
            return Json(c);
        }
        //根据班级id以及seminarid获取学生名单
        //GET api/class/{classId}
        [HttpGet("{classid}/attendance")]
        public JsonResult getAttendanceById(long classid, [FromQuery]bool showLate, [FromQuery]long seminarid)
        {
            if (showLate)
            {
                var students = userService.ListLateStudent(seminarid, classid);
                return Json(students);
            }
            else
            {
                var students = userService.ListPresentStudent(seminarid, classid);
                return Json(students);
            }
        }
    }
}