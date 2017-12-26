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
        private ISeminarService seminarService;
        private ISeminarGroupService seminarGroupService;
        public ClassController(IClassService classService, IUserService userService,ISeminarService seminarService, ISeminarGroupService seminarGroupService)
        {
            this.classService = classService;
            this.userService = userService;
            this.seminarService = seminarService;
            this.seminarGroupService = seminarGroupService;
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
        //根据班级id以及seminarid获取已经签到学生名单
        //GET api/class/{classId}/attendance
        [HttpGet("{classid}/attendance")]
        public IActionResult getAttendanceById(long classid,[FromQuery]long seminarid)
        {           
                var students = userService.ListPresentStudent(seminarid, classid);
                var latestudents = userService.ListLateStudent(seminarid, classid);
                return Json(new { present = students, late = latestudents });
        }

        

        //根据classid和seminarid老师开始签到
        //POST api/class/{classId}/startclass
        [HttpGet("{classid}/startclass")]
        public IActionResult startClass(long classid,[FromQuery]long seminarid, [FromQuery]decimal latitude, [FromQuery]decimal longitude)
        {
            Location location = new Location()
            {
                ClassInfo = classService.GetClassByClassId(classid),
                Latitude = latitude,
                Longitude=longitude,
                Seminar=seminarService.GetSeminarBySeminarId(seminarid),
                Status=1
            };
            classService.CallInRollById(location);
            return Json(new { status = 200 });
        }

        //根据classid和seminarid老师结束签到
        //POST api/class/{classId}/endclass
        [HttpGet("{classid}/endclass")]
        public IActionResult endClass(long classid,[FromQuery]long seminarid)
        {
            classService.EndCallRollById(seminarid, classid);
            seminarGroupService.AutomaticallyGrouping(seminarid, classid);
            return Json(new { status = 200 });
        }

        //根据classid和seminarid获取当前签到情况
        //GET api/class/{classid}/getstatus
        [HttpGet("{classid}/getstatus")]
        public IActionResult getStatus(long classid, [FromQuery]long seminarid)
        {
            try
            {
                Location location = classService.GetCallStatusById(seminarid,classid);
                if(location==null)
                    return Json(new { callstatus = 2 });//2代表没有这个记录0是签到结束1是正在签到
                return Json(new { callstatus = location.Status });
            }
            catch(Exception e)
            {
                return Json(new { callstatus = 2 });//2代表没有这个记录0是签到结束1是正在签到
            }
        }
    }
}