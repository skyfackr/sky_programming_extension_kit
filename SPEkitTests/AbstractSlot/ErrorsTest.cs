using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SPEkit.SemaphoreSlimAttribute.Tests
{
    [TestClass]
    public partial class AbstractSlotTests
    {
        [TestMethod]
        [Timeout(2000)]
        public void ErrorsTest()
        {
            Assert.ThrowsException<NotInitializedException>(ASNotInitializedFunc);
        }
    }
}