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
            services.AddTransient<ITransaction<CreateServiceRequest, TransactionRecord>, CreateServiceTransaction>();
            services.AddTransient<ITransaction<DeleteServiceRequest, TransactionRecord>, DeleteServiceTransaction>();
            services.AddTransient<ITransaction<SellTicketsCommand, TransactionRecord>, SellTicketsTransaction>();
            services.AddTransient<ITransaction<ChangeTicketsCommand, TransactionRecord>, ChangeTicketsTransaction>();
            services.AddTransient<ITransaction<CancelTicketsRequest, TransactionRecord>, CancelTicketsTransaction>();
            return services;
        }
    }
}
