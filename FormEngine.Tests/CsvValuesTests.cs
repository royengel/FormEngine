using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FormEngine.Tests.Helpers;
using FormEngine.Interfaces;
using FormEngine.CsvValues;

namespace FormEngine.Tests
{
    [TestClass]
    public class CsvValuesTests
    {
        [TestMethod]
        public void CsvValues_Basics()
        {
            CsvFile csvFile = MakeCsvFile(@"v1;v2
a;b
c;d
");
            IEnumerable<IValues> values = csvFile.GetValues();
            IEnumerator<IValues> iterator = values.GetEnumerator();
            Assert.IsTrue(iterator.MoveNext());
            Assert.AreEqual("a", iterator.Current.Get("v1"));
            Assert.AreEqual("b", iterator.Current.Get("v2"));
            Assert.IsTrue(iterator.MoveNext());
            Assert.AreEqual("c", iterator.Current.Get("v1"));
            Assert.AreEqual("d", iterator.Current.Get("v2"));
            Assert.IsFalse(iterator.MoveNext());
        }

        private static CsvFile MakeCsvFile(string content)
        {
            IFiles files = new FakeFiles();
            files = new FakeFiles() { textFiles = { { "values.csv", content } } };
            return new CsvFile(files, "values.csv");
        }
    }
}
