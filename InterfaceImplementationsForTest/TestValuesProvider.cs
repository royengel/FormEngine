using System;
using System.Collections.Generic;
using FormEngine.Interfaces;

namespace InterfaceImplementationsForTest
{
    public class TestValuesProvider : IValuesProvider
    {
        public IEnumerable<IValues> GetValues(IFiles files, string valueKey)
        {
            return new List<IValues>() { new Values(new Dictionary<string, object>
                    {
                        { "test1", "value1" },
                        { "test2", "value2" }
                    } ) };
        }
    }
}
