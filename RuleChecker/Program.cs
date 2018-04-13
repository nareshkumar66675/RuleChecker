using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RuleChecker
{
    class Program
    {
        static void Main(string[] args)
        {
            var data = FileOperation.ReadDataFile(@"C:\Users\Naresh\Desktop\DM2\car.txt");

            var rules = FileOperation.ReadRuleFile(@"C:\Users\Naresh\Desktop\DM2\car.r.c.txt");

            Decision.UseMatchingFactor = false;
            Decision.UseSpecificity = false;
            Decision.UseSupport = false;
            Decision.UseStrength = true;

            RuleCheck rCheck = new RuleCheck(data, rules);
            rCheck.Start();
            Statistic stat = new Statistic(data, rules, rCheck.CompleteMatch, rCheck.PartialMatch);
            stat.WriteGeneralStat();
        }
    }
}
