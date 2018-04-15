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

        public static bool PrintConceptStat { get; set; }

        public static bool PrintCasesStat { get; set; }

        static Decision()
        {
            UseSpecificity = false;
            UseMatchingFactor = false;
            UseStrength = false;
            UseSupport = false;
            PrintConceptStat = false;
            PrintCasesStat = false;
        }

        public static void GetChoicesFromUser()
        {
            if (GetCorrectChoice("Do You wish to use Matcing Factor ? (y/RETURN)",new string[] { "y", "" }).ToLower() == "y")
                UseMatchingFactor = true;
            if (GetCorrectChoice("Do you wish to use strength or conditional probability as Strength Factor ? (s/p)", new string[] { "s", "p" }).ToLower() == "s")
                UseStrength = true;
            if (GetCorrectChoice("Do you wish to use Specificity ? (s/p)", new string[] { "y", "" }).ToLower() == "y")
                UseSpecificity = true;
            if (GetCorrectChoice("Do you wish to use Support of other rules ? (y/RETURN)", new string[] { "y", "" }).ToLower() == "y")
                UseSupport = true;
            if (GetCorrectChoice("Do you wish to know Concept Statistics ? (y/RETURN)", new string[] { "y", "" }).ToLower() == "y")
                PrintConceptStat = true;
            if (GetCorrectChoice("Do you wish to know how cases associated with concepts ? (y/RETURN)", new string[] { "y", "" }).ToLower() == "y")
                PrintCasesStat = true;

        }
        private static string GetCorrectChoice(string question,string[] value)
        {
            string input = string.Empty;

            while(true)
            {
                Console.WriteLine(question);
                input = Console.ReadLine();

                foreach (var val in value)
                {
                    if (val.ToLower() == input.ToLower())
                        return input;
                }
            }

            return null;
        }

        public static float CalculateValue(Rule rule)
        {
            float specificty = (UseSpecificity) ? rule.Specificity : 1;
            float strength = (UseStrength) ? rule.Strength : (float)rule.Strength / (float)rule.MatchingCases;
            float matchingFactor = (UseMatchingFactor) ? rule.Attributes.Count / rule.Specificity : 1;
            return specificty * strength * matchingFactor;
        }
    }
}
