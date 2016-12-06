using System;
using System.Dynamic;
using System.Linq;

namespace FormEngine.Interfaces
{
    public class TestValues : DynamicObject
    {
        private Form form;

        public TestValues(Form form)
        {
            this.form = form;
        }

        public override bool TryGetMember(GetMemberBinder binder,
                                  out object result)
        {
            result = null;
            foreach (Report p in form.reports)
            {
                foreach(Section s in p.sections)
                {
                    foreach(Field f in s.fields)
                    {
                        if (f.name == binder.Name)
                            result = f.testValue.Trim();
                    }
                }
            }
            return result == null ? false : true;
        }
    }
}