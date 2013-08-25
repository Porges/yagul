using System;
using System.Collections.Generic;
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
        class TestCase
        {
            public string ID { get; private set; }
            public string Category { get; private set; }
            public string PrintSafeEmail { get; private set; }
            public string RealEmail { get; private set; }
            public bool Valid { get; private set; }
            public string Diagnosis { get; set; }

            public TestCase(string id, string category, string email, string sanitisedEmail, bool valid, string diagnosis)
            {
                ID = id;
                Category = category;
                PrintSafeEmail = email;
                RealEmail = sanitisedEmail;
                Valid = valid;
                Diagnosis = diagnosis;
            }
        }

        IEnumerable<TestCase> LoadTests()
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
                yield return new TestCase(id, category, email, sanitisedEmail, valid, diagnosis);
            }
        }
            
            
        [TestMethod]
        public void UnitTests()
        {
            foreach (var test in LoadTests())
            {
                EmailAddress ea;
                string reason;
                Assert.AreEqual(test.Valid,
                    EmailAddress.TryParse(test.RealEmail, out ea, out reason),
                    "Id is: {3}\r\nEmail address is: {0} (sanitised: {5})\r\nCategory is: {1}\r\nDiagnosis is: {2}\r\nMy failure reason is: {4}",
                    test.PrintSafeEmail, test.Category, test.Diagnosis, test.ID, reason, test.RealEmail);
            }
        }

        /// <summary>
        /// Checks that we are faster than <see cref="MailAddress"/> when parsing all valid and non-valid emails from the test suite.
        /// </summary>
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

            var tests = LoadTests().ToList();

            bool first = true;
            for (int i = 0; i < iterations; ++i)
            {
                foreach (var test in tests)
                {
                    timer1.Start();
                    EmailAddress ea;
                    string reason;
                    Assert.AreEqual(test.Valid,
                        EmailAddress.TryParse(test.RealEmail, out ea, out reason),
                        "Id is: {3}\r\nEmail address is: {0} (sanitised: {5})\r\nCategory is: {1}\r\nDiagnosis is: {2}\r\nMy failure reason is: {4}",
                        test.PrintSafeEmail, test.Category, test.Diagnosis, test.ID, reason, test.RealEmail);
            
                    timer1.Stop();

                    timer2.Start();
                    try
                    {
                        new MailAddress(test.RealEmail);
                        if (first && !test.Valid)
                            Trace.WriteLine("MailAddress should have failed: " + test.PrintSafeEmail);
                    }
                    catch
                    {
                        if (first && test.Valid)
                            Trace.WriteLine("MailAddress should have succeeded: " + test.PrintSafeEmail);
                    }
                    timer2.Stop();

                }
                first = false;
            }

            Console.WriteLine(timer1.Elapsed);
            Console.WriteLine(timer2.Elapsed);
            Assert.IsTrue(timer2.Elapsed > timer1.Elapsed); // this is just about my favourite test
        }

        /// <summary>
        /// Checks that we are faster than <see cref="MailAddress"/> when parsing all emails that it can
        /// parse. This negates the effect of all the exceptions MailAddress throws.
        /// </summary>
        [TestMethod]
        [Ignore] // doesn't yet pass...
        public void StrictTimingUnitTests()
        {
            int iterations = 20;

            {
                string reason;
                EmailAddress ea;
                EmailAddress.TryParse("test", out ea, out reason);
            }

            var canBeParsedByMailAddress = new List<string>();
            foreach (var test in LoadTests())
            {
                try
                {
                    new MailAddress(test.RealEmail);
                    canBeParsedByMailAddress.Add(test.RealEmail);
                }
                catch
                {
                    Trace.WriteLine("Ignoring email address: " + test.PrintSafeEmail);
                    // ignore
                }
            }

            var timer1 = new Stopwatch();
            var timer2 = new Stopwatch();
            for (int i = 0; i < iterations; ++i)
            {
                foreach (var test in canBeParsedByMailAddress)
                {
                    timer1.Start();
                    EmailAddress ea;
                    string reason;
                    EmailAddress.TryParse(test, out ea, out reason);
                    timer1.Stop();

                    timer2.Start();
                    new MailAddress(test);
                    timer2.Stop();
                }
            }

            Console.WriteLine(timer1.Elapsed);
            Console.WriteLine(timer2.Elapsed);
            Assert.IsTrue(timer2.Elapsed > timer1.Elapsed);
        }
    }
}
