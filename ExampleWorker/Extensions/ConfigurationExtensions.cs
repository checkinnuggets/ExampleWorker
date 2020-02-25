using Microsoft.Extensions.Configuration;

namespace ExampleWorker.Extensions
{
    public static class ConfigurationExtensions
    {
        public static IConfigurationSection GetSection<T>(this IConfiguration config)
        {
            var sectionName = typeof(T).Name;
            return config.GetSection(sectionName);
        }
    }
}
