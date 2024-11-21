namespace rule_parser_examples.Parsers;

using System.Text.RegularExpressions;
using rule_parser_examples.Model;

class RegexRulesParser : IRulesParser
{
    public List<Rule> Parse(string rulesText)
    {
        // Just split the lines
        var lines = rulesText.Split("\n");
        
        // Create a list to hold the rules we will be parsing
        var rules = new List<Rule>();

        // Loop through the lines
        foreach (var line in lines) {
            // Find the code with the CodeRegex
            var code = Regex.Match(line, @"^(\w+)").Groups[1].Value;
            var rest = line.Substring(code.Length).Trim();

            // Find the first subject, if any
            var subject = (
                from m in Regex.Matches(rest, @"(!)?\[(.*?)\]")
                select new NegatableTerm(m.Groups[2].Value, m.Groups[1].Success)
            ).FirstOrDefault();

            // Find the modifiers
            var modifiers = (
                from m in Regex.Matches(rest, @"(!)?\{(.*?)\}")
                select new NegatableTerm(m.Groups[2].Value, m.Groups[1].Success)
            ).ToList();

            // Find the body parts
            var bodyParts = (
                from m in Regex.Matches(rest, @"(!)?`(.*?)`")
                select new NegatableTerm(m.Groups[2].Value, m.Groups[1].Success)
            ).ToList();

            // Find the demographics
            var demographics = (
                from m in Regex.Matches(rest, @"(!)?""(.*?)""")
                select new NegatableTerm(m.Groups[2].Value, m.Groups[1].Success)
            ).ToList();

            // Add the rule to the list if we have a code and a subject
            if (subject != null)
                rules.Add(new Rule(code, subject, modifiers, bodyParts, demographics));
        }
        return rules;
    }
}