using System;
using System.Collections.Generic;
using Apparser.Parser;
using Outcomes;

namespace ConsoleApplication1
{
    static class Program
    {
        static void Main(string[] args)
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
