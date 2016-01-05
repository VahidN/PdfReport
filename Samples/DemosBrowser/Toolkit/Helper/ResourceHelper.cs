using System.IO;
using System.Reflection;

namespace DemosBrowser.Toolkit.Helper
{
    public static class ResourceHelper
    {
        public static string GetInputFile(string filename)
        {
            var thisAssembly = Assembly.GetExecutingAssembly();
            var stream = thisAssembly.GetManifestResourceStream(filename);
            return new StreamReader(stream).ReadToEnd();
        }
    }
}