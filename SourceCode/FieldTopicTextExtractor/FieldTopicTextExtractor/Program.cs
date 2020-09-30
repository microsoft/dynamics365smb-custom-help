using CommandLine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace FieldTopicTextExtractor
{
    public class Options
    {
        [Option('n', "navDirectory", Required = true, HelpText = "Directory containing the NAV field topic HTML files.")]
        public string NavDirectory { get; set; }

        [Option('o', "outputDirectory", Required = true, HelpText = "Directory where the spreadsheet should be created.")]
        public string OutputDirectory { get; set; }
    }
    class Program
    {
        static void Main(string[] args)
        {
            Parser.Default.ParseArguments<Options>(args)
                .WithParsed<Options>(opts => RunOptions(opts));
        }

        private static void RunOptions(Options opts)
        {
            DirectoryInfo navDirectoryInfo = new DirectoryInfo(opts.NavDirectory);
            String filename = "Field_topic_summaries" + "_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsx";
            FileInfo file = new FileInfo(Path.Combine(new DirectoryInfo(opts.OutputDirectory).FullName, filename));

            List<FieldTopicInfo> fieldTopics = GetFieldTopics(navDirectoryInfo);
            if (fieldTopics.Count > 0)
            {
                try
                {
                    ExcelFieldNameDocument excelFieldNameDocument = new ExcelFieldNameDocument();
                    excelFieldNameDocument.FieldTopics.AddRange(fieldTopics);
                    excelFieldNameDocument.Save(file.FullName);
                    Console.WriteLine("Success: " + file.FullName + " contains the field topic summaries.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Could not generate spreadsheet " + filename + " in directory: " + opts.OutputDirectory);
                    Console.WriteLine("Exception: " + ex.Message);
                }
            }
            else
            {
                Console.WriteLine("No field topics found to process in " + opts.NavDirectory);
            }
        }

        private static List<FieldTopicInfo> GetFieldTopics(DirectoryInfo navDirectoryInfo)
        {
            List<FieldTopicInfo> fieldTopicInfos = new List<FieldTopicInfo>();
            if (navDirectoryInfo.Exists)
            {
                foreach (FileInfo file in navDirectoryInfo.GetFiles("T_*.htm"))
                {
                    try
                    {
                        fieldTopicInfos.Add(DomParser.getFieldTopicSummary(file));
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Could not extract the field topic summary from " + file.FullName);
                        Console.WriteLine("Exception: " + ex.Message);
                    }
                }
            }
            return fieldTopicInfos;
        }
    }
}
