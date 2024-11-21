namespace rule_parser_examples.Parsers;
using rule_parser_examples.Model;

interface IRulesParser
{
    List<Rule> Parse(string rulesText);
}