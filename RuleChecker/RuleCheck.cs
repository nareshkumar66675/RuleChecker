using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RuleChecker
{
    public enum Classification
    {
        NotClassified,
        CorrectlyClassified,
        IncorrectylyClassified,
        CorrectlyAndIncorrectly,
        NoMatch
    }

    public class CaseDetails
    {
        public string ID { get; set; }

        public List<Rule> MatchingRules { get; set; }

        public List<Rule> CorrectlyClassified { get; set; }
        public List<Rule> InCorrectlyClassified { get; set; }

        public CaseDetails()
        {
            MatchingRules = new List<Rule>();
            CorrectlyClassified = new List<Rule>();
            InCorrectlyClassified = new List<Rule>();
        }
    }

    public class RuleCheck
    {
        public DataTable TestData { get; set; }
        public RulesModel Rules { get; set; }

        public List<CaseDetails> CompleteMatch { get; set; }

        public RuleCheck(DataTable data,RulesModel rules)
        {
            TestData = data;
            Rules = rules;
            CompleteMatch = new List<CaseDetails>();
        }

        public void Start()
        {
            foreach(DataRow row in TestData.Rows)
            {
                CaseDetails cd = new CaseDetails();
                cd.ID = row[row.Table.Columns.Count-1].ToString();
                Rules.Rules.ForEach(rule =>
                {
                    if (IsMatching(rule.Attributes, row))
                        cd.MatchingRules.Add(rule);
                });

                CompleteMatch.Add(cd);
            }
        }

        private Classification ClassifyCase(DataRow row, Rule rule)
        {
            var matching = IsMatching(rule.Attributes, row);

            if(matching)
            {
                if (row[rule.Decision.Key].ToString() == rule.Decision.Value)
                {
                    return Classification.CorrectlyClassified;
                }else
                {
                    return Classification.IncorrectylyClassified;
                }
            }
            return Classification.NoMatch;
        }

        private bool IsMatching(OrderedDictionary attributes, DataRow row)
        {
            bool matching = true;

            foreach (DictionaryEntry attrValue in attributes)
            {
                if(new string[] { attrValue.Value.ToString(), "*", "-" }.Contains(row[attrValue.Key.ToString()].ToString()))
                {
                    continue;
                }else
                {
                    return false;
                }
            }

            return true;

        }
    }
}
