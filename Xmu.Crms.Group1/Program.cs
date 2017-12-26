using System.Reflection;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Xmu.Crms.Shared;

namespace Xmu.Crms.Group1
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = BuildWebHost(args);
            host.Run();
        }
        
        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseIISIntegration()
                .UseStartup<Startup>()
                .ConfigureServices(services => services.AddViceVersaCourseService().AddViceVersaCourseDao().AddViceVersaGradeDao().AddViceVersaClassDao().AddViceVersaGradeService().AddViceVersaClassService().AddInsomniaLoginService().AddInsomniaFixedGroupService().AddInsomniaSeminarGroupService().AddGroup1UserService().AddHignGradeSeminarService().AddGroup1UserDao().AddGroup1TopicService().AddGroup1TopicDao().AddGroup1SchoolService().AddGroup1SchoolDao().AddCrmsView("Group1"))
                .Build();

    }
}