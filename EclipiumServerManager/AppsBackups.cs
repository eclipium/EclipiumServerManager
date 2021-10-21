using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using ConsoleTables;

namespace EclipiumServerManager
{
    public static class AppsBackups
    {
        public static void Backup(string[] args)
        {
            if (args.Length != 2)
            {
                Console.WriteLine("Veuillez spécifier une application");
                return;
            }

            if (args[1] == "all")
            {
                var apps = AppsInformations.GetAppsAsList();
                foreach (var app in apps)
                {
                    BackupApp(app.ServiceName);
                }
            }
            else
            {
                BackupApp(args[1]);
            }
        }

        public static void Delete(string[] args)
        {
            if (args.Length != 2)
            {
                Console.WriteLine("Veuillez spécifier une backup");
                return;
            }

            if (File.Exists("/home/eclipium-server-manager/backups/" + args[1]))
            {
                File.Delete("/home/eclipium-server-manager/backups/" + args[1]);
                Console.WriteLine("La backup a bien été supprimée");
            }
            else
            {
                Console.WriteLine("La backup n'existe pas");
            }
        }
        
        private static void BackupApp(string appName)
        {

            var apps = AppsInformations.GetAppsAsList();
            foreach (var app in apps)
            {
                if (app.DisplayName == appName || app.ServiceName == appName)
                {
                    var current = DateTime.Now;
                    if (!Directory.Exists("/home/eclipium-server-manager/backups/"))
                        Directory.CreateDirectory("/home/eclipium-server-manager/backups/");
                    var backupFile = "/home/eclipium-server-manager/backups/" + app.ServiceName +
                                     "-"+current.ToString("dd-MM-yyyy-HH:mm")+".tar.gz";
                    CreateTarGz(backupFile, app.ServiceName);
                    Console.WriteLine($"La backup de l'application '{app.DisplayName}' a été créée dans le fichier : ");
                    Console.WriteLine(backupFile);
                    return;
                }
            }
            Console.WriteLine("L'application n'existe pas");
        }

        public static void Restore(string[] args)
        {
            if (args.Length != 2)
            {
                Console.WriteLine("Veuillez spécifier une backup");
                return;
            }

            if (File.Exists("/home/eclipium-server-manager/backups/" + args[1]))
            {
                var serviceName = (new Regex(@"(.+?)(?=-[\d]{2}-[\d]{2}-[\d]{4}-[\d]{2}:[\d]{2}.tar.gz)")).Match(args[1]).Value;
                var apps = AppsInformations.GetAppsAsList();
                foreach (var app in apps)
                {
                    if(app.ServiceName == serviceName)
                    {
                        ProcessManager.RunCommandWithBash($"rm -rf /home/eclipium-server-manager/applications/{serviceName}");
                        ProcessManager.RunCommandWithBashNoOutput($"tar -xf /home/eclipium-server-manager/backups/{args[1]} -C /home/eclipium-server-manager/applications/");
                        Console.WriteLine("La backup a bien été restaurée");
                        return;
                    }
                }
                Console.WriteLine("L'application à restaurer n'existe pas");
            }
            else
            {
                Console.WriteLine("La backup n'existe pas");
            }
        }
        
        public static void ListBackups(string[] args)
        {
            string app = null;
            if (args.Length == 2)
            {
                var checkApp = AppsInformations.GetAppsAsList().Find(application =>
                    application.ServiceName == args[1] || application.DisplayName == args[1]);
                if (checkApp != null)
                    app = checkApp.ServiceName;
                else
                {
                    Console.WriteLine("L'application n'existe pas");
                    return;
                }
            }

            var backupsList = Directory.GetFiles("/home/eclipium-server-manager/backups/");
            Console.WriteLine(app == null ? "\nListe des sauvegardes" : $"\nListe des sauvegardes pour l'application {app}");
            var table = new ConsoleTable("Nom", "Service", "Date", "Heure", "Nom du fichier");
            foreach (var backupPath in backupsList)
            {
                var backup = backupPath.Split("/").Last();
                var serviceName = (new Regex(@"(.+?)(?=-[\d]{2}-[\d]{2}-[\d]{4}-[\d]{2}:[\d]{2}.tar.gz)")).Match(backup).Value;
                if (app == null || app == serviceName)
                {
                    var apps = AppsInformations.GetAppsAsList();
                    foreach (var app2 in apps)
                    {
                        if(app2.ServiceName == serviceName){
                            table.AddRow(app2.DisplayName, app2.ServiceName, 
                                (new Regex(@"([\d]{2}-[\d]{2}-[\d]{4})(?=-[\d]{2}:[\d]{2}.tar.gz)")).Match(backup).Value,
                                (new Regex(@"(?<=[\d]{2}-[\d]{2}-[\d]{4}-)([\d]{2}:[\d]{2})(?=.tar.gz)")).Match(backup).Value,
                                backup);
                        }
                    }
                }
            }
            
            table.Write();
            Console.WriteLine();

        }
        
        private static void CreateTarGz(string tgzFilename, string folderName)
        {
            ProcessManager.RunCommandWithBashNoOutput($"tar -czvf {tgzFilename} --directory=/home/eclipium-server-manager/applications {folderName}");
        }
        
    }
}