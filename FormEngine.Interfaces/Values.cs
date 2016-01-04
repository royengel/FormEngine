using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormEngine.Interfaces
{
    public class Values : IValues
    {
        private readonly Dictionary<string, object> values;

        public Values(Dictionary<string, object> values)
        {
            this.values = values;
        }

        public string Get(string valueName, string format = null)
        {
            object o;
            if(values.TryGetValue(valueName, out o))
            {
                if (!string.IsNullOrWhiteSpace(format) && o.GetType().ToString() == "DateTime")
                    return ((DateTime)o).ToString(format);
                else
                    return o.ToString();
            }
            return "";
        }
    }
}
