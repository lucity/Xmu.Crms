using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Xmu.Crms.Shared.Models;

namespace Xmu.Crms.Connections
{
    public class JwtConfigurtaionServices
    {
        private readonly IOptions<JwtSettings> _appConfiguration;
        public JwtConfigurtaionServices(IOptions<JwtSettings> appConfiguration)
        {
            _appConfiguration = appConfiguration;
        }

        public JwtSettings JwtConfigurations
        {
            get
            {
                return _appConfiguration.Value;
            }
        }
    }
}
