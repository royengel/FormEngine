using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using FormEngine.Interfaces;

namespace FormEngine.FileSystem
{
    public class Folder : IFiles
    {
        string _path;

        public Folder(string path)
        {
            _path = path;
        }

        public byte[] Get(string name)
        {
            if (name.IndexOf(Path.DirectorySeparatorChar) >= 0 || name.IndexOf(Path.AltDirectorySeparatorChar) >= 0)
                return File.ReadAllBytes(name);
            return File.ReadAllBytes(_path + Path.DirectorySeparatorChar + name);
        }
    }
}
