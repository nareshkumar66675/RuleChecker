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
        public int Specificity { get; set; }
        public int Strength { get; set; }
        public int MatchingCases { get; set; }

        public KeyValuePair<string,string> Decision { get; set; }

        public OrderedDictionary Attributes { get; set; }
    }
}
