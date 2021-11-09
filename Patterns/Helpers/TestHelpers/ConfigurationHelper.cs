using Microsoft.Extensions.Configuration;
using System;
using System.IO;

namespace TNDStudios.TestHelpers
{
    public class ConfigurationHelper
    {
        public static IConfigurationRoot GetIConfigurationRoot(string outputPath)
        {
            return new ConfigurationBuilder()
                .SetBasePath(outputPath)
                .AddJsonFile("appsettings.json", optional: true)
                //.AddEnvironmentVariables()
                .Build();
        }

        public static T GetConfiguration<T>(string outputPath, string section = null)
        {
            T configuration = (T)Activator.CreateInstance(typeof(T));

            var root = GetIConfigurationRoot(Directory.GetCurrentDirectory());

            if (string.IsNullOrEmpty(section))
                root.Bind(configuration);
            else
                root.GetSection(section).Bind(configuration);

            return configuration;
        }
    }
}
