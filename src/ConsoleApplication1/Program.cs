using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Apparser;
using Apparser.Parser;
using Outcomes;
using Strung;

namespace ConsoleApplication1
{
    static class Program
    {
        static void Main(string[] args)
        {
            TestStringConcatenation();
        }

        private static void TestStringConcatenation()
        {
            var timer1 = new Stopwatch();
            var timer2 = new Stopwatch();
            var timer3 = new Stopwatch();

            var rubbish = "rubbish";
            var encodedRubbish2 = new EncodedString<UTF8Encoding>(rubbish);
            var encodedRubbish3 = new EncodedString<UnicodeEncoding>(rubbish);
            const int iterations = 10000;
            const int runs = 20;

            for (var run = 0; run < runs+1; ++run)
            {
                timer1.Start();
                string result = ""; 
                for (int iteration = 0; iteration < iterations; ++iteration)
                {
                    result += rubbish;
                }
                timer1.Stop();

                timer2.Start();
                var result2 = new EncodedString<UTF8Encoding>();
                for (int iteration = 0; iteration < iterations; ++iteration)
                {
                    result2 += encodedRubbish2;
                }
                timer2.Stop();

                timer3.Start();
                var result3 = new EncodedString<UnicodeEncoding>();
                for (int iteration = 0; iteration < iterations; ++iteration)
                {
                    result3 += encodedRubbish3;
                }
                timer3.Stop();

                if (run == 0)
                {
                    timer1.Reset();
                    timer2.Reset();
                    timer3.Reset();
                }
            }

            Console.WriteLine("timer1: {0}", timer1.ElapsedMilliseconds);
            Console.WriteLine("timer2: {0}", timer2.ElapsedMilliseconds);
            Console.WriteLine("timer3: {0}", timer3.ElapsedMilliseconds);
        }

        static void TestParser()
        {
            var parseComplicated =
                ParserExtensions.Satisfy<char>(Char.IsWhiteSpace).Many().Then(
                    from he in ParserExtensions.ExactSequence<string, char>("he")
                    from ll in ParserExtensions.ExactSequence<string, char>("ll")
                    from o in ParserExtensions.ExactSequence<string, char>("o")
                    select string.Concat(he, ll, o));

            var resultComplicated = parseComplicated.RunWithResult(new StringParser("   hello"));

            string success;
            if (resultComplicated.TryGetSuccess(out success))
            {
                Console.WriteLine(success);                
            }

            Console.WriteLine("---");

            var parser
                = (ParserExtensions.ExactSequence<string, char>("hello")
                   | ParserExtensions.Satisfy<char>(char.IsWhiteSpace).Select(c => c.ToString())
                   | ParserExtensions.ExactSequence<string, char>("world")
                   | ParserExtensions.Exactly('w').Select(c => c.ToString())).Many();

            var result = parser
                .RunWithResult(new StringParser(" wworldhello"));

            IList<string> correct;
            if (result.TryGetSuccess(out correct))
            {
                foreach (var value in correct)
                    Console.WriteLine(value);
            }

            Console.WriteLine(result);
        }
    }
}
