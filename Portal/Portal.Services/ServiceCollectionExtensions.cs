using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Portal.Services.API;
using Portal.Services.API.Base;
using Portal.Services.AuthProviders;
using Portal.Services.Managers;
using Portal.Shared.Interfaces;
using Portal.Shared.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Portal.Services
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// All portal service Ioc adding here 
        /// </summary>
        /// <param name="services"></param>
        public static void AddPortalServices(this IServiceCollection services)
        {
            services.AddScoped<IAuthenticationService, AuthenticationService>();
            services.AddScoped<AuthStateProvider>();
            services.AddScoped<AuthenticationStateProvider, AuthStateProvider>();

            // 
            services.AddTransient<IAuthenticationService, AuthenticationService>();
        }
    }
}
