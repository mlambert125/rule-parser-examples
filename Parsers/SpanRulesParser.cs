namespace rule_parser_examples.Parsers;

using System.Collections.Generic;
using rule_parser_examples.Model;

class SpanRulesParser : IRulesParser
{
    public List<Rule> Parse(string rulesText)
    {
        // Create a list to hold the rules we will be parsing
        var rules = new List<Rule>();

        // Make a text span that will start at the line we are currently parsing
        // and go to the end of the rulesText
        var remainingRulesTextSpan = rulesText.AsSpan();

        // Loop as long as our sliding span has any length
        while(remainingRulesTextSpan.Length > 0) {
            // Find the next line break
            var lineBreakIndex = remainingRulesTextSpan.IndexOf('\n');

            // If we didn't find a line break, set the line break index to the
            // end of the entire document span
            if (lineBreakIndex == -1)
                lineBreakIndex = remainingRulesTextSpan.Length;
        
            // Make a little span that captures the current line (start of our
            // big window up until the next line break or EOF)
            var lineSpan = remainingRulesTextSpan[..lineBreakIndex];

            // Call the ParseLine method, giving it the line span we just made
            // and add the result to the rules list
            rules.Add(ParseLine(lineSpan));

            // Slide the span start to the start of the next line
            remainingRulesTextSpan = lineBreakIndex == remainingRulesTextSpan.Length ? [] : remainingRulesTextSpan[(lineBreakIndex + 1)..];
        }
        return rules;
    }

    public Rule ParseLine(ReadOnlySpan<char> lineSpan) {
        // Find the first space in the line
        var firstSpace = lineSpan.IndexOf(' ');

        // Make a span that captures the code part of the line (everything up to the first space)
        var codeSpan = lineSpan[..firstSpace];

        // Make a span that captures the rest of the line (everything after the first space)
        var restOfLineSpan = lineSpan[(firstSpace + 1)..];

        // Capture the code from the code span
        var code = codeSpan.ToString();

        // Initialize some variables that we will be filling in as we parse the line
        NegatableTerm? subject = null;
        var modifiers = new List<NegatableTerm>();
        var bodyParts = new List<NegatableTerm>();
        var demographics = new List<NegatableTerm>();

        // Is the next term negated?
        var nextTermNegated = false;

        // Loop through the rest of the line, with the restOfLineSpan that we
        // will keep advancing the start of as we parse the line
        while(restOfLineSpan.Length > 0) {
            if (restOfLineSpan[0] == '!') {
                // If we see a !, just note that the next term is negated
                nextTermNegated = true;

                // Advance the start of the restOfLineSpan to the next character
                restOfLineSpan = restOfLineSpan[1..];
            } else if (restOfLineSpan[0] == '[') {
                // Advance the start of the restOfLineSpan to the next character to go past the [
                restOfLineSpan = restOfLineSpan[1..];
                // Find the next ]
                var subjectEnd = restOfLineSpan.IndexOf(']');   
                // Build a subject from the start of the restOfLineSpan up to the ]
                subject ??= new NegatableTerm(restOfLineSpan[..subjectEnd].ToString(), nextTermNegated);
                // Reset the next term negated flag
                nextTermNegated = false;
                // Advance the start of the restOfLineSpan to the character after the ]
                restOfLineSpan = restOfLineSpan[(subjectEnd + 1)..];
            } else if (restOfLineSpan[0] == '{') {
                // Advance the start of the restOfLineSpan to the next character to go past the {
                restOfLineSpan = restOfLineSpan[1..];
                // Find the next }
                var modifierEnd = restOfLineSpan.IndexOf('}');
                // Build a modifier from the start of the restOfLineSpan up to the }
                modifiers.Add(new NegatableTerm(restOfLineSpan[..modifierEnd].ToString(), nextTermNegated));
                // Reset the next term negated flag
                nextTermNegated = false;
                // Advance the start of the restOfLineSpan to the character after the }
                restOfLineSpan = restOfLineSpan[(modifierEnd + 1)..];
            } else if (restOfLineSpan[0] == '`') {
                // Advance the start of the restOfLineSpan to the next character to go past the `
                restOfLineSpan = restOfLineSpan[1..];
                // Find the next `
                var bodyPartEnd = restOfLineSpan.IndexOf('`');
                // Build a body part from the start of the restOfLineSpan up to the `
                bodyParts.Add(new NegatableTerm(restOfLineSpan[..bodyPartEnd].ToString(), nextTermNegated));
                // Reset the next term negated flag
                nextTermNegated = false;
                // Advance the start of the restOfLineSpan to the character after the `
                restOfLineSpan = restOfLineSpan[(bodyPartEnd + 1)..];
            } else if (restOfLineSpan[0] == '"') {
                // Advance the start of the restOfLineSpan to the next character to go past the "
                restOfLineSpan = restOfLineSpan[1..];
                // Find the next "
                var demographicEnd = restOfLineSpan.IndexOf('"');
                // Build a demographic from the start of the restOfLineSpan up to the "
                demographics.Add(new NegatableTerm(restOfLineSpan[..demographicEnd].ToString(), nextTermNegated));
                // Reset the next term negated flag
                nextTermNegated = false;
                // Advance the start of the restOfLineSpan to the character after the "
                restOfLineSpan = restOfLineSpan[(demographicEnd + 1)..];
            } else {
                // We aren't in a term, and we didn't find a term start character, so this must be a junk character
                // so just slide the start of the restOfLineSpan to the next character and keep going
                restOfLineSpan = restOfLineSpan[1..];
            }
        }
        return new Rule(code, subject!, modifiers, bodyParts, demographics);
    }
}
