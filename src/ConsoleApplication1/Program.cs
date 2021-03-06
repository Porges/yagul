﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Apparser.Parser;
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
            var timer4 = new Stopwatch();

            var rubbish = "rubbish";
            EncodedString<UTF8Encoding> encodedRubbish2 = rubbish;
            EncodedString<UnicodeEncoding> encodedRubbish3 = rubbish;
            const int iterations = 10000;
            const int runs = 80;

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
                EncodedString<UTF8Encoding> result2 = "";
                for (int iteration = 0; iteration < iterations; ++iteration)
                {
                    result2 += encodedRubbish2;
                }
                timer2.Stop();

                timer3.Start();
                EncodedString<UnicodeEncoding> result3 = "";
                for (int iteration = 0; iteration < iterations; ++iteration)
                {
                    result3 += encodedRubbish3;
                }
                timer3.Stop();

                timer4.Start();
                var result4 = new StringBuilder();
                for (int iteration = 0; iteration < iterations; ++iteration)
                {
                    result4.Append(rubbish);
                }
                timer4.Stop();

                if (run == 0) // warmup round
                {
                    Console.WriteLine(result.Length);
                    Console.WriteLine(result2.CountBytes);
                    Console.WriteLine(result3.CountBytes);
                    Console.WriteLine(result4.Length);

                    timer1.Reset();
                    timer2.Reset();
                    timer3.Reset();
                    timer4.Reset();
                }
            }

            Console.WriteLine("timer1: {0}", timer1.ElapsedMilliseconds);
            Console.WriteLine("timer2: {0}", timer2.ElapsedMilliseconds);
            Console.WriteLine("timer3: {0}", timer3.ElapsedMilliseconds);
            Console.WriteLine("timer4: {0}", timer4.ElapsedMilliseconds);
        }

        static void TestParser()
        {
            var parseComplicated =
                Parser.Satisfy<char>(x => char.IsWhiteSpace(x)).Many().Then(
                    from he in Parser.ExactSequence<string, char>("he")
                    from ll in Parser.ExactSequence<string, char>("ll")
                    from o in Parser.ExactSequence<string, char>("o")
                    select string.Concat(he, ll, o));

            var resultComplicated = parseComplicated.RunWithResult(new StringParser("   hello"));

            string success;
            if (resultComplicated.TryGetSuccess(out success))
            {
                Console.WriteLine(success);                
            }

            Console.WriteLine("---");

            var parser
                = (Parser.ExactSequence<string, char>("hello")
                   | Parser.Satisfy<char>(x => char.IsWhiteSpace(x)).Select(c => c.ToString())
                   | Parser.ExactSequence<string, char>("world")
                   | Parser.Exactly('w').Select(c => c.ToString())).ManyList();

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
