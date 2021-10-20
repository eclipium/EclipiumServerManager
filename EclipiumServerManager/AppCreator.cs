using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;

namespace EclipiumServerManager
{
    public static class AppCreator
    {
        public static void CreateApp()
        {
            string name = null;
            while (string.IsNullOrEmpty(name))
            {
                Console.WriteLine("Quel est le nom de l'application à créer ?");
                name = Console.ReadLine();
            }
            var serviceName = name.ToLower().Replace(" ", "-");

            var apps = AppsInformations.GetAppsAsList() ?? new List<Application>();

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
                if (result.Key is ConsoleKey.Enter or ConsoleKey.N)
                    isRoot = false;
                else if (result.Key == ConsoleKey.O)
                    isRoot = true;
                Console.Write("\n");
            }

            string startCommand = null;
            string type = null;
            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
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
                        if (string.IsNullOrEmpty(fileName) || string.IsNullOrWhiteSpace(fileName))
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
                        if (string.IsNullOrEmpty(fileName) || string.IsNullOrWhiteSpace(fileName))
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
                        if (string.IsNullOrEmpty(fileName) || string.IsNullOrWhiteSpace(fileName))
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
                        if (string.IsNullOrEmpty(fileName) || string.IsNullOrWhiteSpace(fileName))
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
                        if (string.IsNullOrEmpty(fileName) || string.IsNullOrWhiteSpace(fileName))
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
            var resourceName = asm.GetManifestResourceNames()
                .Single(str => str.EndsWith("Template.service"));
            using (var resource = asm.GetManifestResourceStream(resourceName))
            {
                var template = Encoding.UTF8.GetString(ResourceReader.ReadFully(resource));
                var service = template
                    .Replace("{user}", application.NeedsRoot
                        ? "root"
                        : "eclipium-server-manager")
                    .Replace("{workingDir}", workingDir)
                    .Replace("{command}", startCommand);
                File.WriteAllText(("/etc/systemd/system/" + serviceName + ".service"), service);
            }

            ProcessManager.RunCommandWithBash("mkdir " + workingDir);
            
            apps.Add(application);
            File.WriteAllText("/home/eclipium-server-manager/applications/.apps", JsonSerializer.Serialize(apps));
            ProcessManager.RunCommandWithBash("systemctl enable " + serviceName);
            Console.WriteLine("L'application a été créée, veuillez vous rendre dans le dossier de l'application a l'aide de la commande\n "
                              + "'cd " + workingDir + "' pour y mettre les fichiers de l'application\n" 
                              +"puis démarrez-la à l'aide de la commande 'esm start " + serviceName + "'");
        }
    }
}