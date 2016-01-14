using FormEngine.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormEngine.CsvValues
{
    public class CsvFile
    {
        private IFiles files;
        private string fileName;
        private string[] headerRow;

        public CsvFile(IFiles files, string fileName)
        {
            this.files = files;
            this.fileName = fileName;
        }

        public IEnumerable<IValues> GetValues()
        {
            headerRow = null;
            string fileContent = files.GetText(fileName).Replace(Environment.NewLine, "\n");
            string[] rows = fileContent.Split('\n');
            List<Values> values = new List<Values>();
            foreach(string row in rows)
            {
                if (string.IsNullOrWhiteSpace(row))
                    continue;

                if (headerRow == null)
                    headerRow = row.Split(';');
                else
                    values.Add(new Values(this, row.Split(';')));
            }
            return values;
        }

        internal string[] GetHeaderRow()
        {
            return headerRow;
        }
    }
}
