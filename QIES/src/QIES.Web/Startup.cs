using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using QIES.Core;
using QIES.Core.Services;
using QIES.Core.Users;
using QIES.Infra;

namespace QIES.Web
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
            services.AddSingleton<IServicesList>(sp => new ValidServicesList(new System.IO.FileInfo("QIES/src/QIES.Web/Resources/valid-services-list.txt"))); // TODO: This needs to be configurable
            services.AddSingleton<IUserManager>(sp => new UserManager());
            services.AddTransient<ISummaryWriter, SummaryWriter>();
            services.AddTransactions();
            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}