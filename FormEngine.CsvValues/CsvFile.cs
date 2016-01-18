using FormEngine.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormEngine.CsvValues
{
    public class CsvFile : IValuesProvider
    {
        private IResources files;
        private string fileName;
        private string[] headerRow = null;
        private List<Values> values;

        //public CsvFile(IFiles files, string fileName)
        //{
        //    this.files = files;
        //    this.fileName = fileName;
        //}
        public IEnumerable<IValues> GetValues(IResources files, string valueKey)
        {
            this.files = files;
            this.fileName = valueKey;
            return GetValues();
        }

        private IEnumerable<IValues> GetValues()
        {
            InitValues();
            return values;
        }

        private void InitValues()
        {
            if (headerRow == null)
            {
                string fileContent = files.GetText(fileName).Replace(Environment.NewLine, "\n");
                string[] rows = fileContent.Split('\n');
                values = new List<Values>();
                foreach (string row in rows)
                {
                    if (string.IsNullOrWhiteSpace(row))
                        continue;

                    if (headerRow == null)
                        headerRow = row.ToLower().Split(';');
                    else
                        values.Add(new Values(this, row.Split(';')));
                }

                for (int i = 0; i <= headerRow.GetUpperBound(0); i++)
                    headerRow[i] = headerRow[i].Trim();
            }
        }

        internal string[] GetHeaderRow()
        {
            InitValues();
            return headerRow;
        }
    }
}
