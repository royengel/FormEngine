using FormEngine.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace FormEngine
{
    public static class ClassFactory<T>
    {
        public static T Instanciate(string dllName = "", string className = "")
        {
            Type interfaceType = typeof(T);
            List<Assembly> assemblies = AppDomain.CurrentDomain.GetAssemblies()
                .Where(a => a.GetName().Name.ToLower().Contains(dllName.ToLower())
                    && a.GetTypes().FirstOrDefault(c => interfaceType.IsAssignableFrom(c)) != null).ToList();

            

            Type implementationType = assemblies
                .SelectMany(s => s.GetTypes())
                .FirstOrDefault(c => interfaceType.IsAssignableFrom(c) && c.Name.ToLower().Contains(className.ToLower()));

            return (T)Activator.CreateInstance(implementationType);
        }
    }
}
