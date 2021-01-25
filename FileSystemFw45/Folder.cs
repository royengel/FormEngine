using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using FormEngine.Interfaces;

namespace FormEngine.FileSystem
{
    public class Folder : IResources
    {
        string _path;

        public Folder(string path)
        {
            _path = path;
        }

        public byte[] Get(string name)
        {
            try
            {
                return File.ReadAllBytes(Path.Combine(_path, name));
            }
            catch(Exception ex)
            {
                throw new Exception(string.Format("Error reading file '{0}'", name), ex);
            }
        }

        public string GetText(string name)
        {
            try
            {
                return File.ReadAllText(Path.Combine(_path, name));
            }
            catch(Exception ex)
            {
                throw new Exception(string.Format("Error reading text file '{0}'", name), ex);
            }
        }
    }
}
