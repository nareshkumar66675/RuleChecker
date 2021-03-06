﻿using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RuleChecker
{
    public static class FileOperation
    {
        public static string DataFilePath { get; set; }
        public static string RuleFilePath { get; set; }
        public static DataTable ReadDataFile()
        {
            while(true)
            {
                Console.WriteLine("Enter Name of Data File:");
                string path = Console.ReadLine();

                if(File.Exists(Path.Combine(Directory.GetCurrentDirectory(),path)))
                {
                    DataFilePath = path;
                    return ReadDataFile(path);
                }else
                {
                    Console.WriteLine("Incorrect File Name or File Not Exists");
                }
            }
        }

        public static RulesModel ReadRuleFile()
        {
            while (true)
            {
                Console.WriteLine("Enter Name of Rule File:");
                string path = Console.ReadLine();

                if (File.Exists(Path.Combine(Directory.GetCurrentDirectory(), path)))
                {
                    RuleFilePath = path;
                    return ReadRuleFile(path);
                }
                else
                {
                    Console.WriteLine("Incorrect File Name or File Not Exists");
                }
            }
        }

        public static DataTable ReadDataFile(string path)
        {
            DataTable data = new DataTable();
            if (File.Exists(path))
            {
                using (StreamReader sr = new StreamReader(path))
                {
                    string line = string.Empty;
                    while ((line = sr.ReadLine()) != null)
                    {
                        if(line!=string.Empty)
                        switch (line[0])
                        {
                            case '<':
                                break;
                            case '!':
                                break;
                            case '[':
                                ParseHeaders(line, data);
                                break;
                            default:
                                ParseLine(line, data);
                                break;
                        }
                    }
                }

            }
            else
            {
                throw new Exception("Data File Missing - " + Path.GetFileName(path));
            }
            return data;
        }

        public static RulesModel ReadRuleFile(string path)
        {
            RulesModel rules = new RulesModel();
            rules.Rules = new List<Rule>();

            if (File.Exists(path))
            {
                using (StreamReader sr = new StreamReader(path))
                {
                    string line = string.Empty;
                    while ((line = sr.ReadLine()) != null)
                    {
                        Rule rule = new Rule();
                        if (line == string.Empty || line[0] == '!')
                            continue;
                        else if(int.TryParse(line[0].ToString(),out int specificity))
                        {
                            var values = line.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                            values.ToList().ForEach(t => t.Trim());

                            if(values.Length  == 3 && int.TryParse(values[1], out int strength) 
                                && int.TryParse(values[2], out int matchingCase))
                            {
                                rule.Strength = strength;
                                rule.Specificity = specificity;
                                rule.MatchingCases = matchingCase;

                                rule.Attributes = new OrderedDictionary();

                                var rulLine = sr.ReadLine();
                                if(!rulLine.Contains("->") && !rulLine.Contains("-->"))
                                {
                                    string tempLine = string.Empty;
                                    while((tempLine=sr.ReadLine())!=null )
                                    {
                                        rulLine += tempLine;
                                        if (tempLine.Contains("->") || rulLine.Contains("-->"))
                                            break;

                                    }
                                }

                                var rulData = rulLine.Split(new string[] { "&","->","-->" }, StringSplitOptions.RemoveEmptyEntries).ToList();

                                int count = 0;

                                rulData.ForEach(pair =>
                                    {
                                        count++;
                                        pair.Trim();

                                        var attrPair = pair.Split(',');

                                        if(count == rulData.Count)
                                        {
                                            rule.Decision = new KeyValuePair<string, string>(ReplaceFirstOccurrence(attrPair[0], "(", "").Trim(), ReplaceLastOccurrence(attrPair[1],")","").Trim());
                                        }else
                                        {
                                            rule.Attributes.Add(ReplaceFirstOccurrence(attrPair[0],"(","").Trim(), ReplaceLastOccurrence(attrPair[1], ")", "").Trim());
                                        }

                                        rule.CalculatedValue = Decision.CalculateValue(rule);

                                    });

                            }
                            else
                            {
                                throw new Exception("Error in Rule File: Incorrect Numbers preceding rule");
                            }
                            rules.Rules.Add(rule);
                        }
                    }
                }

            }
            else
            {
                throw new Exception("Rule File Missing - " + Path.GetFileName(path));
            }
            return rules;
            
        }

        private static void ParseHeaders(string line, DataTable data)
        {
            var colHeaders = line.Trim().Replace('[', ' ').Replace(']', ' ').Split(new char[0], StringSplitOptions.RemoveEmptyEntries).ToList();
            colHeaders.ForEach(t => data.Columns.Add(new DataColumn(t, typeof(string))));
            data.Columns.Add(new DataColumn("ID", typeof(string)));
        }
        private static void ParseLine(string line, DataTable data)
        {
            var values = line.Trim().Split(new char[0]).ToList();
            values.Add(data.Rows.Count + 1 + "");

            if (values.Count == data.Columns.Count)
            {
                data.Rows.Add(values.ToArray());
            }
            else
                throw new Exception("Column and Data count Mismatch");

        }

        public static string ReplaceLastOccurrence(string Source, string Find, string Replace)
        {
            int place = Source.LastIndexOf(Find);

            if (place == -1)
                return Source;

            string result = Source.Remove(place, Find.Length).Insert(place, Replace);
            return result;
        }

        public static string ReplaceFirstOccurrence(string text, string search, string replace)
        {
            int pos = text.IndexOf(search);
            if (pos < 0)
            {
                return text;
            }
            return text.Substring(0, pos) + replace + text.Substring(pos + search.Length);
        }
    }
}
