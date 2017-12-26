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

namespace Xmu.Crms.Controllers
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
                List<Course> courses = new List<Course>();
                List<long> classid = new List<long>();
                foreach (var i in classes)
                {
                    var course = courseService.GetCourseByCourseId(i.Course.Id);
                    courses.Add(course);
                    classid.Add(i.Id);
                }
                return Json(new { courses = courses, classid = classid });
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
            var seminars = seminarService.ListSeminarByCourseId(id);
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
