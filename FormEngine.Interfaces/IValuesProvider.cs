using System.Collections.Generic;

namespace FormEngine.Interfaces
{
    public interface IValuesProvider
    {
        IEnumerable<IValues> GetValues(IResources files, string valueKey);
    }
}
