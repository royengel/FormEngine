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

        public override string Get(string valueName)
        {
            string[] header = file.GetHeaderRow();

            if (header == null || row == null)
                return "";

            for(int i = 0; i <= row.GetUpperBound(0) && i <= header.GetUpperBound(0); i++)
            {
                if (header[i] == valueName.ToLower().Trim())
                    return row[i].Trim();
            }

            return "";
        }
    }
}
