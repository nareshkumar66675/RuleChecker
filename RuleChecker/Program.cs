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
            //var data = FileOperation.ReadDataFile(@"C:\Users\Naresh\Desktop\DM2\m-iris.txt");
            //var data = FileOperation.ReadDataFile(@"C:\Users\Naresh\Desktop\DM2\car.txt");
            //var rules = FileOperation.ReadRuleFile(@"C:\Users\Naresh\Desktop\DM2\m-iris.r.txt");
            //var rules = FileOperation.ReadRuleFile(@"C:\Users\Naresh\Desktop\DM2\car.r.p.txt");

            var data = FileOperation.ReadDataFile(@"C:\Users\Naresh\Desktop\DM2\austr-aca.txt");
            var rules = FileOperation.ReadRuleFile(@"C:\Users\Naresh\Desktop\DM2\austr-aca.r.txt");


            //Decision.GetChoicesFromUser();
            Decision.PrintConceptStat = true;
            Decision.PrintCasesStat = true;
            Decision.UseSupport = true;
            Decision.UseStrength = true;
            Decision.UseMatchingFactor = true;
            RuleCheck rCheck = new RuleCheck(data, rules);
            rCheck.Start();
            Statistic stat = new Statistic(data, rules, rCheck.CompleteMatch, rCheck.PartialMatch);
            stat.WriteGeneralStat();
            stat.WriteConceptStat();
            stat.WriteCasesStat();
            Console.ReadLine(); 
        }
    }
}
