using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceProcess;
using Microsoft.Win32;

namespace ServiceControl
{
    class Program
    {
        private static void Main(string[] args)
        {
            try
            {
                PrintServicesFromRegistry();
                ServiceController service = new ServiceController("WpnService");
                Console.WriteLine($"service status is {service.Status}");
            }
            catch (Exception ex)
            {
                var fullname = System.Reflection.Assembly.GetEntryAssembly().Location;
                var progname = Path.GetFileNameWithoutExtension(fullname);
                Console.Error.WriteLine(progname + ": Error: " + ex.Message);
            }

        }



        private static void PrintServicesFromRegistry(bool is64Bit = true)
        {
            var keyName = @"SYSTEM\CurrentControlSet\Services"; 
            var registryView = is64Bit ? RegistryView.Registry64 : RegistryView.Registry32;
            using (var basekey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, registryView))
            {
                using (var key = basekey.OpenSubKey(keyName))
                {
                    var subkeyNames = key.GetSubKeyNames();
                    if (subkeyNames != null && subkeyNames.Any())
                    {
                        foreach (var subkeyName in subkeyNames)
                        {
                            Console.WriteLine($"{subkeyName}");
                        }
                    }
                }
            }
        }
    }
}
