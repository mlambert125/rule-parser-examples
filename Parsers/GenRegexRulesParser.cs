namespace rule_parser_examples.Parsers;

using System.Collections.Generic;
using System.Text.RegularExpressions;
using rule_parser_examples.Model;

partial class GenRegexRulesParser : IRulesParser
{
    [GeneratedRegex(@"^(\w+)")]
    private static partial Regex CodeRegex();
    
    [GeneratedRegex(@"(!)?\[(.*?)\]")]
    private static partial Regex SubjectRegex();

    [GeneratedRegex(@"(!)?\{(.*?)\}")]
    private static partial Regex ModifierRegex();

    [GeneratedRegex(@"(!)?`(.*?)`")]
    private static partial Regex BodyPartRegex();

    [GeneratedRegex(@"(!)?""(.*?)""")]
    private static partial Regex DemographicRegex();

    public List<Rule> Parse(string rulesText)
    {
        // Just split the lines
        var lines = rulesText.Split("\n");

        // Create a list to hold the rules we will be parsing
        var rules = new List<Rule>();

        // Loop through the lines
        foreach (var line in lines) {
            // Find the code with the CodeRegex
            var code = CodeRegex().Match(line).Groups[1].Value;
            var rest = line.Substring(code.Length).Trim();

            // Find the first subject, if any, with the SubjectRegex
            var subject = (
                from m in SubjectRegex().Matches(rest)
                select new NegatableTerm(m.Groups[2].Value, m.Groups[1].Success)
            ).FirstOrDefault();

            // Find the modifiers with the ModifierRegex
            var modifiers = (
                from m in ModifierRegex().Matches(rest)
                select new NegatableTerm(m.Groups[2].Value, m.Groups[1].Success)
            ).ToList();

            // Find the body parts with the BodyPartRegex
            var bodyParts = (
                from m in BodyPartRegex().Matches(rest)
                select new NegatableTerm(m.Groups[2].Value, m.Groups[1].Success)
            ).ToList();

            // Find the demographics with the DemographicRegex
            var demographics = (
                from m in DemographicRegex().Matches(rest)
                select new NegatableTerm(m.Groups[2].Value, m.Groups[1].Success)
            ).ToList();

            // If we found a code and a subject, add the rule to the list
            if (subject is not null)
                rules.Add(new Rule(code, subject, modifiers, bodyParts, demographics));
        }
        return rules;
    }
}