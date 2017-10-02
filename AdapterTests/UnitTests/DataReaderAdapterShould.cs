using System.Linq;
using Adapter;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ploeh.AutoFixture;

namespace AdapterTests.UnitTests
{
    [TestClass]
    public class DataReaderAdapterShould
    {
        private readonly Fixture _fixture = new Fixture();

        [TestMethod]
        public void SupportEnumeration()
        {
            var customers = _fixture.CreateMany<Customer>(10).ToList();
            var customerDr = new DataReaderAdapter<Customer>(customers);
            var counter = 0;
            while (customerDr.Read())
            {
                Assert.IsNotNull(customerDr.GetValue(0)); //Id
                Assert.IsNotNull(customerDr.GetValue(1)); //First name
                Assert.IsNotNull(customerDr.GetValue(2)); //Last name
                Assert.IsNotNull(customerDr.GetValue(3)); //Address
                counter++;
            }
            Assert.AreEqual(counter, customers.Count);
        }
    }
}