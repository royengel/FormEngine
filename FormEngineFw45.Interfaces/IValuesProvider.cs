using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormEngine.Interfaces
{
    public interface IValuesProvider
    {
        IEnumerable<IValues> GetValues(IResources files, string valueKey);
    }
}
