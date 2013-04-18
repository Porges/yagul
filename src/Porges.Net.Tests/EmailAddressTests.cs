using System;
using System.Diagnostics;
using System.Linq;
using System.Net.Mail;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Porges.Net.Email;

namespace Porges.Net.Tests
{
    [TestClass]
    public class EmailAddressTests
    {
        [TestMethod]
        public void UnitTests()
        {
            var xdoc = XDocument.Load("EmailTests.xml");
            foreach (var test in xdoc.Descendants("test"))
            {
                var id = test.Attribute("id").Value;
                
                var category = test.Element("category").Value;
                var valid = !category.Contains("ERR");

                var email = test.Element("address").Value;

                var diagnosis = test.Element("diagnosis").Value;
                if (diagnosis == "ISEMAIL_ERR_DOMAINHYPHENSTART" ||
                    diagnosis == "ISEMAIL_ERR_DOMAINHYPHENEND")
                   continue;

                var sanitisedEmail = string.Concat(email.Select(c => c >= 0x2400 ? (char)(c - 0x2400) : c));

                EmailAddress ea;
                string reason;
                Assert.AreEqual(valid,
                    EmailAddress.TryParse(sanitisedEmail, out ea, out reason),
                    "Id is: {3}\r\nEmail address is: {0} (sanitised: {5})\r\nCategory is: {1}\r\nDiagnosis is: {2}\r\nMy failure reason is: {4}", email, category, diagnosis, id, reason, sanitisedEmail);
            }
        }

        [TestMethod]
        public void TimingUnitTests()
        {
            var timer1 = new Stopwatch();
            var timer2 = new Stopwatch();
            int iterations = 20;

            {
                string reason;
                EmailAddress ea;
                EmailAddress.TryParse("test", out ea, out reason);
            }

            bool first = true;
            for (int i = 0; i < iterations; ++i)
            {
                var xdoc = XDocument.Load("EmailTests.xml");
                foreach (var test in xdoc.Descendants("test"))
                {
                    var id = test.Attribute("id").Value;

                    var category = test.Element("category").Value;
                    var valid = !category.Contains("ERR");

                    var email = test.Element("address").Value;

                    var diagnosis = test.Element("diagnosis").Value;
                    if (diagnosis == "ISEMAIL_ERR_DOMAINHYPHENSTART" ||
                        diagnosis == "ISEMAIL_ERR_DOMAINHYPHENEND")
                        continue;

                    var sanitisedEmail = string.Concat(email.Select(c => c >= 0x2400 ? (char) (c - 0x2400) : c));


                    timer1.Start();
                    EmailAddress ea;
                    string reason;
                    Assert.AreEqual(valid,
                                    EmailAddress.TryParse(sanitisedEmail, out ea, out reason),
                                    "Id is: {3}\r\nEmail address is: {0} (sanitised: {5})\r\nCategory is: {1}\r\nDiagnosis is: {2}\r\nMy failure reason is: {4}",
                                    email, category, diagnosis, id, reason, sanitisedEmail);
                    timer1.Stop();

                    timer2.Start();
                    try
                    {
                        new MailAddress(sanitisedEmail);
                        if (first && !valid)
                            Trace.WriteLine("MailAddress should have failed: " + sanitisedEmail);
                    }
                    catch
                    {
                        if (first && valid)
                            Trace.WriteLine("MailAddress should have succeeded: " + sanitisedEmail);
                    }
                    timer2.Stop();

                }
                first = false;
            }

            Console.WriteLine(timer1.Elapsed);
            Console.WriteLine(timer2.Elapsed);
            Assert.IsTrue(timer2.Elapsed > timer1.Elapsed);
        }
    }
}
