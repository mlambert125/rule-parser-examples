namespace rule_parser_examples.Parsers;

using System.Collections.Generic;
using System.Text;
using rule_parser_examples.Model;

class CharacterRulesIndexParser : IRulesParser
{
    public List<Rule> Parse(string rulesText)
    {
        // Create a list to hold the rules we will be parsing
        var rules = new List<Rule>();

        // Loop through the entire rulesText string
        for (var i = 0; i < rulesText.Length; i++) {
            // we will always be at the start of a line at the beginning of the loop body
            string? code = null;
            NegatableTerm? subject = null;
            List<NegatableTerm> modifiers = [];
            List<NegatableTerm> bodyParts = [];
            List<NegatableTerm> demographics = [];
            bool nextTermNegated = false;

            var lineStart = i;

            // consume the code and advance i
            for (;i < rulesText.Length; i++) {
                if (rulesText[i] == ' ') {
                    code = rulesText[lineStart..i];
                    break;
                }
            }

            // find the next line break after the code (or eof)
            var lineStartAfterCode = i;
            var nextLineBreakIndex = rulesText.IndexOf('\n', i);
            if (nextLineBreakIndex == -1) {
                nextLineBreakIndex = rulesText.Length;
            }
 
            // advance i to the start of the next line
            // we are going to do things with this line, but we won't be using
            // i to do it.  we have the current line start (lineStart) and the
            // current line end (nextLineBreakIndex) to work with
            i = nextLineBreakIndex + 1;

            // J loops from the line start to the line end
            for (var j = lineStartAfterCode; j < nextLineBreakIndex; j++) {
                if (rulesText[j] == '!') {
                    // if we see an !, just note that the next term is negated
                    nextTermNegated = true;
                }
                else if (rulesText[j] == '[') 
                {
                    // we found a subject, so we need to find the closing bracket
                    // and consume the subject, advancing j forward as we do so.
                    j++;
                    var subjectStart = j;
                    for (; j < rulesText.Length; j++) {
                        if (rulesText[j] == ']') {
                            subject ??= new NegatableTerm(rulesText[subjectStart..j], nextTermNegated);
                            nextTermNegated = false;
                            break;
                        }
                    }
                } 
                else if (rulesText[j] == '{') 
                {
                    // we found a modifier, so we need to find the closing brace
                    // and consume the modifier, advancing j forward as we do so.
                    j++;
                    var modifierStart = j;
                    for (; j < rulesText.Length; j++) {
                        if (rulesText[j] == '}') {
                            modifiers.Add(new NegatableTerm(rulesText[modifierStart..j], nextTermNegated));
                            nextTermNegated = false;
                            break;
                        }
                    }
                } 
                else if (rulesText[j] == '`') 
                {
                    // we found a body part, so we need to find the closing backtick
                    // and consume the body part, advancing j forward as we do so.
                    j++;
                    var bodyPartStart = j;
                    for (; j < rulesText.Length; j++) {
                        if (rulesText[j] == '`') {
                            bodyParts.Add(new NegatableTerm(rulesText[bodyPartStart..j], nextTermNegated));
                            nextTermNegated = false;
                            break;
                        }
                    }
                } 
                else if (rulesText[j] == '"') 
                {
                    // we found a demographic, so we need to find the closing quote
                    // and consume the demographic, advancing j forward as we do so.
                    j++;
                    var demographicStart = j;
                    for (; j < rulesText.Length; j++) {
                        if (rulesText[j] == '"') {
                            demographics.Add(new NegatableTerm(rulesText[demographicStart..j], nextTermNegated));
                            nextTermNegated = false;
                            break;
                        }
                    }
                }
            }
            // we are done finding things on this line, so make a rule if we at least have a subject
            if (code != null && subject != null) 
                rules.Add(new Rule(code, subject, modifiers, bodyParts, demographics));

            // Continue the loop (remember, we already moved i to the start of the next line)
        }

        // All done, return the rules we found
        return rules;
    }
}