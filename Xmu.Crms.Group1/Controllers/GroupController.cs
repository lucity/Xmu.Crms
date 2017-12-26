using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Xmu.Crms.Shared.Service;

namespace Xmu.Crms.Group1.Controllers
{
    [Produces("application/json")]
    [Route("/api/group")]
    public class GroupController : Controller
    {
        private IGradeService gradeService;
        private ISeminarGroupService seminarGroupService;
        private ITopicService topicService;
        public GroupController(IGradeService gradeService, ISeminarGroupService seminarGroupService, ITopicService topicService)
        {
            this.gradeService = gradeService;
            this.seminarGroupService = seminarGroupService;
            this.topicService = topicService;
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet("{id}/grade/presentation")]
        public IActionResult PostGrade(long id, [FromQuery]long topicId, [FromQuery]int grade)
        {
            var userId = long.Parse(User.Claims.Single(c => c.Type == "id").Value);
            gradeService.InsertGroupGradeByUserId(topicId, userId, id, grade);
            return Json(new { status = 200 });
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet("{id}")]
        public IActionResult Get(long id)
        {
            var userId = long.Parse(User.Claims.Single(c => c.Type == "id").Value);
            var group = topicService.ListSeminarGroupTopicByGroupId(id);
            return Json(group);
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet("{id}/leader")]
        public IActionResult GetLeader(long id)
        {
            var userId = long.Parse(User.Claims.Single(c => c.Type == "id").Value);
            var leader = seminarGroupService.GetSeminarGroupLeaderByGroupId(id);
            if (leader == userId) return Json(new { isLeader = true });
            return Json(new { isLeader = false });
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet("{id}/resign")]
        public IActionResult Resign(long id)
        {
            var userId = long.Parse(User.Claims.Single(c => c.Type == "id").Value);
            seminarGroupService.ResignLeaderById(id, userId);
            return Json(new { status = 200 });
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet("{id}/assign")]
        public IActionResult Assign(long id)
        {
            var userId = long.Parse(User.Claims.Single(c => c.Type == "id").Value);
            if (seminarGroupService.GetSeminarGroupLeaderByGroupId(id) == -1)
            {
                seminarGroupService.AssignLeaderById(id, userId);
                return Json(new { isLeader = true });
            }
            return Json(new { isLeader = false });
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet("{id}/topic")]
        public IActionResult Assign(long id, [FromQuery]long topicId)
        {
            var userId = long.Parse(User.Claims.Single(c => c.Type == "id").Value);
            var group = seminarGroupService.GetSeminarGroupByGroupId(id);
            if (topicService.GetRestTopicById(topicId, group.ClassInfo.Id) == 0)
                return Json(new { isChoose = false });
            seminarGroupService.InsertTopicByGroupId(id, topicId);
            return Json(new { isChoose = true });
        }
    }
}
