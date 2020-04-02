using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using Microsoft.Win32;
using System.ServiceProcess;

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

                var ws = new WindowsService();
                ws.RemoveService("helloservice");

            }
            catch (Exception ex)
            {
                var fullname = System.Reflection.Assembly.GetEntryAssembly().Location;
                var progname = Path.GetFileNameWithoutExtension(fullname);
                Console.Error.WriteLine(progname + ": Error: " + ex.Message);
            }

        }


        class Finder
        {
            private readonly string pattern;
            private readonly string startdir;
            private readonly List<string> files = new List<string>();

            public Finder(string pattern, string startdir = "")
            {
                this.pattern = pattern;
                this.startdir = startdir;
                FindFiles(pattern, string.IsNullOrEmpty(startdir) ? Directory.GetCurrentDirectory() : startdir);
            }

            private void FindFiles(string pattern, string startdir)
            {
                var files = Directory.GetFiles(startdir, pattern);
                this.files.AddRange(files);
                var dirs = Directory.GetDirectories(startdir);
                foreach (var dir in dirs)
                {
                    FindFiles(pattern, dir);
                }
            }

            public List<string> Files => files;
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
                            using (var k2 = key.OpenSubKey(subkeyName))
                            {
                                var image = k2.GetValue("ImagePath");
                                if (image != null)
                                {
                                    Console.WriteLine($"    {image}");
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
