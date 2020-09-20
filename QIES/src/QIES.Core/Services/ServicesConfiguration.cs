using Microsoft.Extensions.DependencyInjection;
using QIES.Api.Models;
using QIES.Common.Record;
using QIES.Core.Commands;

namespace QIES.Core.Services
{
    public static class ServicesConfiguration
    {
        public static IServiceCollection AddTransactions(this IServiceCollection services)
        {
            services.AddTransient<ILoginService, LoginService>();
            services.AddTransient<ILogoutService, LogoutService>();
            services.AddTransient<ITransaction<CreateServiceRequest>, CreateServiceTransaction>();
            services.AddTransient<ITransaction<DeleteServiceRequest>, DeleteServiceTransaction>();
            services.AddTransient<ITransaction<SellTicketsCommand>, SellTicketsTransaction>();
            services.AddTransient<ITransaction<ChangeTicketsCommand>, ChangeTicketsTransaction>();
            services.AddTransient<ITransaction<CancelTicketsRequest>, CancelTicketsTransaction>();
            return services;
        }
    }
}
