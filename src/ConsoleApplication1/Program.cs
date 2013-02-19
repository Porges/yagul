using System;
using System.Collections.Generic;
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
            var parseComplicated =
                Parser.Satisfy<char>(Char.IsWhiteSpace).Many().Then(
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
                   | Parser.Satisfy<char>(char.IsWhiteSpace).Select(c => c.ToString())
                   | Parser.ExactSequence<string, char>("world")
                   | Parser.Exactly('w').Select(c => c.ToString())).Many();

            var result = parser
                .RunWithResult(new StringParser(" wworldhello"));

            var correct = result as Success<string, IList<string>>;
            if (correct != null)
            {
                foreach (var value in correct.Value)
                    Console.WriteLine(value);
            }

            Console.WriteLine(result);
        }
    }
}
