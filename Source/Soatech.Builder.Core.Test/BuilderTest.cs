using Soatech.Builder.Core.Test.TestObjects;
using NUnit.Framework;
using System;
using Moq;

namespace Soatech.Builder.Core.Test
{
    [TestFixture]
    public class BuilderTest
    {
        [Test]
        public void BuildSimpleObjectWithoutSetup()
        {
            var obj = Builder<SimpleObject>.Create();
            Assert.IsNotNull(obj);
        }

        [Test]
        public void BuildSimpleObjectWithSimplePropertySetUp()
        {
            var obj = Builder<SimpleObject>.Create(b => b
                .With(o => o.Val1 = 1));

            Assert.AreEqual(1, obj.Val1);
        }

        [Test]
        public void BuildSimpleObjectWithListUsingListBuilder()
        {
            var obj = Builder<SimpleObject>.Create(b => b
                .With(o => o.List = ListBuilder<string>.Create()));

            Assert.IsNotNull(obj.List);
        }

        [Test]
        public void BuildParentObjectWithChildListUsingListBuilder()
        {
            var obj = Builder<ParentObject>.Create(b => b
                .With(o => o.List = ListBuilder<ChildObject>.Create(clb => clb
                    .Add(cb => cb.With(c => c.Val1 = 1))
                    .Add(cb => cb.With(c => c.Val1 = 2)))));

            Assert.AreEqual(2, obj.List.Count);
            Assert.AreEqual(1, obj.List[0].Val1);
            Assert.AreEqual(2, obj.List[1].Val1);
        }

        [Test]
        public void BuildObjectWithoutInvalidConstructorArgs_InvalidOperationException()
        {
            var ex = Record.Exception(() => Builder<SimpleObject>.Create(b => b
                .WithCtorArg("Not parameter needed")));

            Assert.IsNotNull(ex);
            Assert.IsInstanceOf(typeof(InvalidOperationException), ex);
            Assert.IsTrue(ex.Message.Contains("No constructor"));
        }

        [Test]
        public void BuildObjectWithConstructorWithoutSetup()
        {
            var obj = Builder<ObjectWithConstructor>.Create();
            Assert.IsNotNull(obj);
        }

        [Test]
        public void BuildObjectWithConstructorWithFirstArgSetup()
        {
            var obj = Builder<ObjectWithConstructor>.Create(b => b
                .WithCtorArg(1));

            Assert.AreEqual(1, obj.Val1);
        }

        [Test]
        public void BuildObjectWithConstructorWithFirstArgFncSetup()
        {
            var obj = Builder<ObjectWithConstructor>.Create(b => b
                .WithCtorArg(() => 1));

            Assert.AreEqual(1, obj.Val1);
        }

        [Test]
        public void BuildObjectWithConstructorWithNamedArgSetup()
        {
            var obj = Builder<ObjectWithConstructor>.Create(b => b
                .WithCtorArg("val2", 1));

            Assert.AreEqual(1, obj.Val2);
        }

        [Test]
        public void BuildObjectWithConstructorWithWrongNamedArg_InvalidOperationException()
        {
            var ex = Record.Exception(() => Builder<ObjectWithConstructor>.Create(b => b
                .WithCtorArg("invalidArg", 1)));

            Assert.IsNotNull(ex);
            Assert.IsInstanceOf(typeof(InvalidOperationException), ex);
            Assert.IsTrue(ex.Message.Contains("No constructor"));
        }

        [Test]
        public void BuildObjectWithConstructorWithNamedArgFncSetup()
        {
            var obj = Builder<ObjectWithConstructor>.Create(b => b
                .WithCtorArg("val2", () => 1));

            Assert.AreEqual(1, obj.Val2);
        }

        [Test]
        public void BuildObjectWithConstructorWithArgSetupByIndex()
        {
            var obj = Builder<ObjectWithConstructor>.Create(b => b
                .WithCtorArg(1)
                .WithCtorArg(2)
                .WithCtorArg("test"));

            Assert.AreEqual(1, obj.Val1);
            Assert.AreEqual(2, obj.Val2);
            Assert.AreEqual("test", obj.Val3);
        }

        [Test]
        public void BuildObjectWithConstructorWithListBuilderFnc()
        {
            var obj = Builder<ObjectWithConstructor>.Create(b => b
                .WithCtorArg(() => ListBuilder<string>.Create(lb =>  
                    lb.Add("val1"))));

            Assert.IsNotNull(obj.List);
            Assert.AreEqual(1, obj.List.Count);
            Assert.AreEqual("val1", obj.List[0]);
        }

        [Test]
        public void BuildObjectWithConstructorWithListBuilder()
        {
            var obj = Builder<ObjectWithConstructor>.Create(b => b
                .WithCtorArg(ListBuilder<string>.Create(lb =>
                    lb.Add("val1"))));

            Assert.IsNotNull(obj.List);
            Assert.AreEqual(1, obj.List.Count);
            Assert.AreEqual("val1", obj.List[0]);
        }

        [Test]
        public void BuildObjectWithConstructorWithInterfaceBuilderFnc()
        {
            var mockedInterace = new Mock<IFormatProvider>().Object;
            var obj = Builder<ObjectWithInterfaceConstructor>.Create(b => b
                .WithCtorArg(() => mockedInterace));

            Assert.IsNotNull(obj.FormatProvider);
            Assert.AreEqual(mockedInterace, obj.FormatProvider);
        }

        [Test]
        public void BuildObjectWithConstructorWithInterfaceBuilderDerivedFnc()
        {
            var mockedInterace = new Mock<IDerivedFormatProvider>().Object;
            var obj = Builder<ObjectWithInterfaceConstructor>.Create(b => b
                .WithCtorArg<IFormatProvider>(() => mockedInterace));

            Assert.IsNotNull(obj.FormatProvider);
            Assert.AreEqual(mockedInterace, obj.FormatProvider);
        }
    }
}
