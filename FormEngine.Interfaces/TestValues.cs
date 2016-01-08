using System;
using System.Linq;

namespace FormEngine.Interfaces
{
    public class TestValues : IValues
    {
        private Form form;

        public TestValues(Form form)
        {
            this.form = form;
        }

        public string Get(string valueName, string format = null)
        {
            foreach(Page p in form.pages)
            {
                foreach(Section s in p.sections)
                {
                    foreach(Field f in s.fields)
                    {
                        if (f.name == valueName)
                            return f.testValue;
                    }
                }
            }
            return "";
        }
    }
}