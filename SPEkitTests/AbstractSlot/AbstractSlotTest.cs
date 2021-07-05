using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssert;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SPEkit.BinLikeClassSelectors;

namespace SPEkit.SemaphoreSlimAttribute.Tests
{
    
    public partial class AbstractSlotTests
    {
        [TestMethod]
        [Timeout(300)]
        public void GetSelectorTests()
        {
            var se = GetType().GetMethod(nameof(ASMakeTimeout)).GetAbstractSlotAttribute();
            se.Option.SetTimeOut(1);
            try
            {
                ASMakeTimeout();
            }
            catch (WaitCancelledOrFailedException e)
            {
                e.Reasons.ShouldBeEqualTo(CancelFlag.Timeout);
                e.IsExecuted.ShouldBeFalse();
                var selector = e.Reasons.GetSelector();
                BinLikeClassSelector.CreateBinLikeClassSelectorUnit(Convert.ToInt64(e.Reasons))
                    .ShouldBeEqualTo(selector);
                selector.GetValidBinArray().ShouldBeEqualTo(new[]
                {
                    Convert.ToInt64(e.Reasons)
                });
            }
        }
    }
}
