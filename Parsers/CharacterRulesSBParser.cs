namespace rule_parser_examples.Parsers;

using System.Collections.Generic;
using System.Text;
using rule_parser_examples.Model;

class CharacterRulesSBParser : IRulesParser
{
    public List<Rule> Parse(string rulesText)
    {
        // Create a list to hold the rules we will be parsing
        var rules = new List<Rule>();

        // Loop through the entire rulesText string
        for (var i = 0; i < rulesText.Length; i++) {
            string? code = null;
            NegatableTerm? subject = null;
            List<NegatableTerm> modifiers = [];
            List<NegatableTerm> bodyParts = [];
            List<NegatableTerm> demographics = [];

            // Consume the code and advance i
            var sbCode = new StringBuilder();
            for (;i < rulesText.Length; i++) {
                sbCode.Append(rulesText[i]);
                if (rulesText[i] == ' ') {
                    break;
                }
            }
            code = sbCode.ToString();
            var nextTermNegated = false;

            // Loop through the rest of the line
            for (; i < rulesText.Length; i++) {
                if (rulesText[i] == '\n')
                {
                    // If we find a line break, try to make a code out of what we have
                    // found so far and add it to the rules list, then clear the found items variables
                    if (code != null && subject != null) {
                        rules.Add(new Rule(code, subject, modifiers, bodyParts, demographics));
                    }
                    // Exit the inner loop and continue to the next line
                    // with a new code and new empty lists
                    break;
                } 
                else if (rulesText[i] == '!') 
                {
                    // If we see a '!', just note that the next term is negated
                    nextTermNegated = true;
                }
                else if (rulesText[i] == '[') 
                {
                    // We have found a subject, so we need to find the closing bracket
                    // and consume the subject, advancing i forward as we do so.
                    var sbSubject = new StringBuilder();
                    for (; i < rulesText.Length; i++) {
                        sbSubject.Append(rulesText[i]);
                        if (rulesText[i] == ']') {
                            break;
                        }
                    }
                    subject ??= new NegatableTerm(sbSubject.ToString(), nextTermNegated);
                    nextTermNegated = false;
                }
                else if (rulesText[i] == '{') 
                {
                    // We have found a modifier, so we need to find the closing brace
                    // and consume the modifier, advancing i forward as we do so.
                    var sbModifier = new StringBuilder();
                    for (; i < rulesText.Length; i++) {
                        sbModifier.Append(rulesText[i]);
                        if (rulesText[i] == '}') {
                            break;
                        }
                    }
                    modifiers.Add(new NegatableTerm(sbModifier.ToString(), nextTermNegated));
                    nextTermNegated = false;
                }
                else if (rulesText[i] == '`') 
                {
                    // We have found a body part, so we need to find the closing backtick
                    // and consume the body part, advancing i forward as we do so.
                    var sbBodyPart = new StringBuilder();
                    for (; i < rulesText.Length; i++) {
                        sbBodyPart.Append(rulesText[i]);
                        if (rulesText[i] == '`') {
                            break;
                        }
                    }
                    bodyParts.Add(new NegatableTerm(sbBodyPart.ToString(), nextTermNegated));
                    nextTermNegated = false;
                }
                else if (rulesText[i] == '"') 
                {
                    // We have found a demographic, so we need to find the closing quote
                    // and consume the demographic, advancing i forward as we do so.
                    var sbDemographic = new StringBuilder();
                    for (; i < rulesText.Length; i++) {
                        sbDemographic.Append(rulesText[i]);
                        if (rulesText[i] == '"') {
                            break;
                        }
                    }
                    demographics.Add(new NegatableTerm(sbDemographic.ToString(), nextTermNegated));
                    nextTermNegated = false;
                } 
            }   
        }
        return rules;
    }
}