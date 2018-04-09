using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RuleChecker
{
    public static class Decision
    {
        public static bool UseSpecificity { get; set; }

        public static bool UseMatchingFactor { get; set; }

        public static bool UseStrength { get; set; } //If True - Strength , else Conditional Prob

        public static bool UseSupport { get; set; }

        public static float CalculateValue(Rule rule)
        {
            float specificty = (UseSpecificity) ? rule.Specificity : 1;
            float strength = (UseStrength) ? rule.Strength : (float)rule.Strength / (float)rule.MatchingCases;
            float matchingFactor = (UseMatchingFactor) ? rule.Attributes.Count / rule.Specificity : 1;
            return specificty * strength * matchingFactor;
        }
    }
}
