using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Porges.Net.Addresses;

namespace Porges.Net.Tests
{
    [TestClass]
    public class Ipv6AddressTests
    {
        [TestMethod]
        public void TestCreation()
        {
            var shorts = new ushort[] { 1, 2, 3, 4, 5, 6, 7, 8 };

            Assert.IsTrue(new Ipv6Address(shorts).ToShorts().SequenceEqual(shorts));
        }

        [TestMethod]
        public void TestParseGaps()
        {
            for (int i = 0; i < 8; ++i)
            for (int j = 0; j < 8; ++j)
            {
                var shorts = new ushort[8];
                shorts[i] = 'i';
                shorts[j] = 'j';

                var ip = new Ipv6Address(shorts).ToString();

                Ipv6Address address;
                var success = Ipv6Address.TryParse(ip, out address);
                Assert.IsTrue(success, "Address was {0}", ip);
                Assert.AreEqual(ip, address.ToString());
            }
        }

        [TestMethod]
        public void TestParse2Gaps()
        {
            for (int i = 0; i < 8; ++i)
            for (int j = 0; j < 8; ++j)
            for (int k = 0; k < 8; ++k)
            {
                var shorts = new ushort[8];
                shorts[i] = 1;
                shorts[j] = 2;
                shorts[k] = 3;

                var ip = new Ipv6Address(shorts).ToString();
                Ipv6Address address;
                var success = Ipv6Address.TryParse(ip, out address);
                Assert.IsTrue(success, "Address was {0}", ip);
                Assert.AreEqual(ip, address.ToString());
            }
        }

        [TestMethod]
        public void TestParse3Gaps()
        {
            for (int i = 0; i < 8; ++i)
            for (int j = 0; j < 8; ++j)
            for (int k = 0; k < 8; ++k)
            for (int l = 0; l < 8; ++l)
            {
                var shorts = new ushort[8];
                shorts[i] = 1;
                shorts[j] = 2;
                shorts[k] = 3;
                shorts[l] = 4;

                var ip = new Ipv6Address(shorts).ToString();
               
                Ipv6Address address;
                var success = Ipv6Address.TryParse(ip, out address);
                Assert.IsTrue(success, "Address was {0}", ip);
                Assert.AreEqual(ip, address.ToString());
            }
        }
        
        [TestMethod]
        public void InvalidAddresses()
        {
            var ips = new[]
                          {
                              "1::2::3",
                              "w",
                              "",
                              ":1::1",
                              "1::1:",
                          };

            foreach (var ip in ips)
            {
                Ipv6Address address;
                Assert.IsFalse(Ipv6Address.TryParse(ip, out address), "Address is {0}", ip);
            }
        }

        [TestMethod]
        public void ValidAddresses()
        {
            var ips = new[]
                          {
                              "::"
                          };

            foreach (var ip in ips)
            {
                Ipv6Address address;
                Assert.IsTrue(Ipv6Address.TryParse(ip, out address), "Address is {0}", ip);
            }
        }
    }
}
