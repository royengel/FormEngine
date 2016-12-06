using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FormEngine;
using FormEngine.Interfaces;

namespace FormEngine.Tests
{
    [TestClass]
    public class ValueIteratorTests
    {
        [TestMethod]
        public void ValueIterator_OnFirstRow_IsBreak()
        {
            ValueIterator checker = new ValueIterator();
            checker.IterateTo(new Values(new Dictionary<string, object> { { "v1", "1" } }));
            Assert.IsTrue(checker.IsBreak(new List<Func<dynamic, object>>() { b => b.v1 }, true));
        }
        [TestMethod]
        public void ValueIterator_OnFirstRow_IsNoFooterBreak()
        {
            ValueIterator checker = new ValueIterator();
            checker.IterateTo(new Values(new Dictionary<string, object> { { "v1", "1" } }));
            Assert.IsFalse(checker.IsBreak(new List<Func<dynamic, object>>() { b => b.v1 }, false));
        }
        [TestMethod]
        public void ValueIterator_TwoEqualRowsRow_IsNotBreak()
        {
            ValueIterator checker = new ValueIterator();
            checker.IterateTo(new Values(new Dictionary<string, object> { { "v1", "1" }, { "v2", "x" } }));
            checker.IterateTo(new Values(new Dictionary<string, object> { { "v1", "1" }, { "v2", "y" } }));
            Assert.IsFalse(checker.IsBreak(new List<Func<dynamic, object>>() { b => b.v1 }, true));
            Assert.IsTrue(checker.IsBreak(new List<Func<dynamic, object>>() { b => b.v2 }, true));
        }
        [TestMethod]
        public void ValueIterator_NoBreakColumns_IsNotBreak()
        {
            ValueIterator checker = new ValueIterator();
            checker.IterateTo(new Values(new Dictionary<string, object> { { "v1", "1" } }));
            checker.IterateTo(new Values(new Dictionary<string, object> { { "v1", "1" } }));
            Assert.IsFalse(checker.IsBreak(new List<Func<dynamic, object>>(), true));
            Assert.IsFalse(checker.IsBreak(null, true));
        }
    }
}
