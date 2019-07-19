using FakeItEasy;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace FakeUnitTestProject
{
    [TestClass]
    public class FakeItEasyTests
    {
        [TestMethod]
        public void ThrowException()
        {
            var sut = A.Fake<TestObject>();
            A.CallTo(() => sut.DoWork("", 0)).Throws(() => new System.Exception("fake exception"));
            
            var ex = Assert.ThrowsException<Exception>(() => sut.DoWork("", 0));
            Assert.AreEqual(ex.Message, "fake exception");
        }

        [TestMethod]
        public void MultipleCallsReturnSameDummy()
        {
            var sut = A.Fake<TestObject>();

            // FakeItEasy automatically returns a fake for methods
            Assert.IsNotNull(sut.Child());

            var child = A.Fake<TestObject>();
            A.CallTo(() => sut.Child()).Returns(child);
            // FakeItEasy does not reuse the return value unless manually configured to do so
            Assert.AreSame(sut.Child(), sut.Child());
        }

        [TestMethod]
        public void PropertyChain()
        {
            var sut = A.Fake<TestObject>();
            // FakeItEasy automatically provides a fake for properties
            Assert.IsNotNull(sut.ChildProperty.ChildProperty);
            // the fake is re-used on properties
            Assert.AreSame(sut.ChildProperty.ChildProperty, sut.ChildProperty.ChildProperty);
        }

        [TestMethod]
        public void MethodChain()
        {
            var sut = A.Fake<TestObject>();
            var child = A.Fake<TestObject>();
            A.CallTo(() => child.GetValue()).Returns(5);
            A.CallTo(() => sut.Child()).Returns(child);

            Assert.AreEqual(5, sut.Child().GetValue());
        }

        [TestMethod]
        public void InteractionTest_MethodWasCalledWithSpecificParameters()
        {
            var sut = A.Fake<TestObject>();

            sut.DoWork("expected", 1);

            // don't need to set this up beforehand, just verify the values in the Assert portion of the test
            A.CallTo(() => sut.DoWork("expected", 1)).MustHaveHappened();
        }

        [TestMethod]
        public void InteractionTest_MethodWasCalledWithNonSpecificParameters()
        {
            var sut = A.Fake<TestObject>();

            sut.DoWork("expected", 1);

            // Two ways to specify: A<type>.Ignored or A<type>._
            A.CallTo(() => sut.DoWork(A<string>.Ignored, A<int>._)).MustHaveHappened();
        }

        [TestMethod]
        public void InteractionTest_StrictMock()
        {
            // Strict mocks will fail if any unexpected calls were made to them.

            var sut = new Fake<TestObject>(options => options.Strict());
            sut.CallsTo(s => s.DoWork("expected", 1)).DoesNothing();

            sut.FakedObject.DoWork("expected", 1);
            Assert.ThrowsException<ExpectationException>(() => sut.FakedObject.DoWork("violate strict mock", 2));
        }
    }
}
