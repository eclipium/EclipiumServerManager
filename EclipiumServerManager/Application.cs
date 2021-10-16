namespace EclipiumServerManager
{
    public class Application
    {
        public string DisplayName { get; set; }
        public string ServiceName { get; set; }
        public string Directory { get; set; }
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