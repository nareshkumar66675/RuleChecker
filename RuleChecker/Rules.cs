using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Specialized;

namespace RuleChecker
{
    public class RulesModel
    {
        public List<Rule> Rules { get; set; }
    }

    public class Rule
    {
        public Rule(int spec,int strength, int matchingCases, KeyValuePair<string, string> decision,OrderedDictionary attrib)
        {
            this.Specificity = spec;
            Strength = strength;
            MatchingCases = matchingCases;
            Decision = decision;
            Attributes = attrib;
        }

        public int Specificity { get; set; }
        public int Strength { get; set; }
        public int MatchingCases { get; set; }

        public KeyValuePair<string,string> Decision { get; set; }

        public OrderedDictionary Attributes { get; set; }
        public float CalculatedValue { get;  set; }
    }
}
