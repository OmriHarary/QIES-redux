using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using QIES.Api.Models;
using QIES.Common;
using QIES.Core;
using QIES.Core.Services;
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
            services.AddSingleton<IServicesList>(sp => new ValidServicesList(new System.IO.FileInfo("QIES.Cli/test/resources/valid-services-list.txt")));
            services.AddSingleton<ITransactionQueue>(sp => new Infra.TransactionQueue());
            services.AddTransient<ITransaction<CreateServiceRequest, Service>, CreateServiceTransaction>();
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
