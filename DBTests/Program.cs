using Sk2Services;

namespace DBTests
{

    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    ///  Command-line entre to the WiktionaryUtil library
    /// </summary>
    public class Program
    {
        /// <summary>
        ///  Entry point to the program
        /// </summary>
        /// <param name="args">command-line arguments can be a term, a space-separated list of terms, or a path to a file that contains a list of terms</param>
        public static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.Unicode;

            if (args.Length == 0)
            {
                Hold("Supply a file path as command line argument. Press any key to close.");
                return;
            }

            // Hold("\nPress any key to start.");
            string filePath = args.First();

            // the command-line arguments can be one filepath or a space-separated list of words
            // var words = File.Exists(filePath) ? File.ReadAllText(filePath, Encoding.UTF8).Split('\n').Select(w => w.Trim()).OrderBy<string, Guid>(line => Guid.NewGuid()).ToArray() : args;
            var words = File.Exists(filePath) ? File.ReadAllText(filePath, Encoding.UTF8).Split('\n').Select(w => w.Trim()).ToArray() : args;

            foreach (string word in words)
            {
                TestWord(word);

                // Timer(1000);
                if (Console.KeyAvailable)
                {
                    //var fiSections = string.Join(", ", fiSectionHeadings.OrderBy(s => s));
                    //var enSections = string.Join(", ", enSectionHeadings.OrderBy(s => s));

                    //Console.WriteLine("fiCount: {0} enCount: {1} fiSections: {2} enSections: {3}", isFiCount, isEnCount, fiSections, enSections);
                    Hold("\nPress any key to continue.");
                }
            }

            Hold("\nPress any key to close.");
        }

        private static void TestWord(string term)
        {
            //var termObj = Wiktionary.GetTerm(term);
            TermToDatabase.InsertTermToDatabase(term);

            //var termJson = JsonConvert.SerializeObject(termObj, Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            Console.Write("{0} ", term);

        }

        //private static void TestWord(string word)
        //{
        //    if (word.StartsWith("//"))
        //    {
        //        return;
        //    }

        //    // Console.Write("\r{0}".PadRight(70 - word.Length) + "fi:{1} en:{2} total:{3}   ", word, isFiCount, isEnCount, total);
        //    total++;

        //    FiFiPage fifiPage = FiFiPage.GetPage(word);
        //    bool isFi = !fifiPage.IsEmpty();
        //    bool hasFinnish = fifiPage.IsFinnish();
        //    if (isFi && hasFinnish)
        //    {
        //        isFiCount++;

        //        // TODO: 'ununbium' has no entries, therefore the scraper does not pick up its definition.  The scraper should get its information, regardless of its being in an 'entry' or not.

        //        // var fiSections = fifiPage.entries == null? Enumerable.Empty<string>() : fifiPage.entries.SelectMany(e => e.sections).Select(s => s.heading);
        //        // fiSectionHeadings = fiSectionHeadings.Concat(fiSections).Distinct();
        //        var fifiObj = fifiPage.GetJsonObject();
        //        var fifiJson = JsonConvert.SerializeObject(fifiObj, Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
        //        Console.Write(fifiJson);

        //        // Console.WriteLine(",");

        //        // fifiPage.Output();
        //    }

        //    EnFiPage enfiPage = EnFiPage.GetPage(word);
        //    bool isEn = !enfiPage.IsEmpty();
        //    bool isFinnish = enfiPage.IsFinnish();
        //    if (isEn && isFinnish)
        //    {
        //        isEnCount++;

        //        // enfiPage.Output();

        //        // var enSections = enfiPage.entries == null ? Enumerable.Empty<string>() : enfiPage.entries.SelectMany(e => e.sections).Select(s => s.heading);
        //        // enSectionHeadings = enSectionHeadings.Concat(enSections).Distinct();
        //        var enfiJsonObj = enfiPage.GetJsonObject();
        //        var enfiJson = JsonConvert.SerializeObject(enfiJsonObj, Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
        //        Console.WriteLine(enfiJson);

        //        // Console.WriteLine(",");
        //    }
        //}

        private static void Hold(string p)
        {
            Output(p);
            while (Console.KeyAvailable)
            {
                Console.ReadKey(true);
            }

            Console.ReadKey();
        }

        private static void Output(string p)
        {
            Console.Out.WriteLine(p);
        }

    }



}
