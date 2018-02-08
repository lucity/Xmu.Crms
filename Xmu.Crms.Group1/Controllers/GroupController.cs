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

        //构造函数
        public GroupController(IGradeService gradeService, ISeminarGroupService seminarGroupService, ITopicService topicService)
        {
            this.gradeService = gradeService;
            this.seminarGroupService = seminarGroupService;
            this.topicService = topicService;
        }

        //上传学生打分
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet("{id}/grade/presentation")]
        public IActionResult PostGrade(long id, [FromQuery]long topicId, [FromQuery]int grade)
        {
            var userId = long.Parse(User.Claims.Single(c => c.Type == "id").Value);
            gradeService.InsertGroupGradeByUserId(topicId, userId, id, grade);
            return Json(new { status = 200 });
        }

        //通过Groupid获取所有的seminar
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet("{id}")]
        public IActionResult Get(long id)
        {
            var userId = long.Parse(User.Claims.Single(c => c.Type == "id").Value);
            var group = topicService.ListSeminarGroupTopicByGroupId(id);
            return Json(group);
        }

        //获取组长
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet("{id}/leader")]
        public IActionResult GetLeader(long id)
        {
            var userId = long.Parse(User.Claims.Single(c => c.Type == "id").Value);
            var leader = seminarGroupService.GetSeminarGroupLeaderByGroupId(id);
            if (leader == userId) return Json(new { isLeader = true });
            return Json(new { isLeader = false });
        }

        //组长辞职
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet("{id}/resign")]
        public IActionResult Resign(long id)
        {
            var userId = long.Parse(User.Claims.Single(c => c.Type == "id").Value);
            seminarGroupService.ResignLeaderById(id, userId);
            return Json(new { status = 200 });
        }

        //申请当组长
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

        //选话题
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet("{id}/topic")]
        public IActionResult Choosetopic(long id, [FromQuery]long topicId)
        {
            var userId = long.Parse(User.Claims.Single(c => c.Type == "id").Value);
            var group = seminarGroupService.GetSeminarGroupByGroupId(id);
            if (topicService.GetRestTopicById(topicId, group.ClassInfo.Id) == 0)
                return Json(new { isChoose = false });
            seminarGroupService.InsertTopicByGroupId(id, topicId);
            return Json(new { isChoose = true });
        }

        //通过seminarid和calssId获取所有小组
        [HttpGet("all")]
        public IActionResult GetAllGroup([FromQuery]long seminarId, [FromQuery]long classId)
        {
            var group = seminarGroupService.ListSeminarGroupBySeminarId(seminarId);
            List<long> list = new List<long>();
            foreach (var g in group)
            {
                if (g.ClassInfo.Id == classId)
                    list.Add(g.Id);
            }
            return Json(list);
        }

        //通过groupid获得小组
        [HttpGet("getgroup")]
        public IActionResult GetGroup([FromQuery]long groupid)
        {
            var group = seminarGroupService.ListSeminarGroupMemberByGroupId(groupid);
            return Json(new { members = group });
        }

        //将id为userid的同学加入id为groupid的小组
        [HttpGet("add")]
        public IActionResult addStudent([FromQuery]long groupid, [FromQuery]long userid)
        {
            seminarGroupService.InsertSeminarGroupMemberById(userid, groupid);
            return Json(new { status = "200" });
        }

        //将id为userid的同学从id为groupid的小组中删除
        [HttpGet("delete")]
        public IActionResult deleteStudent([FromQuery]long groupid, [FromQuery]long userid)
        {
            seminarGroupService.DeleteSeminarGroupMemberById(userid, groupid);
            return Json(new { status = "200" });
        }
    }
}
