using Microsoft.Extensions.DependencyInjection;
using QIES.Api.Models;
using QIES.Common;
using QIES.Common.Record;

namespace QIES.Core.Services
{
    public static class ServicesConfiguration
    {
        public static IServiceCollection AddTransactions(this IServiceCollection services)
        {
            services.AddTransient<ILoginService, LoginService>();
            services.AddTransient<ILogoutService, LogoutService>();
            services.AddTransient<ITransaction<CreateServiceRequest, Service>, CreateServiceTransaction>();
            services.AddTransient<ITransaction<DeleteServiceRequest, TransactionRecord>, DeleteServiceTransaction>();
            services.AddTransient<ITransaction<SellOrChangeTicketsRequest, TransactionRecord>, SellOrChangeTicketsTransaction>();
            services.AddTransient<ITransaction<CancelTicketsRequest, TransactionRecord>, CancelTicketsTransaction>();
            return services;
        }
    }
}
