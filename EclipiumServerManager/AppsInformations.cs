namespace EclipiumServerManager;

public static class AppsInformations
{
    public static void ListApps()
    {
        Console.WriteLine("\nListe des applications :");
        var table = new ConsoleTable("Nom", "Service", "Type", "Statut", "Démarrage auto", "Utilisation RAM",
            "Dossier d'exécution");

        var apps = GetAppsAsList();
        foreach (var app in apps)
        {
            var status = ProcessManager.RunCommandWithSystemD("status " + app.ServiceName);
            var regex = new Regex(@"(?<=Active:[\s])([\S])*");
            switch (regex.Match(status).Value)
            {
                case "inactive":
                    table.AddRow(app.DisplayName, app.ServiceName, app.Type, "Arrêtée",
                        status.Contains("enabled") ? "Oui" : "Non", "0K", app.Directory);
                    break;
                case "failed":
                    table.AddRow(app.DisplayName, app.ServiceName, app.Type, "Crash",
                        status.Contains("enabled") ? "Oui" : "Non", "0K", app.Directory);
                    break;
                case "active":
                    table.AddRow(app.DisplayName, app.ServiceName, app.Type, "En cours",
                        status.Contains("enabled") ? "Oui" : "Non",
                        (new Regex(@"(?<=Memory:[\s])([\S])*")).Match(status).Value, app.Directory);
                    break;
                default:
                    table.AddRow(app.DisplayName, app.ServiceName, app.Type, "Inconnu",
                        status.Contains("enabled") ? "Oui" : "Non", "0K", app.Directory);
                    break;
            }
        }

        table.Write();
        Console.WriteLine();
    }

    public static void GetAppStatus(string[] args)
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
                var status = ProcessManager.RunCommandWithSystemD("status " + app.ServiceName);
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

    public static void GetAppLog(string[] args)
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
                var logs = ProcessManager.RunCommandWithBash("journalctl -u " + app.ServiceName + " -n 30 --no-pager");
                Console.Write(logs);
                return;
            }
        }

        Console.WriteLine("L'application n'existe pas");
    }

    public static List<Application> GetAppsAsList()
    {
        var file = File.ReadAllText("/home/eclipium-server-manager/applications/.apps");
        var apps = JsonSerializer.Deserialize<List<Application>>(file);
        return apps;
    }
}