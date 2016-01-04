using FormEngine.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Collections;
using System.IO;

namespace FormEngine.XmlSpecification
{
    public class XmlValues : IValues
    {
        private XmlDocument _valuesDoc = null;
        private string _exception = null;
        private static int _noSampleRows = 3;

        public XmlValues(IFiles Files, string valuesXmlFile)
        {
            _valuesDoc = LoadXmlFile(Files, valuesXmlFile);
        }

        public static XmlDocument LoadXmlFile(IFiles Files, string xmlFile)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(new MemoryStream(Files.Get(xmlFile)));
            return xmlDoc;
        }

        public string GetValue(string tableName, int row, string valueName, string format = null)
        {
            if (string.IsNullOrEmpty(tableName))
                return GetValue(valueName);
            else
                return GetValue(tableName + "." + valueName) + " " + (row + 1).ToString();
        }

        private string GetValue(string valueName)
        {
            string Value = "";
            XmlElement valueElem = (XmlElement)_valuesDoc.DocumentElement.SelectSingleNode("descendant::Field[@FieldName='" + valueName + "']");
            if (valueElem != null)
            {
                if (valueElem.Attributes["Value"] != null)
                    Value = valueElem.Attributes["Value"].Value;
                else if (valueElem.Attributes["TestValue"] != null)
                    Value = valueElem.Attributes["TestValue"].Value;
            }
            return Value;
        }

        public int TableRowCount(string tableName)
        {
            if (string.IsNullOrEmpty(tableName))
                return 1;
            else
                return _noSampleRows;
        }

        public string Get(string valueName, string format = null)
        {
            throw new NotImplementedException();
        }

        public IEnumerator GetEnumerator()
        {
            throw new NotImplementedException();
        }

        public string Exception
        {
            get
            {
                if (_exception != null)
                    return _exception;

                return GetValue(null, 0, "exception");
            }
            set
            {
                _exception = value;
            }
        }
    }
}
