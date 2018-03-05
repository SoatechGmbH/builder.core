using NUnit.Framework;
using Soatech.Builder.Core.Test.TestObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Soatech.Builder.Core.Test
{
    [TestFixture]
    public class ComplexObjectBuilderTest
    {
        [Test]
        public void ComplexObjectBuilderTestMethod()
        {
            var testObj = ComplexObjectBuilder.Create(cb => cb
                .With(o => o.SomeProperty = "Value")
                .CallInit());

            Assert.AreEqual("SET", testObj.Param1);
        }
    }
}
