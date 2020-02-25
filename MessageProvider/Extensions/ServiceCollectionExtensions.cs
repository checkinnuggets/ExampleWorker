using Microsoft.Extensions.DependencyInjection;

namespace MessageProvider.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection UseHelloWorldMessageProvider(this IServiceCollection services)
        {
            services.AddTransient<IMessageProvider, HelloWorldMessageProvider>();
            return services;
        }
    }
}
