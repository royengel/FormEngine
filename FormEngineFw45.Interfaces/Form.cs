using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormEngine.Interfaces
{
    public class Form : FieldProperties
    {
        public string formTitle;
        public PageSize pageSize;
        public List<Report> reports;

        public IEnumerable<dynamic> TestValues()
        {
            return new List<dynamic>() { new TestValues(this) };
        }
    }
}
