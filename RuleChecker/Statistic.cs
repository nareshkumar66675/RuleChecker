using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RuleChecker
{
    public class Statistic
    {
        public DataTable TestData { get; set; }
        public RulesModel Rules { get; set; }

        public List<CaseDetails> CompleteMatch { get; set; }

        public List<CaseDetails> PartialMatch { get; set; }

        public Statistic(DataTable data, RulesModel rules, List<CaseDetails> completeMatch, List<CaseDetails> partialMatch)
        {
            TestData = data;
            Rules = rules;
            CompleteMatch = completeMatch;
            PartialMatch = partialMatch;
        }

        public void WriteGeneralStat()
        {
            StringBuilder stat = new StringBuilder();

            var notClassifiedCount = GetNotClassifiedCount();
            var partialIncorrectCount = PartialMatch.Sum(t => t.InCorrectlyClassified.Count);
            var completeIncorrectCount = CompleteMatch.Sum(t => t.InCorrectlyClassified.Count);
            var totIncorrectCount = notClassifiedCount + partialIncorrectCount + completeIncorrectCount;
            var errorRate = Math.Round((float)totIncorrectCount / (float)TestData.Rows.Count,2);

            stat.AppendFormat("This Report was created from: {0} and from: {1} \n", "rule file", "data file");
            stat.AppendFormat("The total number of cases: {0} \n",TestData.Rows.Count);
            stat.AppendFormat("The total number of attributes: {0} \n", TestData.Columns.Count - 2);
            stat.AppendFormat("The total number of rules: {0} \n", Rules.Rules.Count);
            stat.AppendFormat("The total number of conditions: {0} \n", Rules.Rules.Sum(t => t.Attributes.Count));
            stat.AppendFormat("The total number of cases that are not classified: {0} \n", notClassifiedCount);
            stat.AppendLine("\t PARTIAL MATCHING:");
            stat.AppendFormat("The total number of cases that are incorrectly classified: {0} \n", partialIncorrectCount);
            stat.AppendFormat("The total number of cases that are correctly classified: {0} \n", PartialMatch.Sum(t => t.CorrectlyClassified.Count));
            stat.AppendLine("\t COMPLETE MATCHING:");
            stat.AppendFormat("The total number of cases that are incorrectly classified: {0} \n", completeIncorrectCount);
            stat.AppendFormat("The total number of cases that are correctly classified: {0} \n", CompleteMatch.Sum(t => t.CorrectlyClassified.Count));
            stat.AppendLine("\t PARTIAL AND COMPLETE MATCHING::");
            stat.AppendFormat("The total number of cases that are not classified or incorrectly classified: {0} \n", totIncorrectCount);
            stat.AppendFormat("Error rate: {0}", errorRate);

            Console.WriteLine(stat.ToString());
        }
        private int GetNotClassifiedCount()
        {
            return TestData.Rows.Count - (PartialMatch.Sum(t => t.InCorrectlyClassified.Count) + PartialMatch.Sum(t => t.CorrectlyClassified.Count) +
                CompleteMatch.Sum(t => t.InCorrectlyClassified.Count) + CompleteMatch.Sum(t => t.CorrectlyClassified.Count));
        }
        
    }
}
