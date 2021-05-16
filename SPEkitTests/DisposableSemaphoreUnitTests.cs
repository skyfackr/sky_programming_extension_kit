using Microsoft.VisualStudio.TestTools.UnitTesting;
// ReSharper disable CheckNamespace

namespace SPEkit.DisposableSemaphoreSlim.Tests
{
    [TestClass]
    public class DisposableSemaphoreUnitTests
    {
        private static DisposableSemaphoreUnit _getInstance()
        {
            return new DisposableSemaphoreSlim().Wait();
        }

        [TestMethod]
        public void DisposeTest()
        {
            var a = _getInstance();
            Assert.IsFalse(a.IsReleased);
            var b = a.Factory.CurrentCount;
            a.Dispose();
            Assert.IsTrue(a.IsReleased);
            Assert.AreEqual(b + 1, a.Factory.CurrentCount);
        }

        [TestMethod]
        public void ReleaseTest()
        {
            var a = _getInstance();
            Assert.IsFalse(a.IsReleased);
            var b = a.Factory.CurrentCount;
            a.Release();
            Assert.IsTrue(a.IsReleased);
            Assert.AreEqual(b + 1, a.Factory.CurrentCount);
            
                
        }

        
    }
}