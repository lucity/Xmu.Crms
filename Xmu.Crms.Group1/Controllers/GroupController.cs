using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Xmu.Crms.Shared.Service;

namespace Xmu.Crms.Group1.Controllers
{
    [Produces("application/json")]
    [Route("/api/group")]
    public class GroupController : Controller
    {
        private ISeminarGroupService seminarGroupService;
        private IFixGroupService fixGroupService;
        public GroupController(ISeminarGroupService seminarGroupService, IFixGroupService fixGroupService)
        {
            this.seminarGroupService = seminarGroupService;
            this.fixGroupService = fixGroupService;
        }
    }
}
