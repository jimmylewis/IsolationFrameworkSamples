using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace FakeUnitTestProject
{
    [TestClass]
    public class MoqTests
    {
        [TestMethod]
        public void ThrowException()
        {
            var sut = new Mock<TestObject>();
            sut.Setup(s => s.DoWork("", 0)).Throws(new Exception("fake"));

            var ex = Assert.ThrowsException<Exception>(() => sut.Object.DoWork("", 0));
            Assert.AreEqual(ex.Message, "fake");
        }

        [TestMethod]
        public void MultipleCallsReturnSameDummy()
        {
            var sut = new Mock<TestObject>();
            var child = new Mock<TestObject>();
            sut.Setup(s => s.Child()).Returns(child.Object);

            Assert.IsNotNull(sut.Object.Child());
            Assert.AreSame(sut.Object.Child(), sut.Object.Child());
        }

        [TestMethod]
        public void PropertyChain()
        {
            // By default, Mock returns null instead of a fake.  So you have to set everything up 
            // manually for a chain scenario.
            // note: for this to work in Moq, ChildProperty must have a setter
            var sut = new Mock<TestObject>();
            var child = new Mock<TestObject>();
            var grandChild = new Mock<TestObject>();
            sut.SetupProperty(s => s.ChildProperty, child.Object);
            child.SetupProperty(x => x.ChildProperty, grandChild.Object);

            Assert.IsNotNull(sut.Object.ChildProperty.ChildProperty);
        }

        [TestMethod]
        public void MethodChain()
        {
            var sut = new Mock<TestObject>();
            var child = new Mock<TestObject>();
            child.Setup(x => x.GetValue()).Returns(5);
            sut.Setup(x => x.Child()).Returns(child.Object);

            Assert.AreEqual(5, sut.Object.Child().GetValue());
        }

        [TestMethod]
        public void InteractionTest_MethodWasCalledWithSpecificParameters()
        {
            var sut = new Mock<TestObject>();
            // Note: must specify .Verifiable() for Verify() to work.
            sut.Setup(x => x.DoWork("setup", 1)).Verifiable();

            sut.Object.DoWork("setup", 1);
            sut.Object.DoWork("not setup", 2);

            sut.Verify(); // this will verify only setups which were maked Verifiable()

            // You can also verify an expected call explicitly, regardless of setup or Verifiable()
            sut.Verify(x => x.DoWork("not setup", 2));
        }

        [TestMethod]
        public void InteractionTest_MethodWasCalledWithNonSpecificParameters()
        {
            var sut = new Mock<TestObject>();
            // Note: must specify .Verifiable() for Verify() to work.
            sut.Setup(x => x.DoWork(It.IsAny<string>(), It.IsAny<int>())).Verifiable();

            sut.Object.DoWork("setup", 1);

            sut.Verify(); // this will verify only setups which were maked Verifiable()
            // alternatively:
            sut.Verify(x => x.DoWork(It.IsAny<string>(), It.IsAny<int>()));
        }

        [TestMethod]
        public void InteractionTest_StrictMock()
        {
            // Strict mocks will fail if any unexpected calls were made to them.

            var sut = new Mock<TestObject>(MockBehavior.Strict);
            sut.Setup(x => x.DoWork("expected", 1));

            sut.Object.DoWork("expected", 1);
            Assert.ThrowsException<MockException>(() => sut.Object.DoWork("violate strict mock", 2));
        }
    }
}