using FormEngine.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormEngine.Tests.Helpers
{
    public class FakeFormBuilder : IFormBuilder
    {
        public List<FakeFormPage> pages = new List<FakeFormPage>();
        public IFormPage AddPage(PageSize pageSize)
        {
            pages.Add(new FakeFormPage(pageSize.ToString()));
            return pages.Last();
        }

        public bool Finalize()
        {
            return true;
        }
    }
}
