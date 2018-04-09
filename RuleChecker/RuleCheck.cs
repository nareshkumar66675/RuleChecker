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
        public string DecisionValue { get; set; }

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
            foreach (DataRow row in TestData.Rows)
            {
                CaseDetails cd = new CaseDetails();
                cd.ID = row[row.Table.Columns.Count - 1].ToString();
                cd.DecisionValue = row[row.Table.Columns.Count - 2].ToString();
                Rules.Rules.ForEach(rule =>
                {
                    if (IsMatchingComplete(rule.Attributes, row))//Complete Match
                        cd.MatchingRules.Add(rule);
                });

                CompleteMatch.Add(cd);
            }

            //CompleteMatch.Where(t => t.MatchingRules.Count == 0).ToList().ForEach(notMatched =>
            //{
            //    var partialAttrib = IsMatchingPartial(notMatched.Attributes, row);
            //    var newRule = Helper.DeepCopy<Rule>(rule);
            //    newRule.Attributes = partialAttrib;
            //    newRule.CalculatedValue = Decision.CalculateValue(newRule);
            //}
            //);



            foreach (var row in CompleteMatch)
            {
                var concepts = row.MatchingRules.Select(t => t.Decision.Value).Distinct().ToList();
                if(row.MatchingRules.Count == 1) // Only 1 Matching Rule
                {
                    if(row.DecisionValue == concepts[0]) // Rule is Correctly Classified
                    {
                        row.CorrectlyClassified.Add(row.MatchingRules[0]);
                    }
                    else
                    {
                        row.InCorrectlyClassified.Add(row.MatchingRules[0]);
                    }
                }
                else if(concepts.Count ==1) //Multiple Rules but Single Concept - No support
                {
                    var bestRule= row.MatchingRules.OrderByDescending(t => t.CalculatedValue).FirstOrDefault();
                    if (row.DecisionValue == bestRule.Decision.Value)
                        row.CorrectlyClassified.Add(bestRule);
                    else
                        row.InCorrectlyClassified.Add(bestRule);

                }else if(concepts.Count >1)//Multiple Group Uses Support
                {
                    Rule bestRule;
                    if(Decision.UseSupport)
                    {
                        var bestConcept = row.MatchingRules.GroupBy(t => t.Decision.Value).Select(g => new { key = g.Key, Value = g.Sum(s => s.CalculatedValue) }).OrderByDescending(u => u.Value).FirstOrDefault();
                        bestRule = row.MatchingRules.Where(t => t.Decision.Value == bestConcept.key).OrderByDescending(t => t.CalculatedValue).FirstOrDefault();
                    }else
                    {
                        bestRule = row.MatchingRules.OrderByDescending(t => t.CalculatedValue).FirstOrDefault();
                    }

                    if (row.DecisionValue == bestRule.Decision.Value)
                        row.CorrectlyClassified.Add(bestRule);
                    else
                        row.InCorrectlyClassified.Add(bestRule);
                }
            }
        }

        private OrderedDictionary IsMatchingPartial(OrderedDictionary attributes, DataRow row)
        {
            OrderedDictionary partialAttr = new OrderedDictionary();

            foreach (DictionaryEntry attrValue in attributes)
            {
                var value = attrValue.Value.ToString();
                if (!value.Contains(".."))
                {
                    if (new string[] { value, "*", "-" }.Contains(row[attrValue.Key.ToString()].ToString()))
                    {
                        partialAttr.Add(attrValue.Key, attrValue.Value);
                    }
                }
                else//Interval
                {
                    var intervals = value.Split(new string[] { ".." }, StringSplitOptions.RemoveEmptyEntries);
                    if (float.TryParse(intervals[0], out float min)
                        && float.TryParse(intervals[1], out float max) && float.TryParse(row[attrValue.Key.ToString()].ToString(), out float data))
                    {
                        if (data > min && data < max)
                            partialAttr.Add(attrValue.Key, attrValue.Value);
                    }
                    else
                    {
                        throw new Exception("Invalid Intervals");
                    }
                }

            }

            return partialAttr;
        }

        //private Classification ClassifyCase(DataRow row, Rule rule)
        //{
        //    var matching = IsMatching(rule.Attributes, row);

        //    if(matching)
        //    {
        //        if (row[rule.Decision.Key].ToString() == rule.Decision.Value)
        //        {
        //            return Classification.CorrectlyClassified;
        //        }else
        //        {
        //            return Classification.IncorrectylyClassified;
        //        }
        //    }
        //    return Classification.NoMatch;
        //}
        /// <summary>
        /// Complete Matching
        /// </summary>
        /// <param name="attributes"></param>
        /// <param name="row"></param>
        /// <returns></returns>
        private bool IsMatchingComplete(OrderedDictionary attributes, DataRow row)
        {

            foreach (DictionaryEntry attrValue in attributes)
            {
                var value = attrValue.Value.ToString();
                if (!value.Contains(".."))
                {
                    if (new string[] { value, "*", "-" }.Contains(row[attrValue.Key.ToString()].ToString()))
                    {
                        continue;
                    }
                    else
                    {
                        return false;
                    }
                }
                else//Interval
                {
                    var intervals = value.Split(new string[] { ".." },StringSplitOptions.RemoveEmptyEntries);
                    if(float.TryParse(intervals[0], out float min)
                        && float.TryParse(intervals[1], out float max) && float.TryParse(row[attrValue.Key.ToString()].ToString(),out float data))
                    {
                        if (data > min && data < max)
                            continue;
                        else
                            return false;
                    }
                    else
                    {
                        throw new Exception("Invalid Intervals");
                    }
                }

            }

            return true;

        }


    }
}
