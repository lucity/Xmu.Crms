using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Xmu.Crms.Shared.Exceptions;
using Xmu.Crms.Shared.Models;
using Xmu.Crms.Shared.Service;

namespace Xmu.Crms.Group1.Controllers
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
            var type = User.Claims.Single(c => c.Type == "type").Value;
            var classinfo = classService.GetClassByClassId(classid);
            if (type == "teacher")
            {
                var students = userService.ListUserByClassId(classid, "", "");
                return Json(new {classInfo=classinfo,list=students});
            }
            return Json(classinfo);
        }
        //根据班级id以及seminarid获取学生名单
        //GET api/class/{classId}
        [HttpGet("{classid}/attendance")]
        public IActionResult getAttendanceById(long classid, [FromQuery]bool showLate, [FromQuery]long seminarid)
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


        //根据classid和seminarid老师开始签到
        //POST api/class/{classId}/startclass
        [HttpPost("{classid}/startclass")]
        public IActionResult startClass(long classid,[FromBody]dynamic json)
        {
            Location location = new Location()
            {
                ClassInfo = classService.GetClassByClassId(classid),
                Latitude = json.latitude,
                Longitude=json.longitude,
                Seminar=json.seminarid,
                Status=1
            };
            classService.CallInRollById(location);
            return Json(new { status = 200 });
        }

        //根据classid和seminarid老师结束签到
        //POST api/class/{classId}/endclass
        [HttpPost("{classid}/endclass")]
        public IActionResult endClass(long classid, [FromBody]dynamic json)
        {
            classService.EndCallRollById(json.seminarid, classid);
            return Json(new { status = 200 });
        }

        //根据classid和seminarid获取当前签到情况
        //GET api/class/{classid}/getstatus
        [HttpGet("{classid}/getstatus")]
        public IActionResult getStatus(long classid,[FromBody]dynamic json)
        {
            try
            {
                Location location = classService.GetCallStatusById(json.seminarid, classid);
                if(location==null)
                    return Json(new { callstatus = 0 });
                return Json(new { callstatus = location.Status });
            }
            catch(SeminarNotFoundException e)
            {
                return Json(new { callstatus = 0 });
            }
        }
    }
}