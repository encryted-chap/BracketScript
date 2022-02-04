using System;
using System.Diagnostics;
using System.Net;
using System.Collections.Generic;
using System.IO;

namespace installer
{
    class Program
    {
        static void Main(string[] args)
        {
            bool admin = !runCMD("/c net session").Contains("Access is denied."); // check if user is admin
            if(!admin) {
                Console.WriteLine("Please run with administrator privilege.");
                EndProg();
            }
            Console.WriteLine("Admin privilege found, downloading necessary files...");
            WebClient c = new WebClient();

            List<byte[]> bins = new List<byte[]>();
            for(int i = 0; i < binaries.Length; i++) {
                Console.WriteLine("attempting..."+binaries[i]);
                bins.Add(c.DownloadData(binaries[i]));
                Console.WriteLine("got ok..."+binaries[i]);
            }
            Console.WriteLine("\nFiles downloaded, creating necessary dirs...");
            foreach(string d in dirs) {
                if(!Directory.Exists(d))
                    Directory.CreateDirectory(d);
            }

            EndProg();
        }
        static string[] binaries = new string[] {
            "https://cygwin.com/setup-x86_64.exe"
        };
        static string[] dirs = new string[] {
            "prog_downloads", "%appdata%/bracketscript"
        };
        
        static string runCMD(string cmd) {
            
            ProcessStartInfo proc_i = new ProcessStartInfo() {
                CreateNoWindow=true,
                WindowStyle=ProcessWindowStyle.Hidden,
                RedirectStandardOutput=true,
                RedirectStandardError=true,
                FileName="cmd.exe",
                Arguments=cmd
            };
            
            var proc = Process.Start(proc_i);
            proc.WaitForExit(); // wait for process to end
            
            return proc.StandardError.ReadToEnd();
        }
        static void EndProg() {
            Console.WriteLine("\n\nPress any key to exit...");
            Console.ReadKey();
            Environment.Exit(0);
        }
    }
}
