using FormEngine.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormEngine.Tests.Helpers
{
    public class FakeFiles : IFiles
    {
        public readonly Dictionary<string, byte[]> files = new Dictionary<string, byte[]>();

        public byte[] Get(string name)
        {
            return files[name];
        }
    }
}
