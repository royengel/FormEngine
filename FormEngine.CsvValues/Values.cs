using FormEngine.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormEngine.CsvValues
{
    class Values : IValues
    {
        private CsvFile file;
        private string[] row;

        internal Values(CsvFile file, string[] row)
        {
            this.file = file;
            this.row = row;
        }

        public string Get(string valueName, string format = null)
        {
            if (file.GetHeaderRow() == null || row == null)
                return "";

            for(int i = 0; i <= row.GetUpperBound(0) && i <= file.GetHeaderRow().GetUpperBound(0); i++)
            {
                if (file.GetHeaderRow()[i].ToLower().Trim() == valueName)
                    return row[i];
            }

            return "";
        }
    }
}
