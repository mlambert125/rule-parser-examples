namespace rule_parser_examples.Parsers;
using Sprache;
using rule_parser_examples.Model;

class SpracheParser : IRulesParser
{
    public List<Rule> Parse(string rulesText)
    {
        // A code is a sequence of characters that are not a space or a line break
        var codeParser = Sprache.Parse.CharExcept(" \n").Many().Text();

        // A term is a sequence of characters enclosed in a pair of brackets, braces, backticks or quotes
        // and optionally preceded by a negation symbol
        var termParser = 
            from neg in Sprache.Parse.Char('!').Optional()
            from opener in Sprache.Parse.Chars("[{`\"")
            let expectedCloser = opener switch {
                '[' => ']',
                '{' => '}',
                '`' => '`',
                '"' => '"',
                _ => throw new NotImplementedException()
            }
            from term in Sprache.Parse.CharExcept(new char[] { expectedCloser, '\n' }).Many().Text()
            from closer in Sprache.Parse.Char(expectedCloser)
            select (opener: opener, term: new NegatableTerm(term, neg.IsDefined));
        var junkParser = Sprache.Parse.CharExcept("[{`\"\n").Many();
        
        // A rule is a code followed by a sequence of terms
        var ruleParser =
            from code in codeParser
            from preJunk in junkParser
            from terms in Sprache.Parse.DelimitedBy(termParser, junkParser)
            from postJunk in junkParser
            from lineBreak in Sprache.Parse.Char('\n').Optional()
                let subject = terms.FirstOrDefault(t => t.opener == '[').term
                let modifiers = terms.Where(t => t.opener == '{').Select(t => t.term).ToList()
                let bodyParts = terms.Where(t => t.opener == '`').Select(t => t.term).ToList()
                let demographics = terms.Where(t => t.opener == '"').Select(t => t.term).ToList()
            select new Rule(code, subject, modifiers, bodyParts, demographics);
            
        // Parse many rules
        return ruleParser.Many().Parse(rulesText).ToList();
    }
        
}