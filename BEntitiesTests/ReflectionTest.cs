using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BEntitiesTests
{
    [TestClass]
    public class ReflectionTest
    {

        public class A
        {
            public int Test1 { get; set; }
        }

        public class B : A
        {
            public string Test2 { get; set; }
        }

        public class C :B
        {
            public double Test3 { get; set; }
        }

        public class D : C
        {
            public double Test4 { get; set; }
        }

        [TestMethod]
        public void TestAssignableFromMethod()
        {
            Assert.IsTrue(typeof(A).IsAssignableFrom(typeof(B)));
            Assert.IsTrue(typeof(B).IsAssignableFrom(typeof(C)));
            Assert.IsTrue(typeof(A).IsAssignableFrom(typeof(C)));
            Assert.IsTrue(typeof(A).IsAssignableFrom(typeof(D)));
        }
    }
}
