using KinoLotteryData.Data;
using KinoLotteryData.Services;
using KinoLotteryData.Services.Repositories;
using KinoLotteryWeb.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic; 
using System.Linq;
using System.Threading.Tasks;

namespace KinoLotteryWeb
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
            services.AddDbContext<KinoLotteryContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("WorkConnectionString")));

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(opt => {
                opt.LoginPath = "/KinoInJQuery/Login.html";


            });
            services.AddHostedService<LotteryService>();
            services.AddHostedService<SendLotteryToFrontService>();

            services.AddControllers();

            services.AddSignalR();

            services.AddScoped<IPlayerRepository, PlayerRepository>();
            services.AddScoped<ITicketRepository, TicketRepository>();
            services.AddScoped<ILotteryRepository, LotteryRepository>();
            services.AddScoped<ILotteryTicketRepository, LotteryTicketRepository>();
            
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseStaticFiles();
            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<LotteryHub>("/lottery");
            });
        }
    }
}
