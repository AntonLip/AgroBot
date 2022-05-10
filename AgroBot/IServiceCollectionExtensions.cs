using AgroBot.DataAccess;
using AgroBot.Models.Interfaces.IRepository;
using AgroBot.Models.Interfaces.IService;
using AgroBot.Services;
using Microsoft.Extensions.DependencyInjection;

namespace AgroBot
{
    public static class IServiceCollectionExtensions
    {
        public static void AddRepositories(this IServiceCollection services)
        {
            services.AddTransient<IUserRepository, UserRepository>();

        }

        public static void AddServices(this IServiceCollection services)
        {
            services.AddTransient<IUserService, UserService>();
        }
    }
}
