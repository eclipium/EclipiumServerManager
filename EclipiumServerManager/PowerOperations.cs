namespace EclipiumServerManager;

public static class PowerOperations
{
    public static void StartApp(string[] args)
    {
        if (args.Length != 2)
        {
            Console.WriteLine("Veuillez spécifier une application");
            return;
        }

        var apps = AppsInformations.GetAppsAsList();
        foreach (var app in apps)
        {
            if (app.DisplayName == args[1] || app.ServiceName == args[1])
            {
                var status = ProcessManager.RunCommandWithSystemD("start " + app.ServiceName);
                if (string.IsNullOrEmpty(status))
                {
                    Console.WriteLine("L'application a bien été démarrée.");
                }
                else
                {
                    Console.WriteLine("L'application n'a pas été démarrée, faites 'esm log " + app.ServiceName +
                                      "' pour plus d'informations.");
                }

                return;
            }
        }

        Console.WriteLine("L'application n'existe pas");
    }

    public static void RestartApp(string[] args)
    {
        if (args.Length != 2)
        {
            Console.WriteLine("Veuillez spécifier une application");
            return;
        }

        var apps = AppsInformations.GetAppsAsList();
        foreach (var app in apps)
        {
            if (app.DisplayName == args[1] || app.ServiceName == args[1])
            {
                var status = ProcessManager.RunCommandWithSystemD("restart " + app.ServiceName);
                if (string.IsNullOrEmpty(status))
                {
                    Console.WriteLine("L'application a bien été démarrée.");
                }
                else
                {
                    Console.WriteLine("L'application n'a pas été démarrée, faites 'esm log " + app.ServiceName +
                                      "' pour plus d'informations.");
                }

                return;
            }
        }

        Console.WriteLine("L'application n'existe pas");
    }


    public static void StopApp(string[] args)
    {
        if (args.Length != 2)
        {
            Console.WriteLine("Veuillez spécifier une application");
            return;
        }

        var apps = AppsInformations.GetAppsAsList();
        foreach (var app in apps)
        {
            if (app.DisplayName == args[1] || app.ServiceName == args[1])
            {
                var status = ProcessManager.RunCommandWithSystemD("stop " + app.ServiceName);
                if (string.IsNullOrEmpty(status))
                {
                    Console.WriteLine("L'application a bien été arrêtée.");
                }
                else
                {
                    Console.WriteLine("L'application n'a pas été arrêtée, faites 'esm log " + app.ServiceName +
                                      "' pour plus d'informations.");
                }

                return;
            }
        }

        Console.WriteLine("L'application n'existe pas");
    }

    public static void KillApp(string[] args)
    {
        if (args.Length != 2)
        {
            Console.WriteLine("Veuillez spécifier une application");
            return;
        }

        var apps = AppsInformations.GetAppsAsList();
        foreach (var app in apps)
        {
            if (app.DisplayName == args[1] || app.ServiceName == args[1])
            {
                var status = ProcessManager.RunCommandWithSystemD("kill " + app.ServiceName);
                if (string.IsNullOrEmpty(status))
                {
                    Console.WriteLine("L'application a bien été tuée.");
                }
                else
                {
                    Console.WriteLine("L'application n'a pas été tuée, faites 'esm log " + app.ServiceName +
                                      "' pour plus d'informations.");
                }

                return;
            }
        }

        Console.WriteLine("L'application n'existe pas");
    }
}