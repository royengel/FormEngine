using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormEngine.Interfaces
{
    public interface IFormBuilder
    {
        IFormPage AddPage(PageSize pageSize);
        bool Finalize();
    }
}
