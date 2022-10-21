namespace Microsoft.Extensions.Configuration.Registry
{
    using System;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Win32;

    public class RegistryConfigurationSource : IConfigurationSource
    {
        private readonly string rootKey;
        private readonly RegistryHive hive;

        public RegistryConfigurationSource(string rootKey, RegistryHive hive)
        {
            if (string.IsNullOrWhiteSpace(rootKey))
            {
                throw new ArgumentException($"'{nameof(rootKey)}' cannot be null or whitespace.", nameof(rootKey));
            }

            this.rootKey = rootKey;
            this.hive = hive;
        }

        public IConfigurationProvider Build(IConfigurationBuilder builder)
            => new RegistryConfigurationProvider(this.rootKey, this.hive);
    }
}
