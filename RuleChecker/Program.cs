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
            try
            {
                //var data = FileOperation.ReadDataFile(@"C:\Users\Naresh\Desktop\DM2\m-iris.txt");
                //var data = FileOperation.ReadDataFile(@"C:\Users\Naresh\Desktop\DM2\car.txt");
                //var rules = FileOperation.ReadRuleFile(@"C:\Users\Naresh\Desktop\DM2\m-iris.r.txt");
                //var rules = FileOperation.ReadRuleFile(@"C:\Users\Naresh\Desktop\DM2\car.r.p.txt");

                //var data = FileOperation.ReadDataFile(@"C:\Users\Naresh\Desktop\DM2\austr-aca.txt");
                //var rules = FileOperation.ReadRuleFile(@"C:\Users\Naresh\Desktop\DM2\austr-aca.r.txt");

                var rules = FileOperation.ReadRuleFile();
                var data = FileOperation.ReadDataFile();

                Decision.GetChoicesFromUser();
                //Decision.PrintConceptStat = true;
                //Decision.PrintCasesStat = true;
                //Decision.UseSupport = true;
                //Decision.UseStrength = true;
                //Decision.UseMatchingFactor = true;
                RuleCheck rCheck = new RuleCheck(data, rules);
                rCheck.Start();
                Statistic stat = new Statistic(data, rules, rCheck.CompleteMatch, rCheck.PartialMatch);
                stat.WriteGeneralStat();
                stat.WriteConceptStat();
                stat.WriteCasesStat();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error Occurred while execution. Please check if you have supplied correct values.");
                Console.WriteLine("If not, Please send a mail to nareshkumar@ku.edu with the below exception message");
                Console.WriteLine(ex);
            }
            Console.ReadLine(); 
        }
    }
}
