using System.Diagnostics;
using System.Text;
using rule_parser_examples.Parsers;
using static System.Console;

WriteLine("Building 1,000,000 rules...");
var sb = new StringBuilder();
for (var i = 0; i < 1_000_000; i++) {
    sb.AppendLine("I10 [hypertension] sdfsad {primary} !`history`");
}
var rulesText = sb.ToString();

WriteLine("Creating parsers...");
IEnumerable<IRulesParser> parsers = [
    new RegexRulesParser(), 
    new GenRegexRulesParser(), 
    new CharacterRulesSBParser(), 
    new CharacterRulesIndexParser(),
    new SpanRulesParser(),
    //new SpracheParser()
];

WriteLine("Parsing rules...");
var s = new Stopwatch();
foreach (var parser in parsers) {
    s.Restart();
    var rules = parser.Parse(rulesText);
    s.Stop();
    WriteLine($"Parsed {rules.Count:n0} rules in {s.ElapsedMilliseconds} milliseconds using {parser.GetType().Name}");
    rules.Clear();
}


