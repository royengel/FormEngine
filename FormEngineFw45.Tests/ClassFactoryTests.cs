using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FormEngine.Interfaces;

namespace FormEngine.Tests
{
    [TestClass]
    public class ClassFactoryTests
    {
        [TestMethod]
        public void ClassFactory_WhenPartialDllName_ReturnAnyImplementationWithinThatDll()
        {
            IValuesProvider provider = ClassFactory<IValuesProvider>.Instanciate("Test");
            Assert.AreEqual("TestValuesProvider", provider.GetType().Name);
        }

        [TestMethod]
        public void ClassFactory_WhenPartialClassName_ReturnImplementationWithinAnyDll()
        {
            IValuesProvider provider = ClassFactory<IValuesProvider>.Instanciate("", "TestValues");
            Assert.AreEqual("TestValuesProvider", provider.GetType().Name);
        }

        [TestMethod]
        public void ClassFactory_WhenNothingSpecified_ReturnNull()
        {
            IValuesProvider provider = ClassFactory<IValuesProvider>.Instanciate("", "");
            Assert.IsNull(provider);
        }

        [TestMethod]
        public void ClassFactory_WhenUnknownDll_ReturnNull()
        {
            IValuesProvider provider = ClassFactory<IValuesProvider>.Instanciate("xyz", "");
            Assert.IsNull(provider);
        }

        [TestMethod]
        public void ClassFactory_WhenUnknownClass_ReturnNull()
        {
            IValuesProvider provider = ClassFactory<IValuesProvider>.Instanciate("xyz", "");
            Assert.IsNull(provider);
        }
    }
}
