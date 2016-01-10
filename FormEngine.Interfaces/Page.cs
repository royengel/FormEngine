using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormEngine.Interfaces
{
    public class Page : FieldProperties
    {
        public string backgroundImage;
        public PageSize pageSize;
        public List<Section> sections;
    }
}
