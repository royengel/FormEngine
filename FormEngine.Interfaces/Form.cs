using System.Collections.Generic;

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
