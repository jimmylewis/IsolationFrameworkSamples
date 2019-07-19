namespace FakeUnitTestProject
{
    public class TestObject
    {
        public virtual void DoWork(string workName, int payload)
        {
        }

        public virtual int GetValue()
        {
            return 0;
        }

        public virtual TestObject Child()
        {
            return this;
        }

        public virtual TestObject ChildProperty
        {
            get { return this; }
            set { }
        }
    }
}
