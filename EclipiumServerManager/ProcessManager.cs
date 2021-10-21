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
        
        public static void RunCommandWithBashNoOutput(string command)
        {
            var psi = new ProcessStartInfo
            {
                FileName = "/bin/bash",
                Arguments = "-c \"" + command + "\"",
                RedirectStandardOutput = false,
                UseShellExecute = true,
                CreateNoWindow = true
            };

            using var process = Process.Start(psi);
            if (process == null)
                throw new Exception("Command does not exist");

            process.WaitForExit();

        }
        
        public static string RunCommandWithSystemD(string command)
        {
            var psi = new ProcessStartInfo
            {
                FileName = "/bin/systemctl",
                Arguments = command,
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
    }
}