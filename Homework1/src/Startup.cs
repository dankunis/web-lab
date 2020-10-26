using System.Threading.Tasks;
using AutoMapper;
using Homework1.Database;
using Homework1.Database.Entities;
using Homework1.Hubs;
using Homework1.Models.Requests;
using Homework1.Models.Responses;
using Homework1.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Qoden.Validation.AspNetCore;

namespace Homework1
{
    public class Startup
    {
        private IHostingEnvironment Environment;
        private IConfiguration Configuration;

        public Startup(IHostingEnvironment env, IConfiguration config)
        {
            Environment = env;
            Configuration = config;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc(options => options.Filters.Add<ApiExceptionFilterAttribute>());

            services.AddSignalR();
            services.AddScoped<IChatService, ChatService>();
            services.AddScoped<ILoginService, LoginService>();
            services.AddScoped<IRateRequestService, RateRequestService>();
            services.AddScoped<IManagementAreaService, ManagementAreaService>();
            services.AddScoped<IAccountService, AccountService>();
            services.AddSingleton<IDbConnectionFactory, DbConnectionFactory>();

            services.Configure<DbOptions>(Configuration);
            Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.Events.OnRedirectToLogin = ctx =>
                    {
                        ctx.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        return Task.CompletedTask;
                    };
                    options.Events.OnRedirectToAccessDenied  = ctx =>
                    {
                        ctx.Response.StatusCode = StatusCodes.Status403Forbidden;
                        return Task.CompletedTask;
                    };
                });

            ConfigureAutoMapper();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseCors(x => x.WithOrigins("http://localhost:63342")
                .AllowAnyHeader().AllowAnyMethod().AllowCredentials());
            app.UseMvc();
            app.UseSignalR(routes => routes.MapHub<ChatHub>("/ws/chat"));
        }

        private void ConfigureAutoMapper()
        {
            Mapper.Initialize(cfg =>
            {
                cfg.CreateMap<User, ProfileResponse>();
                cfg.CreateMap<SalaryRateRequest, SalaryRateRequestResponse>();
                cfg.CreateMap<CreateUserRequest, User>();
                cfg.CreateMap<User, UserInfoResponse>();
                cfg.CreateMap<UserInfoResponse, User>();
                cfg.CreateMap<UpdateUserInfoRequest, User>();
                cfg.CreateMap<Message, MessageResponse>();
                cfg.CreateMap<Chat, ChatResponse>();
                cfg.CreateMap<UpdateUserInfoRequest, UserInfoResponse>();
            });
        }
    }
}