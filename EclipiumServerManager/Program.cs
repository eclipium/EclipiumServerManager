using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

namespace EclipiumServerManager
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                Console.WriteLine("Programme uniquement compatible avec Linux.");
                return;
            }

            if (Environment.UserName != "root")
            {
                Console.WriteLine("Merci d'exécuter le programme avec des prévilèges d'administrateur.");
                return;
            }

            //Création de l'utilisateur EclipiumServerManager et les fichiers nécessaires s'ils n'existent pas
            CreateUserIfDoesNotExist();

            //Execution de la commande demandée par l'utilisateur
            var command = args.Length == 0 ? "" : args[0];

            Dictionary<string, Action> commandsList = new()
            {
                {"help", Help},
                {"list", AppsInformations.ListApps},
                {"create", AppCreator.CreateApp},
                {"status", () => AppsInformations.GetAppStatus(args)},
                {"log", () => AppsInformations.GetAppLog(args)},
                {"start", () => PowerOperations.StartApp(args)},
                {"restart", () => PowerOperations.RestartApp(args)},
                {"stop", () => PowerOperations.StopApp(args)},
                {"kill", () => PowerOperations.KillApp(args)},
                {"enable", () => AutostartManage.EnableApp(args)},
                {"disable", () => AutostartManage.DisableApp(args)},
                {"delete", () => ManageApps.DeleteApp(args)},
                {"rename", () => ManageApps.RenameApp(args)},
                {"backup", () => AppsBackups.Backup(args)},
                {"list-backups", () => AppsBackups.ListBackups(args)},
                {"delete-backup", () => AppsBackups.Delete(args)},
                {"restore-backup", () => AppsBackups.Restore(args)}
            };

            if(commandsList.ContainsKey(command))
                commandsList[command].Invoke();
            else
                Console.WriteLine("Commande inconnue, faites 'esm help' pour plus d'informations");

        }

        private static void Help()
        {
            var asm = Assembly.GetExecutingAssembly();
            var resourceName = asm.GetManifestResourceNames()
                .Single(str => str.EndsWith("Help.txt"));
            using var resource = asm.GetManifestResourceStream(resourceName);
            var help = Encoding.UTF8.GetString(ResourceReader.ReadFully(resource));
            Console.WriteLine(help);
        }
        
        private static void CreateUserIfDoesNotExist()
        {
            if (!Directory.Exists("/home/eclipium-server-manager"))
            {
                ProcessManager.RunCommandWithBash("useradd eclipium-server-manager");
            }

            if (!Directory.Exists("/home/eclipium-server-manager/applications"))
            {
                ProcessManager.RunCommandWithBash("mkdir /home/eclipium-server-manager/applications");
            }


            if (!File.Exists("/home/eclipium-server-manager/applications/.apps"))
            {
                File.WriteAllText("/home/eclipium-server-manager/applications/.apps", "[]");
            }
        }
        
    }
}