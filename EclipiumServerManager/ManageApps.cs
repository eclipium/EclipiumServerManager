using System;
using System.IO;
using System.Text.Json;

namespace EclipiumServerManager
{
    public static class ManageApps
    {
        public static void RenameApp(string[] args)
        {
            if (args.Length != 3)
            {
                Console.WriteLine("Veuillez spécifier une application");
                return;
            }

            var apps = AppsInformations.GetAppsAsList();
            foreach (var app in apps)
            {
                if (app.DisplayName == args[1] || app.ServiceName == args[1])
                {
                    var newServiceName = args[2].ToLower().Replace(" ", "-");
                    
                    foreach (var app2 in apps)
                    {
                        if (app2.DisplayName == args[2] || app2.ServiceName == newServiceName || File.Exists("/etc/systemd/system/" + newServiceName + ".service") || File.Exists("/usr/lib/systemd/system/" + newServiceName + ".service"))
                        {
                            Console.WriteLine("Le nom d'application existe déjà");
                            return;
                        }
                    }
                    
                    ProcessManager.RunCommandWithSystemD("disable " + app.ServiceName);
                    ProcessManager.RunCommandWithBash("mv /etc/systemd/system/" + app.ServiceName + ".service /etc/systemd/system/" + newServiceName + ".service");
                    ProcessManager.RunCommandWithBash("mv /home/eclipium-server-manager/applications/" + app.ServiceName + " /home/eclipium-server-manager/applications/" + newServiceName);
                    app.DisplayName = args[2];
                    File.WriteAllText("/etc/systemd/system/" + newServiceName + ".service",File.ReadAllText("/etc/systemd/system/" + newServiceName + ".service").Replace(app.ServiceName, newServiceName));
                    app.Directory = app.Directory.Replace(app.ServiceName, newServiceName);
                    app.ServiceName = newServiceName;
                    ProcessManager.RunCommandWithSystemD("enable " + app.ServiceName);
                    File.WriteAllText("/home/eclipium-server-manager/applications/.apps", JsonSerializer.Serialize(apps));
                    Console.WriteLine("L'application a bien été renommée.");
                    return;
                }
            }
            Console.WriteLine("L'application n'existe pas");
        }
        
        public static void DeleteApp(string[] args)
        {
            if (args.Length != 2)
            {
                Console.WriteLine("Veuillez spécifier une application");
                return;
            }
            
            bool? canDelete = null;
            while (canDelete == null)
            {
                Console.Write("Voulez-vou vraiment supprimer l'application [o/N]");
                var result = Console.ReadKey();
                if (result.Key == ConsoleKey.Enter || result.Key == ConsoleKey.N)
                {
                    Console.Write("\n");
                    return;
                }
                if (result.Key == ConsoleKey.O)
                    canDelete = true;
                Console.Write("\n");
            }
            

            var apps = AppsInformations.GetAppsAsList();
            foreach (var app in apps)
            {
                if (app.DisplayName == args[1] || app.ServiceName == args[1])
                {
                    ProcessManager.RunCommandWithSystemD("disable " + app.ServiceName);
                    ProcessManager.RunCommandWithBash("rm -f /etc/systemd/system/" + app.ServiceName + ".service");
                    ProcessManager.RunCommandWithBash("rm -rf /home/eclipium-server-manager/applications/" + app.ServiceName);
                    apps.Remove(app);
                    File.WriteAllText("/home/eclipium-server-manager/applications/.apps", JsonSerializer.Serialize(apps));
                    Console.WriteLine("L'application a bien été supprimée.");
                    return;
                }
            }
            Console.WriteLine("L'application n'existe pas");
        }
    }
}