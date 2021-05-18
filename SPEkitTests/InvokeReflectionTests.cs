using Microsoft.VisualStudio.TestTools.UnitTesting;
using SPEkit.InvokeReflection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssert;

namespace SPEkit.InvokeReflection.Tests
{
    [TestClass()]
    public class InvokeReflectionTests
    {
        #region TestMethod

        [TestMethod()]
        public void MakeDelegateTestNotParam()
        {
            
            var m1 = InvokeReflection.MakeDelegate<Action>(_getInstance(), "Met1");
            var m2 = InvokeReflection.MakeDelegate<Func<object>>(_getInstance(), "Met2");
            Assert.ThrowsException<ATMHuman.CallMet1NoParam>(m1);
            m2().ShouldBeSameInstanceAs(ATMHuman.Met2NoParam);
        }

        [TestMethod()]
        public void MakeDelegateTestHasParam()
        {
            var m1 = InvokeReflection.MakeDelegate<Action<int>>(_getInstance(), "Met1",typeof(int));
            var m2 = InvokeReflection.MakeDelegate<Func<int, object>>(_getInstance(), "Met2", typeof(int));
            var i1 = 1;
            var i2 = 1;
            Assert.ThrowsException<ATMHuman.CallMet1HasParam>(() => m1( i1));
            //i1.ShouldBeEqualTo(2);
            m2(i2).ShouldBeSameInstanceAs(ATMHuman.Met2HasParam);
            //i2.ShouldBeEqualTo(2);
        }

        [TestMethod()]
        public void InvokeTestNotParam()
        {
            Assert.ThrowsException<ATMHuman.CallMet1NoParam>(()=>InvokeReflection.Invoke(_getInstance(), "Met1"));
            InvokeReflection.Invoke(_getInstance(), "Met2").ShouldBeSameInstanceAs(ATMHuman.Met2NoParam);
        }

        [TestMethod()]
        public void InvokeTestHasParam()
        {
            var i1 = 1;
            var i2 = 1;
            Assert.ThrowsException<ATMHuman.CallMet1HasParam>(
                (() => InvokeReflection.Invoke(_getInstance(), "Met1", i1)));
            //i1.ShouldBeEqualTo(2);
            InvokeReflection.Invoke(_getInstance(), "Met2", i2).ShouldBeSameInstanceAs(ATMHuman.Met2HasParam);
            //i2.ShouldBeEqualTo(2);
        }

        [TestMethod()]
        public void IsExistsTestNotParam()
        {
            InvokeReflection.IsExists(_getInstance(),"Met1").ShouldBeTrue();
            InvokeReflection.IsExists(_getInstance(),"Met2").ShouldBeTrue();
            InvokeReflection.IsExists(_getInstance(),"Met3").ShouldBeFalse();
        }

        [TestMethod()]
        public void IsExistsTestHasParam()
        {
            InvokeReflection.IsExists(_getInstance(), "Met1",typeof(int)).ShouldBeTrue();
            InvokeReflection.IsExists(_getInstance(), "Met2",typeof(int)).ShouldBeTrue();
            InvokeReflection.IsExists(_getInstance(), "Met3",typeof(int)).ShouldBeFalse();
        }

        [TestMethod()]
        [Obsolete]
        public void IsUniqueTestNotParam()
        {
            InvokeReflection.IsUnique(_getInstance(),"Met1").ShouldBeTrue();
            InvokeReflection.IsUnique(_getInstance(),"Met2").ShouldBeTrue();
            InvokeReflection.IsUnique(_getInstance(),"Met3").ShouldBeFalse();
            InvokeReflection.IsUnique(_getInstance(),"Met0").ShouldBeTrue();
        }

        [TestMethod()]
        [Obsolete]
        public void IsUniqueTestHasParam()
        {
            InvokeReflection.IsUnique(_getInstance(), "Met1",typeof(int)).ShouldBeTrue();
            InvokeReflection.IsUnique(_getInstance(), "Met2",typeof(int)).ShouldBeTrue();
            InvokeReflection.IsUnique(_getInstance(), "Met3",typeof(int)).ShouldBeFalse();
            InvokeReflection.IsUnique(_getInstance(), "Met0",typeof(int)).ShouldBeTrue();
        }

        #endregion

        #region 工具人

        private class ATMHuman
        {
            internal class CallMet1NoParam:Exception{}
            internal class CallMet1HasParam:Exception{}

            internal static readonly object Met2NoParam = new();
            internal static readonly object Met2HasParam = new();
            public void Met1()
            {
                throw new CallMet1NoParam();
            }

            public void Met1(int i)
            {
                //i = 2;
                throw new CallMet1HasParam();
            }

            public object Met2()
            {
                return Met2NoParam;
            }

            public object Met2(int i)
            {
                //i = 2;
                return Met2HasParam;
            }
            public void Met0(int i){}
            public void Met0(string i){}
        }

        private ATMHuman _getInstance() => new ATMHuman();

        #endregion
    }
}