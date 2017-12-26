using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Xmu.Crms.Shared.Models;
using Microsoft.AspNetCore.Authorization;
using Xmu.Crms.Shared.Service;
using System.Net.Http.Headers;
using System.IO;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Text.RegularExpressions;

namespace Xmu.Crms.Controllers
{
    [Produces("application/json")]
    [Route("/api/seminar")]
    public class SeminarController : Controller
    {
        private ISeminarGroupService seminarGroupService;
        private IFixGroupService fixGroupService;
        private ITopicService topicService;
        private ISeminarService seminarService;
        public SeminarController(ISeminarGroupService seminarGroupService, ITopicService topicService, ISeminarService seminarService, IFixGroupService fixGroupService)
        {
            this.seminarGroupService = seminarGroupService;
            this.fixGroupService = fixGroupService;
            this.topicService = topicService;
            this.seminarService = seminarService;
        }
        // GET: api/Seminar/5
        [HttpGet("{id}")]
        public IActionResult Get(long id)
        {
            var seminar = seminarService.GetSeminarBySeminarId(id);
            return Json(seminar);
        }
        // GET: api/Seminar/5/Group
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet("{id}/group")]
        public IActionResult GetGroup(long id, [FromQuery]bool isFixed,[FromQuery]bool gradeable, [FromQuery]long classid, [FromQuery]bool include)
        {
            var userId = long.Parse(User.Claims.Single(c => c.Type == "id").Value);
            if (gradeable)
            {
                var group = seminarGroupService.GetSeminarGroupById(id, userId);
                var groupTopics = topicService.ListSeminarGroupTopicByGroupId(group.Id);
                var allGroups = seminarGroupService.ListSeminarGroupBySeminarId(id);
                List<SeminarGroup> groups = allGroups.ToList<SeminarGroup>();
                List<SeminarGroupTopic> topics = new List<SeminarGroupTopic>();
                var sg = seminarGroupService.GetSeminarGroupByGroupId(groupTopics.First().SeminarGroup.Id);
                if (groupTopics.Count == 1)
                {
                    foreach(var g in allGroups)
                    {
                        var t = topicService.ListSeminarGroupTopicByGroupId(g.Id);
                        if(t.Count <= 0 || g.Id == group.Id || t.First().Id == groupTopics.First().Id)
                        {
                            continue;
                        }
                        else
                        {
                            var sg2 = seminarGroupService.GetSeminarGroupByGroupId(t.First().SeminarGroup.Id);
                            if (sg2.ClassInfo.Id != sg.ClassInfo.Id)
                                continue;
                        }
                        topics.Add(t.First());
                    }
                }
                else
                {
                    foreach (var g in allGroups)
                    {
                        if (g.Id != group.Id)
                        {
                            var t = topicService.ListSeminarGroupTopicByGroupId(g.Id);
                            if (t.Count <= 0)
                            {
                                continue;
                            }
                            var sg2 = seminarGroupService.GetSeminarGroupByGroupId(t.First().SeminarGroup.Id);
                            if (sg2.ClassInfo.Id != sg.ClassInfo.Id)
                                continue;
                            topics.Add(t.First());
                        }
                    }
                }
                return Json(topics.GroupBy(g => g.Topic));
            }
            if(classid != 0)
            {
                if (!isFixed)
                {
                    var t = seminarGroupService.ListSeminarGroupBySeminarId(id);
                    List<SeminarGroup> groups = new List<SeminarGroup>();
                    foreach(var g in t)
                    {
                        if (g.ClassInfo.Id == classid) groups.Add(g);
                    }
                    return Json(groups);
                }
                else
                {
                    var groups = fixGroupService.ListFixGroupByClassId(classid);
                    return Json(groups);
                }
            }
            if (include)
            {
                var group = seminarGroupService.GetSeminarGroupById(id, userId);
                var members = seminarGroupService.ListSeminarGroupMemberByGroupId(group.Id);
                return Json(new { group = group, members = members });
            }
            else
                return Json(new{ status="false" });
        }
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet("{id}/topic")]
        public IActionResult GetTopic(long id, [FromQuery]long classid)
        {
            var topics = topicService.ListTopicBySeminarId(id);
            List<int> left = new List<int>();
            foreach (var t in topics)
            {
                int i = topicService.GetRestTopicById(t.Id, classid);
                left.Add(i);
            }
            return Json(new { topics = topics, left = left });
        }
    }
}
