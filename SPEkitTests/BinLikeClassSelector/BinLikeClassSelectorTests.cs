using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SPEkit.BinLikeClassSelectors.Tests
{
    [TestClass]
    public class BinLikeClassSelectorTests
    {
        [TestMethod]
        public void CreateBinLikeClassSelectorUnitTest()
        {
            Assert.IsInstanceOfType(BinLikeClassSelector.CreateBinLikeClassSelectorUnit(10010),
                typeof(BinLikeClassSelectorUnit));
            Assert.AreSame(BinLikeClassSelector.CreateBinLikeClassSelectorUnit(10010),
                BinLikeClassSelector.CreateBinLikeClassSelectorUnit(10010));
            Assert.AreNotSame(BinLikeClassSelector.CreateBinLikeClassSelectorUnit(10011),
                BinLikeClassSelector.CreateBinLikeClassSelectorUnit(100111));
        }
    }
}