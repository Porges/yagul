using System;
using Apparser.Input;
using Apparser.Parser;

namespace Porges.Net.Email
{
    public class EmailAddress
    {
        private readonly string _email;

        private EmailAddress(string email)
        {
            _email = email;
        }

        public override string ToString()
        {
            return _email;
        }

        public static bool TryParse(string email, out EmailAddress emailAddress, out string reason)
        {
            var result = _isValid.Run(new StringParser(email));

            if (result.IsSuccess)
            {
                emailAddress = new EmailAddress(email);
                reason = null;
                return true;
            }
            else
            {
                emailAddress = null;
                result.TryGetFailure(out reason);
                return false;
            }
        }

        private static readonly Parser<char> _isValid = IsValidEmail();

        private static Parser<char> IsValidEmail()
        {
            var wsp1 = Parser.Satisfy((char c) => c == ' ' || c == '\t');
            var crlf = Parser.ExactSequence<string, char>("\r\n").Name("crlf");

            var cr = Parser.Satisfy<char>(c => c == '\r');
            var lf = Parser.Satisfy<char>(c => c == '\n');

            var vchar = Parser.Satisfy<char>(c => c >= 0x21 && c <= 0x7e);

            var obsNoWsCtl = Parser.Satisfy<char>(c =>
                                                  c >= 1 && c <= 8 ||
                                                  c == 11 || c == 12 ||
                                                  c >= 14 && c <= 31 ||
                                                  c == 127);

            var nullChar = '\0';

            var fws =
                (crlf.Then(wsp1).Many(1) |
                wsp1.Then((crlf.Then(wsp1)).Optional())).Name("folding whitespace");

            var commentText = Parser.Satisfy<char>(c =>
                                                c >= 33 && c <= 39 ||
                                                c >= 42 && c <= 91 ||
                                                c >= 93 && c <= 126) | obsNoWsCtl;

            var quotedPair = Parser.Exactly('\\')
                                   .Then(vchar | lf | cr | nullChar | obsNoWsCtl | wsp1).Name("quoted pair");

            var commentParser = Parser.Deferred<char>();
            var comment = commentParser.Name("comment");

            var commentContent = (commentText.Many(1) | quotedPair | comment).Name("comment content");

            commentParser.Parser = Parser.Exactly('(')
                .Then((commentContent | fws).Many())
                .Then(')');

            var cfws = (fws | comment).Many().Name("cfws");

            var atomText = Parser.Satisfy<char>(c => c.IsAsciiAlphaNum()
                                                     || "!#$%&'*+/=?^_`{|}~-".IndexOf(c) >= 0);

            var atom = atomText.Many(1).Name("atom");

            var quotedText = Parser.Satisfy<char>(c =>
                                c == 33 ||
                                c >= 35 && c <= 91 ||
                                c >= 93 && c <= 126) | obsNoWsCtl;

            var quotedContent = quotedPair | quotedText.Many(1);

            var quotedString = Parser.Exactly('"')
                                .Then(fws.Optional().Then(quotedContent).Many())
                                .Then(fws.Optional())
                                .Then('"').Name("quoted string");

            var dottedAtoms = 
                cfws.Optional()
                .Then(atom | quotedString)
                .Then(cfws.Optional())
                .SepBy('.', 1).Name("dotted atoms");

            var domainText = Parser.Satisfy<char>(c =>
                                                  c >= 33 && c <= 90 ||
                                                  c >= 94 && c <= 126) |
                                                  obsNoWsCtl |
                                                  quotedPair;

            var domainLiteral = cfws.Optional()
                .Then('[')
                .Then(fws.Optional().Then(domainText).Many())
                .Then(fws.Optional())
                .Then(']').Then(cfws.Optional()).Name("domain literal");

            var local = dottedAtoms.Name("local-part");
            var domain = (dottedAtoms | domainLiteral).Name("domain-part");

            var email = local.Then('@').Then(domain).Then(Parser.EndOfInput<char>());

            return email;
        }
    }
}
