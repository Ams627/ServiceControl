using System;
using System.Collections.Generic;
using Microsoft.Win32;

namespace ServiceControl
{
    class WindowsService
    {
        public void AddService(string name)
        {
            var keyName = @"SYSTEM\CurrentControlSet\Services";
            var registryView = RegistryView.Registry64;
            using (var basekey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, registryView))
            {
                using (var key = basekey.OpenSubKey(keyName, true))
                {
                    var subkeyNames = key.GetSubKeyNames();
                    if (subkeyNames == null)
                    {
                        throw new Exception($"Cannot get subkeys from {keyName}");
                    }
                    var keyset = new HashSet<string>(subkeyNames);
                    if (keyset.Contains(name))
                    {
                        throw new Exception($"the registry entry for the service {key} already exists");
                    }
                    using (var newkey = key.CreateSubKey(name))
                    {

                    }
                }
            }
        }

        public void RemoveService(string name)
        {
            var keyName = @"SYSTEM\CurrentControlSet\Services";
            using (var basekey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64))
            {
                using (var key = basekey.OpenSubKey(keyName, true))
                {
                    var subkeyNames = key.GetSubKeyNames();
                    if (subkeyNames == null)
                    {
                        throw new Exception($"Cannot get subkeys from {keyName}");
                    }
                    var keyset = new HashSet<string>(subkeyNames);
                    if (!keyset.Contains(name))
                    {
                        throw new Exception($"the registry entry for the service {key} does not exist");
                    }

                    key.DeleteSubKeyTree(name);
                }
            }
        }
    }
}
