using FormEngine.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace FormEngine
{
    public static class ClassFactory<T> where T : class
    {
        public static T Instanciate(string dllName = "", string className = "", params object[] constructorArgument)
        {
            if (string.IsNullOrWhiteSpace(dllName) && string.IsNullOrWhiteSpace(className))
                return null;

            List<Assembly> allAssemblies = new List<Assembly>();
            string path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string searchPattern = string.IsNullOrEmpty(dllName) ? "*.dll" : string.Format("*{0}*.dll", dllName.Trim());

            foreach (string dll in Directory.GetFiles(path, searchPattern))
                allAssemblies.Add(Assembly.LoadFile(dll));

            Type interfaceType = typeof(T);
            List<Assembly> assemblies = allAssemblies
                .Where(a => a.GetName().Name.ToLower().Contains(dllName.ToLower())
                    && a.GetTypes().FirstOrDefault(c => interfaceType.IsAssignableFrom(c)) != null).ToList();

            Type implementationType = assemblies
                .SelectMany(s => s.GetTypes())
                .FirstOrDefault(c => interfaceType.IsAssignableFrom(c) && c != interfaceType && c.Name.ToLower().Contains(className.ToLower()));

            if (implementationType == null)
                return null;

            return (T)Activator.CreateInstance(implementationType, constructorArgument);
        }
    }
}
