using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Porges.Net.Addresses;

namespace Porges.Net.Tests
{
    [TestClass]
    public class Ipv4AddressTests
    {
        [TestMethod]
        public void TestCreation()
        {
            var bytes = new byte[] {1, 2, 3, 4};

            Assert.IsTrue(new Ipv4Address(bytes).ToBytes().SequenceEqual(bytes));
        }

        [TestMethod]
        public void TestParse()
        {
            var ip = "1.2.3.4";
            Ipv4Address address2;
            bool success2 = Ipv4Address.TryParse(ip, out address2);

            Assert.IsTrue(success2);
            Assert.AreEqual(ip, address2.ToString());
        }

        [TestMethod]
        public void InvalidAddresses()
        {
            var ips = new[]
                          {
                              "1.2.3",
                              "1.2.3.4.5",
                              ".1.2.3.4",
                              "1.2.3.4.",
                              "1.2w.3.4",
                              "1w.2.3.4",
                              "1.2.3w.4",
                              "1.2.3.4w",
                              "1111.2.3.4",
                              "256.2.3.4",
                              ""
                          };

            foreach (var ip in ips)
            {
                Ipv4Address address;
                Assert.IsFalse(Ipv4Address.TryParse(ip, out address), "Address is {0}", ip);
            }
        }
    }
}
