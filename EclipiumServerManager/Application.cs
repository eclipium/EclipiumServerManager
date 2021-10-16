namespace EclipiumServerManager
{
    public class Application
    {
        public string DisplayName { get; }
        public string ServiceName { get; }
        public string Directory { get; }
        public string Type { get; }
        public bool NeedsRoot { get; }

        public Application(string displayName, string serviceName, string directory, string type, bool needsRoot)
        {
            DisplayName = displayName;
            ServiceName = serviceName;
            Directory = directory;
            Type = type;
            NeedsRoot = needsRoot;
        }
    }
}