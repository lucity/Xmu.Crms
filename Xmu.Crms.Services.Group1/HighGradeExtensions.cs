using Xmu.Crms.Services.Group1;
using Xmu.Crms.Services.Group1.Dao;
using Xmu.Crms.Services.HighGrade;
using Xmu.Crms.Shared.Service;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class HignGradeExtension
    {
        // 为每一个你写的Service写一个这样的函数

        //school 注入
        public static IServiceCollection AddHignGradeSeminarService(this IServiceCollection serviceCollection)
        {
            return serviceCollection.AddScoped<ISeminarService, SeminarService>();
        }
        
       
    }
}
