namespace rule_parser_examples.Model;

record Rule(string Code, NegatableTerm Subject, List<NegatableTerm> Modifiers, List<NegatableTerm> BodyParts, List<NegatableTerm> Demographics);
