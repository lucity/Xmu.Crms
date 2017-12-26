using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Xmu.Crms.Shared.Models;
using Xmu.Crms.Shared.Service;

namespace Xmu.Crms.Group1.Controllers
{
    [Produces("application/json")]
    [Route("/api/course")]
    public class CourseController : Controller
    {
        private ICourseService courseService;
        private IClassService classService;
        private ISeminarService seminarService;
        public CourseController(ICourseService courseService, IClassService classService, ISeminarService seminarService)
        {
            this.courseService = courseService;
            this.classService = classService;
            this.seminarService = seminarService;
        }
        //根据JWT获取与用户类型
        //根据类型获取相应课程信息
        // GET: api/Course
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet]
        public IActionResult GetCourseList()
        {
            var id = long.Parse(User.Claims.Single(c => c.Type == "id").Value);
            var type = User.Claims.Single(c => c.Type == "type").Value;
            if(type == "teacher")
            {
                var courses = courseService.ListCourseByUserId(id);
                return Json(courses);
            }
            else
            {
                var classes = classService.ListClassByUserId(id);
                var courses = "";
                foreach(var i in classes)
                {
                    var course = courseService.GetCourseByCourseId(i.Course.Id);
                    var temp =
                    new {
                        course = course,
                        classid = i.Id
                    };
                }
                return Json(courses);
            }
            
        }
        //根据课程id获取课程详细信息
        // GET: api/Course/5
        [HttpGet("{id}")]
        public IActionResult Get(long id)
        {
            var course = courseService.GetCourseByCourseId(id);
            return Json(course);
        }
        //根据课程id获取seminar信息
        // GET: api/Course/{id}/Seminar

        [HttpGet("{id}/seminar")]
        public IActionResult getSeminar(long id)
        {
            var type = User.Claims.Single(c => c.Type == "type").Value;
            var seminars = seminarService.ListSeminarByCourseId(id);
            if (type=="teacher")
            {
                foreach (var s in seminars)
                {
                    DateTime now = DateTime.Now;
                    TimeSpan d1 = s.StartTime - now;
                    TimeSpan d2 = s.EndTime - now;
                    if (d1.Days < 0 && d2.Days >= 0)
                        return Json(s);
                }
            }            
            return Json(seminars);
        }

        //根据courseId获取class信息
        //GET: api/course/{courseId}/class
        [HttpGet("{courseId}/class")]
        public IActionResult getClassByCourseId(int courseId)
        {
            var classes = classService.ListClassByCourseId(courseId);
            return Json(classes);
        }
    }
}
