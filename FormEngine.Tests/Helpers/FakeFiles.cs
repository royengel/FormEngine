using FormEngine.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormEngine.Tests.Helpers
{
    public class FakeFiles : IResources
    {
        public readonly Dictionary<string, byte[]> files = new Dictionary<string, byte[]>();
        public readonly Dictionary<string, string> textFiles = new Dictionary<string, string>();

        public byte[] Get(string name)
        {
            return files[name];
        }

        public string GetText(string name)
        {
            return textFiles[name];
        }
    }
}
