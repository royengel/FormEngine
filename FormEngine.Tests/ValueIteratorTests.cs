using System;
using System.Collections.Generic;
using FormEngine.Interfaces;
using Xunit;

namespace FormEngine.Tests
{
    public class ValueIteratorTests
    {
        [Fact]
        public void ValueIterator_OnFirstRow_IsBreak()
        {
            ValueIterator checker = new ValueIterator();
            checker.IterateTo(new Values(new Dictionary<string, object> { { "v1", "1" } }));
            Assert.True(checker.IsBreak(new List<Func<dynamic, object>>() { b => b.v1 }, true));
        }
        [Fact]
        public void ValueIterator_OnFirstRow_IsNoFooterBreak()
        {
            ValueIterator checker = new ValueIterator();
            checker.IterateTo(new Values(new Dictionary<string, object> { { "v1", "1" } }));
            Assert.False(checker.IsBreak(new List<Func<dynamic, object>>() { b => b.v1 }, false));
        }
        [Fact]
        public void ValueIterator_TwoEqualRowsRow_IsNotBreak()
        {
            ValueIterator checker = new ValueIterator();
            checker.IterateTo(new Values(new Dictionary<string, object> { { "v1", "1" }, { "v2", "x" } }));
            checker.IterateTo(new Values(new Dictionary<string, object> { { "v1", "1" }, { "v2", "y" } }));
            Assert.False(checker.IsBreak(new List<Func<dynamic, object>>() { b => b.v1 }, true));
            Assert.True(checker.IsBreak(new List<Func<dynamic, object>>() { b => b.v2 }, true));
        }
        [Fact]
        public void ValueIterator_NoBreakColumns_IsNotBreak()
        {
            ValueIterator checker = new ValueIterator();
            checker.IterateTo(new Values(new Dictionary<string, object> { { "v1", "1" } }));
            checker.IterateTo(new Values(new Dictionary<string, object> { { "v1", "1" } }));
            Assert.False(checker.IsBreak(new List<Func<dynamic, object>>(), true));
            Assert.False(checker.IsBreak(null, true));
        }
    }
}
