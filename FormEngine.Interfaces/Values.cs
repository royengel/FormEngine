using System;
using System.Collections.Generic;

namespace FormEngine.Interfaces
{
    public class Values : IValues
    {
        private readonly Dictionary<string, object> values;
        private string dateFormat = null;

        public Values(Dictionary<string, object> values)
        {
            this.values = values;
        }

        public override string Get(string valueName)
        {
            object o;
            if(values.TryGetValue(valueName, out o))
            {
                if (!string.IsNullOrWhiteSpace(dateFormat) && o.GetType().ToString() == "DateTime")
                    return ((DateTime)o).ToString(dateFormat);
                else
                    return o.ToString();
            }
            return "";
        }
    }
}
