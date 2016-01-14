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
    public class BreakCheckerTests
    {
        [TestMethod]
        public void BreakChecker_OnFirstRow_IsBreak()
        {
            BreakChecker checker = new BreakChecker();
            checker.IterateTo(new Values(new Dictionary<string, object> { { "v1", "1" } }));
            Assert.IsTrue(checker.IsBreak(new List<string>() { "v1" }));
        }
        [TestMethod]
        public void BreakChecker_TwoEqualRowsRow_IsNotBreak()
        {
            BreakChecker checker = new BreakChecker();
            checker.IterateTo(new Values(new Dictionary<string, object> { { "v1", "1" }, { "v2", "x" } }));
            checker.IterateTo(new Values(new Dictionary<string, object> { { "v1", "1" }, { "v2", "y" } }));
            Assert.IsFalse(checker.IsBreak(new List<string>() { "v1" }));
            Assert.IsTrue(checker.IsBreak(new List<string>() { "v2" }));
        }
        [TestMethod]
        public void BreakChecker_NoBreakColumns_IsBreak()
        {
            BreakChecker checker = new BreakChecker();
            checker.IterateTo(new Values(new Dictionary<string, object> { { "v1", "1" } }));
            checker.IterateTo(new Values(new Dictionary<string, object> { { "v1", "1" } }));
            Assert.IsTrue(checker.IsBreak(new List<string>()));
            Assert.IsTrue(checker.IsBreak(null));
        }
    }
}
