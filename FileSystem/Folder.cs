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
            return File.ReadAllBytes(Path.Combine(_path, name));
        }

        public string GetText(string name)
        {
            return File.ReadAllText(Path.Combine(_path, name));
        }
    }
}
