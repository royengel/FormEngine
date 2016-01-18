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
            IEnumerable<IValues> values = MakeCsvFile(@"V1;v2
a;b
c;d
");
            IEnumerator<IValues> iterator = values.GetEnumerator();
            Assert.IsTrue(iterator.MoveNext());
            Assert.AreEqual("a", iterator.Current.Get("v1"));
            Assert.AreEqual("b", iterator.Current.Get("V2"));
            Assert.IsTrue(iterator.MoveNext());
            Assert.AreEqual("c", iterator.Current.Get("v1"));
            Assert.AreEqual("d", iterator.Current.Get("v2"));
            Assert.IsFalse(iterator.MoveNext());
        }

        private static IEnumerable<IValues> MakeCsvFile(string content)
        {
            IResources files = new FakeFiles();
            files = new FakeFiles() { textFiles = { { "values.csv", content } } };

            return new CsvFile().GetValues(files, "values.csv");
        }
    }
}
