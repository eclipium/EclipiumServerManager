namespace EclipiumServerManager;

public static class AutostartManage
{
    public static void EnableApp(string[] args)
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
                var status = ProcessManager.RunCommandWithSystemD("enable " + app.ServiceName);
                Console.WriteLine(string.IsNullOrEmpty(status)
                    ? "L'application est déjà activée."
                    : "L'application a bien été activée au démarrage du système.");

                return;
            }
        }

        Console.WriteLine("L'application n'existe pas");
    }

    public static void DisableApp(string[] args)
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
                var status = ProcessManager.RunCommandWithSystemD("disable " + app.ServiceName);
                Console.WriteLine(string.IsNullOrEmpty(status)
                    ? "L'application est déjà désactivée."
                    : "L'application a bien été désactivée au démarrage du système.");

                return;
            }
        }

        Console.WriteLine("L'application n'existe pas");
    }
}