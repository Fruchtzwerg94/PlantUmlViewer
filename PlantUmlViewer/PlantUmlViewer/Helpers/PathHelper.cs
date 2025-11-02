using System.IO;
using System.Reflection;

namespace PlantUmlViewer.Helpers
{
    public static class PathHelper
    {
        private static readonly string assemblyDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        public static string ResolvePathToAssembly(string path)
        {
            if (Path.IsPathRooted(path))
            {
                return path;
            }
            else
            {
                return Path.GetFullPath(Path.Combine(assemblyDirectory, path));
            }
        }
    }
}
