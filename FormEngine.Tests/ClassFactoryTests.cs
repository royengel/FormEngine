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
            Assert.AreEqual(provider.GetType().Name, "TestValuesProvider");
        }

        [TestMethod]
        public void ClassFactory_WhenPartialClassName_ReturnImplementationWithinAnyDll()
        {
            IValuesProvider provider = ClassFactory<IValuesProvider>.Instanciate("", "TestValues");
            Assert.AreEqual(provider.GetType().Name, "TestValuesProvider");
        }
    }
}
