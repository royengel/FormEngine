using System;

namespace FormEngine.Interfaces
{
    public interface IFiles
    {
        byte[] Get(string name);
        string GetText(string name);
    }
}
