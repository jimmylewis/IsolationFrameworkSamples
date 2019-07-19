using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using System;

namespace FakeUnitTestProject
{
    [TestClass]
    public class NSubTests
    {
        [TestMethod]
        public void ThrowException()
        {
            var sut = Substitute.For<TestObject>();
            sut.When(x => x.DoWork("", 0))
               .Throw(new Exception("fake"));

            var ex = Assert.ThrowsException<Exception>(() => sut.DoWork("", 0));
            Assert.AreEqual("fake", ex.Message);
        }

        [TestMethod]
        public void MultipleCallsReturnSameDummy()
        {
            var sut = Substitute.For<TestObject>();
            // NSub remembers the instance created so multiple calls return the same by default.
            // This can be overridden by specifying return values manually
            Assert.AreSame(sut.Child(), sut.Child());
        }

        [TestMethod]
        public void PropertyChain()
        {
            var sut = Substitute.For<TestObject>();
            // NSub automatically provides a fake for a virtual property, even if there is no setter defined
            Assert.IsNotNull(sut.ChildProperty.ChildProperty);
            // same instance is reused
            Assert.AreSame(sut.ChildProperty.ChildProperty, sut.ChildProperty.ChildProperty);
        }

        [TestMethod]
        public void MethodChain()
        {
            var sut = Substitute.For<TestObject>();
            // NSub lets you fake entire call chains
            sut.Child().GetValue().Returns(5);

            Assert.AreEqual(5, sut.Child().GetValue());
        }

        [TestMethod]
        public void InteractionTest_MethodWasCalledWithSpecificParameters()
        {
            var sut = Substitute.For<TestObject>();

            sut.DoWork("expected", 1);

            sut.Received().DoWork("expected", 1);
        }

        [TestMethod]
        public void InteractionTest_MethodWasCalledWithNonSpecificParameters()
        {
            var sut = Substitute.For<TestObject>();

            sut.DoWork("expected", 1);

            sut.Received().DoWork(Arg.Any<string>(), Arg.Any<int>());
        }

        [TestMethod]
        public void InteractionTest_StrictMock()
        {
            // NSubstitue does not support strict mocks.
        }
    }
}
