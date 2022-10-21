namespace Microsoft.Extensions.Configuration.Registry
{
    using System;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Win32;

#pragma warning disable CA1416 // Validate platform compatibility

    public class RegistryConfigurationProvider : ConfigurationProvider
    {
        private readonly string rootKeyName;
        private readonly RegistryHive registryHive;

        public RegistryConfigurationProvider(string rootKeyName, RegistryHive registryHive)
        {
            if (string.IsNullOrWhiteSpace(rootKeyName))
            {
                throw new ArgumentException($"'{nameof(rootKeyName)}' cannot be null or whitespace.", nameof(rootKeyName));
            }

            this.rootKeyName = rootKeyName;
            this.registryHive = registryHive;
        }

        public override void Load()
        {
            var hive = RegistryKey.OpenBaseKey(this.registryHive, RegistryView.Default);

            using var currentKey =  hive.OpenSubKey(this.rootKeyName);
            this.ProcessRegistryKey(currentKey);
        }

        private void ProcessRegistryKey(RegistryKey currentKey)
        {
            foreach (var n in currentKey.GetValueNames())
            {
                var value = currentKey.GetValue(n);
                var rType = currentKey.GetValueKind(n);

                var dataKeyName = currentKey.Name;
                    
                dataKeyName = dataKeyName
                    .Substring(dataKeyName.IndexOf("\\") + 1);
                dataKeyName = dataKeyName
                    .Substring(this.rootKeyName.Length);
                
                dataKeyName = dataKeyName
                    .TrimStart('\\')
                    .Replace("\\", ":");

                if (!string.IsNullOrEmpty(n))
                {
                    if (!string.IsNullOrEmpty(dataKeyName))
                    {
                        dataKeyName += ":";
                    }

                    dataKeyName += n;
                }

                switch (rType)
                {
                    case RegistryValueKind.String:
                        this.Data[dataKeyName] = (string)value;
                        break;

                    case RegistryValueKind.ExpandString:
                        this.Data[dataKeyName] = Environment.ExpandEnvironmentVariables((string)value);
                        break;


                    case RegistryValueKind.MultiString:
                        var values = (string[])value;
                        for (var i = 0; i < values.Length; i++)
                        {
                            this.Data[dataKeyName + ":" + i] = values[i];
                        }

                        break;

                    case RegistryValueKind.Binary:
                        this.Data[dataKeyName] = Convert.ToBase64String((byte[])value);
                        break;

                    case RegistryValueKind.None:
                    case RegistryValueKind.Unknown:
                    case RegistryValueKind.DWord:
                    case RegistryValueKind.QWord:
                    default:
                        this.Data[dataKeyName] = value.ToString();
                        break;
                }
            }

            foreach (var s in currentKey.GetSubKeyNames())
            {
                using var subKey = currentKey.OpenSubKey(s);
                this.ProcessRegistryKey(subKey);
            }
        }
    }
#pragma warning restore CA1416 // Validate platform compatibility
}
