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
            var errorRate = Math.Round((float)totIncorrectCount / (float)TestData.Rows.Count,4);

            stat.AppendLine("GENERAL STATISTICS");

            stat.AppendFormat("This Report was created from: {0} and from: {1} \n", "rule file", "data file");
            stat.AppendFormat("The total number of cases: {0} \n",TestData.Rows.Count);
            stat.AppendFormat("The total number of attributes: {0} \n", TestData.Columns.Count - 2);
            stat.AppendFormat("The total number of rules: {0} \n", Rules.Rules.Count);
            stat.AppendFormat("The total number of conditions: {0} \n", Rules.Rules.Sum(t => t.Attributes.Count));
            stat.AppendFormat("The total number of cases that are not classified: {0} \n", notClassifiedCount);
            stat.AppendLine("\t PARTIAL MATCHING:");
            stat.AppendFormat("   The total number of cases that are incorrectly classified: {0} \n".PadLeft(4), partialIncorrectCount);
            stat.AppendFormat("   The total number of cases that are correctly classified: {0} \n".PadLeft(4), PartialMatch.Sum(t => t.CorrectlyClassified.Count));
            stat.AppendLine("\t COMPLETE MATCHING:");
            stat.AppendFormat("   The total number of cases that are incorrectly classified: {0} \n".PadLeft(4), completeIncorrectCount);
            stat.AppendFormat("   The total number of cases that are correctly classified: {0} \n", CompleteMatch.Sum(t => t.CorrectlyClassified.Count));
            stat.AppendLine("\t PARTIAL AND COMPLETE MATCHING::");
            stat.AppendFormat("The total number of cases that are not classified or incorrectly classified: {0} \n", totIncorrectCount);
            stat.AppendFormat("Error rate: {0} % \n", errorRate * 100);

            stat.AppendLine("------------------------------------------------------------");

            Console.WriteLine(stat.ToString());
        }

        public void WriteConceptStat()
        {
            if(Decision.PrintConceptStat)
            {
                StringBuilder stat = new StringBuilder();
                var testData = TestData.AsEnumerable();
                var concepts = testData.Select(t => t.Field<string>(TestData.Columns.Count - 2)).Distinct();

                stat.AppendLine("\nConcept Statistics");

                foreach (var concept in concepts)
                {

                    stat.AppendFormat("Concept({0},{1}) :\n",TestData.Columns[TestData.Columns.Count - 2].ColumnName,concept);

                    var tempData = testData.Where(t => t.Field<string>(TestData.Columns.Count - 2) == concept);
                    var conceptNotClassCount = tempData.Where(t=> !GetClassifiedIds().Exists(u => t.Field<string>(TestData.Columns.Count - 1).Equals(u))).Count();
                    var partIncorrCount = PartialMatch.Where(t => t.InCorrectlyClassified.Count > 0 && string.Equals(t.DecisionValue, concept)).Sum(u => u.InCorrectlyClassified.Count);
                    var partCorrCount = PartialMatch.Where(t => t.CorrectlyClassified.Count > 0 && string.Equals(t.DecisionValue, concept)).Sum(u => u.CorrectlyClassified.Count);
                    var completeIncorrCount = CompleteMatch.Where(t => t.InCorrectlyClassified.Count > 0 && string.Equals(t.DecisionValue, concept)).Sum(u => u.InCorrectlyClassified.Count);
                    var completeCorrCount = CompleteMatch.Where(t => t.CorrectlyClassified.Count > 0 && string.Equals(t.DecisionValue, concept)).Sum(u => u.CorrectlyClassified.Count);



                    stat.AppendFormat("The total number of cases that are not classified: {0} \n", conceptNotClassCount);
                    stat.AppendLine("\t PARTIAL MATCHING:");
                    stat.AppendFormat("The total number of cases that are incorrectly classified: {0} \n", partIncorrCount);
                    stat.AppendFormat("The total number of cases that are correctly classified: {0} \n", partCorrCount);
                    stat.AppendLine("\t COMPLETE MATCHING:");
                    stat.AppendFormat("The total number of cases that are incorrectly classified: {0} \n", completeIncorrCount);
                    stat.AppendFormat("The total number of cases that are correctly classified: {0} \n", completeCorrCount);
                    stat.AppendFormat("The total number of cases in the concept: {0}", tempData.Count());
                    stat.AppendLine();
                    stat.AppendLine();

                }

                stat.AppendLine("------------------------------------------------------------");
                Console.WriteLine(stat.ToString());
            }
        }

        private List<string> GetClassifiedIds()
        {
            List<string> notClassIds = new List<string>();

            notClassIds.AddRange(PartialMatch.Select(t => t.ID));
            notClassIds.AddRange(CompleteMatch.Select(t => t.ID));

            return notClassIds;
        }

        private int GetNotClassifiedCount()
        {
            return TestData.Rows.Count - (PartialMatch.Sum(t => t.InCorrectlyClassified.Count) + PartialMatch.Sum(t => t.CorrectlyClassified.Count) +
                CompleteMatch.Sum(t => t.InCorrectlyClassified.Count) + CompleteMatch.Sum(t => t.CorrectlyClassified.Count));
        }
        
    }
}
