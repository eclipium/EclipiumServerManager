using System;
using System.Diagnostics;
using System.Reflection.Metadata.Ecma335;

namespace EclipiumServerManager
{
    public static class ProcessManager
    {
        public static string RunCommandWithBash(string command)
        {
            var psi = new ProcessStartInfo
            {
                FileName = "/bin/bash",
                Arguments = "-c \"" + command + "\"",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var process = Process.Start(psi);
            if (process == null)
                throw new Exception("Command does not exist");

            process.WaitForExit();

            var output = process.StandardOutput.ReadToEnd();

            return output;
        }
        
        public static string RunCommandWithSystemD(string command)
        {
            var psi = new ProcessStartInfo();
            psi.FileName = "/bin/systemctl";
            psi.Arguments = command;
            psi.RedirectStandardOutput = true;
            psi.UseShellExecute = false;
            psi.CreateNoWindow = true;

            using var process = Process.Start(psi);
            if (process == null)
                throw new Exception("Command does not exist");

            process.WaitForExit();

            var output = process.StandardOutput.ReadToEnd();

            return output;
        }
    }
}