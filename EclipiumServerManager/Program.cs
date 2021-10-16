using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using ConsoleTables;

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

            switch (command)
            {
                case "help":
                    Help();
                    break;
                case "list":
                    ListApps();
                    break;
                case "create":
                    CreateApp();
                    break;
                case "status":
                    GetAppStatus(args);
                    break;
                case "log":
                    GetAppLog(args);
                    break;
                case "start":
                    StartApp(args);
                    break;
                case "stop":
                    StopApp(args);
                    break;
                case "restart":
                    RestartApp(args);
                    break;
                case "kill":
                    KillApp(args);
                    break;
                case "enable":
                    EnableApp(args);
                    break;
                case "disable":
                    DisableApp(args);
                    break;
                default:
                    Console.WriteLine("Commande inconnue, exécutez 'esm help' pour la liste des commandes.");
                    break;
            }

        }

        private static void Help()
        {
            Console.WriteLine("Liste des commandes :");
            Console.WriteLine("esm list => Affiche la liste des applications hébergées");
            Console.WriteLine("esm status <app> => Affiche le statut d'une application");
            Console.WriteLine("esm log <app> => Affiche l'historique de la console de l'application");
            Console.WriteLine("esm start <app> => Démarre une application");
            Console.WriteLine("esm stop <app> => Arrête une application");
            Console.WriteLine("esm restart <app> => Redémmare une application");
            Console.WriteLine("esm kill <app> => Tue le processus d'une application");
            Console.WriteLine("esm enable <app> => Active le démarrage automatique de l'application au démarrage du système");
            Console.WriteLine("esm disable <app> => Désactive le démarrage automatique de l'application au démarrage du système");
        }
        
        private static void CreateUserIfDoesNotExist()
        {
            if (!Directory.Exists("/home/eclipium-server-manager"))
            {
                RunCommandWithBash("useradd eclipium-server-manager");
            }

            if (!Directory.Exists("/home/eclipium-server-manager/applications"))
            {
                RunCommandWithBash("mkdir /home/eclipium-server-manager/applications");
            }


            if (!File.Exists("/home/eclipium-server-manager/applications/.apps"))
            {
                File.WriteAllText("/home/eclipium-server-manager/applications/.apps", "[]");
            }
        }

        private static void EnableApp(string[] args)
        {
            if (args.Length != 2)
            {
                Console.WriteLine("Veuillez spécifier une application");
                return;
            }

            var apps = GetAppsAsList();
            foreach (var app in apps)
            {
                if (app.DisplayName == args[1] || app.ServiceName == args[1])
                {
                    var status = RunCommandWithSystemD("enable " + app.ServiceName);
                    if (!string.IsNullOrEmpty(status))
                    {
                        Console.WriteLine("L'application a bien été activée au démarrage du système.");
                    }
                    else
                    {
                        Console.WriteLine("L'application est déjà activée.");
                    }
                    return;
                }
            }
            Console.WriteLine("L'application n'existe pas");
        }
        
        private static void DisableApp(string[] args)
        {
            if (args.Length != 2)
            {
                Console.WriteLine("Veuillez spécifier une application");
                return;
            }

            var apps = GetAppsAsList();
            foreach (var app in apps)
            {
                if (app.DisplayName == args[1] || app.ServiceName == args[1])
                {
                    var status = RunCommandWithSystemD("disable " + app.ServiceName);
                    if (!string.IsNullOrEmpty(status))
                    {
                        Console.WriteLine("L'application a bien été désactivée au démarrage du système.");
                    }
                    else
                    {
                        Console.WriteLine("L'application est déjà désactivée.");
                    }
                    return;
                }
            }
            Console.WriteLine("L'application n'existe pas");
        }
        
        private static void StartApp(string[] args)
        {
            if (args.Length != 2)
            {
                Console.WriteLine("Veuillez spécifier une application");
                return;
            }

            var apps = GetAppsAsList();
            foreach (var app in apps)
            {
                if (app.DisplayName == args[1] || app.ServiceName == args[1])
                {
                    var status = RunCommandWithSystemD("start " + app.ServiceName);
                    if (string.IsNullOrEmpty(status))
                    {
                        Console.WriteLine("L'application a bien été démarrée.");
                    }
                    else
                    {
                        Console.WriteLine("L'application n'a pas été démarrée, faites 'esm log " + app.ServiceName + "' pour plus d'informations.");
                    }
                    return;
                }
            }
            Console.WriteLine("L'application n'existe pas");
        }
        
        private static void RestartApp(string[] args)
        {
            if (args.Length != 2)
            {
                Console.WriteLine("Veuillez spécifier une application");
                return;
            }

            var apps = GetAppsAsList();
            foreach (var app in apps)
            {
                if (app.DisplayName == args[1] || app.ServiceName == args[1])
                {
                    var status = RunCommandWithSystemD("restart " + app.ServiceName);
                    if (string.IsNullOrEmpty(status))
                    {
                        Console.WriteLine("L'application a bien été démarrée.");
                    }
                    else
                    {
                        Console.WriteLine("L'application n'a pas été démarrée, faites 'esm log " + app.ServiceName + "' pour plus d'informations.");
                    }
                    return;
                }
            }
            Console.WriteLine("L'application n'existe pas");
        }

        
        private static void StopApp(string[] args)
        {
            if (args.Length != 2)
            {
                Console.WriteLine("Veuillez spécifier une application");
                return;
            }

            var apps = GetAppsAsList();
            foreach (var app in apps)
            {
                if (app.DisplayName == args[1] || app.ServiceName == args[1])
                {
                    var status = RunCommandWithSystemD("stop " + app.ServiceName);
                    if (string.IsNullOrEmpty(status))
                    {
                        Console.WriteLine("L'application a bien été arrêtée.");
                    }
                    else
                    {
                        Console.WriteLine("L'application n'a pas été arrêtée, faites 'esm log " + app.ServiceName + "' pour plus d'informations.");
                    }
                    return;
                }
            }
            Console.WriteLine("L'application n'existe pas");
        }
        
        private static void KillApp(string[] args)
        {
            if (args.Length != 2)
            {
                Console.WriteLine("Veuillez spécifier une application");
                return;
            }

            var apps = GetAppsAsList();
            foreach (var app in apps)
            {
                if (app.DisplayName == args[1] || app.ServiceName == args[1])
                {
                    var status = RunCommandWithSystemD("kill " + app.ServiceName);
                    if (string.IsNullOrEmpty(status))
                    {
                        Console.WriteLine("L'application a bien été tuée.");
                    }
                    else
                    {
                        Console.WriteLine("L'application n'a pas été tuée, faites 'esm log " + app.ServiceName + "' pour plus d'informations.");
                    }
                    return;
                }
            }
            Console.WriteLine("L'application n'existe pas");
        }
        
        private static void GetAppStatus(string[] args)
        {
            if (args.Length != 2)
            {
                Console.WriteLine("Veuillez spécifier une application");
                return;
            }

            var apps = GetAppsAsList();
            foreach (var app in apps)
            {
                if (app.DisplayName == args[1] || app.ServiceName == args[1])
                {
                    var status = RunCommandWithSystemD("status " + app.ServiceName);
                    var regex = new Regex(@"(?<=Active:[\s])([\S])*");
                    switch (regex.Match(status).Value)
                    {
                        case "inactive":
                            Console.WriteLine("L'application est arrêtée.");
                            break;
                        case "failed":
                            Console.WriteLine("L'application a crashé.");
                            break;
                        case "active":
                            Console.WriteLine("L'application est en cours d'exécution.");
                            break;
                        default:
                            Console.WriteLine("Etat de l'application inconnu.");
                            break;
                    }
                    return;
                }
            }
            Console.WriteLine("L'application n'existe pas");
        }
        
        private static void GetAppLog(string[] args)
        {
            if (args.Length != 2)
            {
                Console.WriteLine("Veuillez spécifier une application");
                return;
            }

            var apps = GetAppsAsList();
            foreach (var app in apps)
            {
                if (app.DisplayName == args[1] || app.ServiceName == args[1])
                {
                    var logs = RunCommandWithBash("journalctl -u testgo -n 30 --no-pager");
                    Console.Write(logs);
                    return;
                }
            }
            Console.WriteLine("L'application n'existe pas");
        }

        private static void ListApps()
        {
            Console.WriteLine("\nListe des applications :");
            var table = new ConsoleTable("Nom", "Service", "Type", "Statut","Démarrage auto", "Utilisation RAM", "Dossier d'exécution");

            var file = File.ReadAllText("/home/eclipium-server-manager/applications/.apps");
            var apps = JsonSerializer.Deserialize<Application[]>(file);
            foreach (var app in apps)
            {
                var status = RunCommandWithSystemD("status " + app.ServiceName);
                var regex = new Regex(@"(?<=Active:[\s])([\S])*");
                switch (regex.Match(status).Value)
                {
                    case "inactive":
                        table.AddRow(app.DisplayName, app.ServiceName, app.Type, "Arrêtée", status.Contains("enabled") ? "Oui" : "Non" , "0K", app.Directory);
                        break;
                    case "failed":
                        table.AddRow(app.DisplayName, app.ServiceName, app.Type, "Crash",status.Contains("enabled") ? "Oui" : "Non" , "0K", app.Directory);
                        break;
                    case "active":
                        table.AddRow(app.DisplayName, app.ServiceName, app.Type, "En cours", status.Contains("enabled") ? "Oui" : "Non", (new Regex(@"(?<=Memory:[\s])([\S])*")).Match(status).Value, app.Directory);
                        break;
                    default:
                        table.AddRow(app.DisplayName, app.ServiceName, app.Type, "Inconnu", status.Contains("enabled") ? "Oui" : "Non", "0K", app.Directory);
                        break;
                }
            }

            table.Write();
            Console.WriteLine();
        }

        private static void CreateApp()
        {
            Console.WriteLine("Quel est le nom de l'application à créer ?");
            var name = Console.ReadLine();
            var serviceName = name.ToLower().Replace(" ", "-");
            if (name.Length < 2)
            {
                Console.WriteLine("Le nom du serveur doit faire au minimum deux caractères");
                return;
            }

            var apps = GetAppsAsList();
            if (apps == null)
                apps = new List<Application>();

            //Vérification si l'application existe
            foreach (var app in apps)
            {
                if (app.DisplayName == name || app.ServiceName == serviceName || File.Exists("/etc/systemd/system/" + serviceName + ".service") || File.Exists("/usr/lib/systemd/system/" + serviceName + ".service"))
                {
                    Console.WriteLine("L'application existe déjà");
                    return;
                }
            }

            var workingDir = "/home/eclipium-server-manager/applications/" + serviceName;

            bool? isRoot = null;
            while (isRoot == null)
            {
                Console.Write("L'application nécessite-elle des droits Root [o/N]");
                var result = Console.ReadKey();
                if (result.Key == ConsoleKey.Enter || result.Key == ConsoleKey.N)
                    isRoot = false;
                else if (result.Key == ConsoleKey.O)
                    isRoot = true;
                Console.Write("\n");
            }

            string startCommand = null;
            string type = null;
            while (type == null || startCommand == null)
            {
                Console.WriteLine("Quel est le type de l'application ?");
                Console.WriteLine("1) NodeJS");
                Console.WriteLine("2) Python");
                Console.WriteLine("3) .NET 3.1/5/6");
                Console.WriteLine("4) GoLang");
                Console.WriteLine("5) Java");
                var result = Console.ReadKey();
                Console.Write("\n");
                string fileName;
                switch (result.Key)
                {
                    case ConsoleKey.D1:
                    case ConsoleKey.NumPad1:
                        type = "NodeJS";
                        Console.Write("Quel est le nom du fichier à lancer [index.js] : ");
                        fileName = Console.ReadLine();
                        if (String.IsNullOrEmpty(fileName.Replace(" ", "")))
                        {
                            fileName = "index.js";
                        }

                        startCommand = "npm i && node " + fileName;
                        break;
                    case ConsoleKey.D2:
                    case ConsoleKey.NumPad2:
                        type = "Python";
                        Console.Write("Quel est le nom du fichier à lancer [main.py] : ");
                        fileName = Console.ReadLine();
                        if (String.IsNullOrEmpty(fileName.Replace(" ", "")))
                        {
                            fileName = "main.py";
                        }

                        startCommand = "python " + fileName;
                        Console.WriteLine(
                            "Vous devrez peut être devoir installer des packages via le gestionnaire de paquets pip.");
                        break;
                    case ConsoleKey.D3:
                    case ConsoleKey.NumPad3:
                        type = "DotNet";
                        Console.Write("Quel est le nom du fichier à lancer [Program.dll] : ");
                        fileName = Console.ReadLine();
                        if (String.IsNullOrEmpty(fileName.Replace(" ", "")))
                        {
                            fileName = "Program.dll";
                        }

                        startCommand = "dotnet " + fileName;
                        break;
                    case ConsoleKey.D4:
                    case ConsoleKey.NumPad4:
                        type = "GoLang";
                        Console.Write("Quel est le nom du fichier à lancer [main.go] : ");
                        fileName = Console.ReadLine();
                        if (String.IsNullOrEmpty(fileName.Replace(" ", "")))
                        {
                            fileName = "main.go";
                        }

                        startCommand = "go run " + fileName;
                        break;
                    case ConsoleKey.D5:
                    case ConsoleKey.NumPad5:
                        type = "Java";
                        Console.Write("Quel est le nom du fichier à lancer [main.jar] : ");
                        fileName = Console.ReadLine();
                        if (String.IsNullOrEmpty(fileName.Replace(" ", "")))
                        {
                            fileName = "main.jar";
                        }

                        startCommand = "java -jar " + fileName;
                        break;
                }
            }

            Application application = new(name, serviceName, workingDir, type, (bool) isRoot);

            //Création du service
            var asm = Assembly.GetExecutingAssembly();
            string resourceName = asm.GetManifestResourceNames()
                .Single(str => str.EndsWith("Template.service"));
            using (var resource = asm.GetManifestResourceStream(resourceName))
            {
                string template = Encoding.UTF8.GetString(ReadFully(resource));
                string service = template
                    .Replace("{user}", application.NeedsRoot
                        ? "root"
                        : "eclipium-server-manager")
                    .Replace("{workingDir}", workingDir)
                    .Replace("{command}", startCommand);
                File.WriteAllText(("/etc/systemd/system/" + serviceName + ".service"), service);
            }

            RunCommandWithBash("mkdir " + workingDir);
            
            apps.Add(application);
            File.WriteAllText("/home/eclipium-server-manager/applications/.apps", JsonSerializer.Serialize(apps));
            RunCommandWithBash("systemctl enable " + serviceName);
            Console.WriteLine("L'application a été crée, veuillez vous rendre dans le dossier de l'application a l'aide de la commande\n "
                              + "'cd " + workingDir + "' pour y mettre les fichiers de l'application\n" 
                              +"puis démarrez-la à l'aide de la commande 'esm start " + serviceName + "'");
        }

        private static List<Application> GetAppsAsList()
        {
            var file = File.ReadAllText("/home/eclipium-server-manager/applications/.apps");
            var apps = JsonSerializer.Deserialize<List<Application>>(file);
            return apps;
        }
        
        private static byte[] ReadFully(Stream input)
        {
            byte[] buffer = new byte[16 * 1024];
            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }

                return ms.ToArray();
            }
        }

        private static string RunCommandWithBash(string command)
        {
            var psi = new ProcessStartInfo();
            psi.FileName = "/bin/bash";
            psi.Arguments = "-c \"" + command + "\"";
            psi.RedirectStandardOutput = true;
            psi.UseShellExecute = false;
            psi.CreateNoWindow = true;

            using var process = Process.Start(psi);

            process.WaitForExit();

            var output = process.StandardOutput.ReadToEnd();

            return output;
        }
        
        private static string RunCommandWithSystemD(string command)
        {
            var psi = new ProcessStartInfo();
            psi.FileName = "/bin/systemctl";
            psi.Arguments = command;
            psi.RedirectStandardOutput = true;
            psi.UseShellExecute = false;
            psi.CreateNoWindow = true;

            using var process = Process.Start(psi);

            process.WaitForExit();

            var output = process.StandardOutput.ReadToEnd();

            return output;
        }


    }
}