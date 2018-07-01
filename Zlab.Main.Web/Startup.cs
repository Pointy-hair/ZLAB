using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Zlab.Main.Web.Hubs;
using Zlab.Main.Web.MidWares;
using Zlab.Main.Web.Service.Implments;
using Zlab.Main.Web.Service.Interfaces;
using Zlab.Main.Web.Services.Implements;
using Zlab.Main.Web.Services.Interfaces;
using Zlab.Web.Main.Services.Interfaces;
using Zlab.Web.Service.Implments;
using Zlab.Web.Service.Interfaces;
using ZLAB.Controllers;

namespace Zlab.Main.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //services = services
            //        .AddScoped<IIdsService, IdsService>()
            //        .AddScoped<ISessionManager, SessionManager>();

            //var serviceProvider = services.BuildServiceProvider();
            services.AddSingleton<IIdsService, IdsService>();
            services.AddSingleton<ISessionManager, SessionManager>(); 
            services.AddSingleton<IMessageService, MessageService>();
            services.AddSingleton<IAccountService, AccountService>();
            //services.AddScoped<IMessageService, MessageService>(x => new MessageService(x.GetService<ISessionManager>()));
            //services.AddScoped<IAccountService, AccountService>(x => new AccountService(x.GetService<IIdsService>(), x.GetService<ISessionManager>()));
            //services.AddScoped<AccountController>();
            //services.AddScoped<MessageController>();
            //services.AddScoped(x => new AccountController(x.GetService<IAccountService>()));
            //services.AddScoped(x => new MessageController(x.GetService<IMessageService>()));

            services.Configure<CookiePolicyOptions>(options =>
            {
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = Microsoft.AspNetCore.Http.SameSiteMode.None;
            });
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            services.AddSignalR();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                //app.UseBrowserLink();
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();
            //app.UseSignalR(routes =>
            //{
            //    routes.MapHub<SocketHub>("/websocket");
            //});
            app.Map("/ws/websocket", (con) =>
            {
                con.UseWebSockets();
                con.UseMiddleware<ChatWebSocketMiddleware>();
            });
            app.UseSignalR(routes =>
            {
                routes.MapHub<SocketHub>("/websocket");
            });
            app.UseMvc();
        }
    }
}
