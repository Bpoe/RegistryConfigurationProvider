namespace Microsoft.Extensions.Configuration.Registry
{
    using Microsoft.Extensions.Configuration;
    using Microsoft.Win32;

#pragma warning disable CA1416 // Validate platform compatibility
    public static class ConfigurationBuilderExtensions
    {
        public static IConfigurationBuilder AddRegistryKey(this IConfigurationBuilder config, string key, RegistryHive hive = RegistryHive.LocalMachine)
            => config.Add(new RegistryConfigurationSource(key, hive));
    }

#pragma warning restore CA1416 // Validate platform compatibility
}
