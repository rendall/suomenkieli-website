using Suomenkieli;
using SuomenkieliWebsite.Models;
using SuomenkieliWebsite.Models.Subtitles;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace SuomenkieliWebsite.Controllers
{
    public class SubtitlesController : Controller
    {
        private const int FI = 1;
        private const int EN = 2;
        private const string FILE_PATH = "/Files";

        private static readonly Regex srtTime = new Regex(@"^\d{2}:\d{2}:\d{2},\d{3}\s{1}-->\s{1}\d{2}:\d{2}:\d{2},\d{3}$");

        // GET: Subtitles
        public ActionResult Index()
        {
            List<string> Links = SuomenkieliRepository.db.Fetch<string>("SELECT [Link] FROM [SuomenkieliDB].[files].[SubtitleCompares]");
            return View(Links);
        }

        // GET: Subtitles/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Subtitles/Create
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Upload(SubtitlesFileUploadViewModel form)
        {
            try
            {
                // parse subtitles 
                string finResult = new StreamReader(form.FinnishSubtitleFile.InputStream, Encoding.GetEncoding(1250)).ReadToEnd();
                SubtitleList finSubList = srtToSubtitleList(finResult);

                string engResult = new StreamReader(form.EnglishSubtitleFile.InputStream, Encoding.GetEncoding(1250)).ReadToEnd();
                SubtitleList enSubList = srtToSubtitleList(engResult);

                finSubList.CompareTo(enSubList);
                // send to 'compare'

                SubtitleCompareVM vm = new SubtitleCompareVM();
                vm.leftList = finSubList;
                vm.rightList = enSubList;

                string id = SaveSubtitleFiles(form);


                return RedirectToAction("Compare", new { controller = "Subtitles", action = "Compare", id=id });
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                return HttpNotFound(e.Message);
            }
        }



        //public ActionResult CrossLoad()
        //{
        //    return View();
        //}

        //[HttpPost]
        //public ActionResult CrossLoad(SubtitlesURLsViewModel form)
        ////public ActionResult Create(FormCollection collection)
        //{
        //    try
        //    {
        //        WebClient downloader = new WebClient();
        //        string finSrt = downloader.DownloadString(form.FinnishSubtitleURL);
        //        // TODO: Add insert logic here
        //        string engSrt = downloader.DownloadString(form.EnglishSubtitleURL);

        //        return RedirectToAction("Index");
        //    }
        //    catch
        //    {
        //        return View();
        //    }
        //}

        // GET: Subtitles/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Subtitles/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Subtitles/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Subtitles/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        private SubtitleList srtToSubtitleList(string fileContent)
        {
            //TODO: detect encoding better.  Assuming encoding is correct...

            // assuming file is .srt type.



            SubtitleList subList = new SubtitleList();

            int entryNum = 0;
            int entryStatus = 0; // 0 = expecting new entry, 1 = expecting time, 2 = reading text until empty line.
            SubtitleEntry entry = null;

            using (StringReader reader = new StringReader(fileContent))
            {
                string line;
                int lineNum = 0;


                while ((line = reader.ReadLine()) != null)
                {
                    Debug.WriteLine(lineNum + ": " + line);

                    switch (entryStatus)
                    {
                        case 0:
                            entryNum++;
                            if (int.Parse(line) != entryNum) throw new Exception("Bad format in line " + lineNum + ". Expected " + entryNum + " got " + line);
                            entry = new SubtitleEntry();
                            subList.Add(entry);
                            entry.Index = entryNum;
                            entryStatus = 1;
                            break;
                        case 1:
                            if (!srtTime.IsMatch(line)) throw new Exception("Bad format in time line " + lineNum);
                            string startStr = "1/1/0001 " + line.Substring(0, 12).Replace(',', '.');
                            string endStr = "1/1/0001 " + line.Substring(17, 12).Replace(',', '.');


                            DateTime st = Convert.ToDateTime(startStr);

                            entry.StartTime = st.Ticks;

                            DateTime et = Convert.ToDateTime(endStr);
                            entry.EndTime = et.Ticks;

                            entryStatus = 2;
                            break;

                        case 2:
                            // all of these lines until empty string is the entry string


                            if (String.IsNullOrEmpty(line))
                            {
                                entryStatus = 0;
                            }
                            else
                            {
                                var textLine = StripHTML(line);
                                if (String.IsNullOrEmpty(entry.Text)) entry.Text = textLine;
                                else entry.Text += " " + textLine;


                                
                            }

                            break;



                        default:
                            break;
                    }

                    lineNum++;
                }
            }

            return subList;
        }

        private string SaveSubtitleFiles(SubtitlesFileUploadViewModel form)
        {
            HttpPostedFileBase fiFile = form.FinnishSubtitleFile;
            HttpPostedFileBase enFile = form.EnglishSubtitleFile;


            string fiFullPath = Path.Combine(Server.MapPath(FILE_PATH), fiFile.FileName);
            string enFullPath = Path.Combine(Server.MapPath(FILE_PATH), enFile.FileName);

            fiFile.SaveAs(fiFullPath);
            enFile.SaveAs(enFullPath);

            SubtitleFile fiSubFile = new SubtitleFile();
            fiSubFile.Filename = fiFile.FileName;
            fiSubFile.LanguageID = FI;

            int fiID = (int)fiSubFile.Insert();

            SubtitleFile enSubFile = new SubtitleFile();
            enSubFile.Filename = enFile.FileName;
            enSubFile.LanguageID = EN;

            int enID = (int)enSubFile.Insert();

            string subLink = "";

            {
                subLink = RandomString(5);

            } while (SubtitleCompare.Exists("WHERE Link=@0", subLink)) ;

            SubtitleCompare compare = new SubtitleCompare();
            compare.Link = subLink;
            compare.SubtitleFileA = Math.Min(fiID, enID);
            compare.SubtitleFileB = Math.Max(fiID, enID);
            compare.Title = "Title";
            compare.Insert();

            return subLink;
        }

        public ActionResult Compare(string id)
        {
            SubtitleCompare st = SubtitleCompare.SingleOrDefault("WHERE Link=@0", id);
            if (st == null || st.SubtitleFileA == 0 || st.SubtitleFileB == 0) return RedirectToAction("Index");

            SubtitleFile A = SubtitleFile.FirstOrDefault("WHERE ID=@0", st.SubtitleFileA);
            SubtitleFile B = SubtitleFile.FirstOrDefault("WHERE ID=@0", st.SubtitleFileB);

            SubtitleFile fiFile = A.LanguageID == FI ? A : B;
            string fiFullPath = Path.Combine(Server.MapPath(FILE_PATH), fiFile.Filename);
            SubtitleFile enFile = A.LanguageID == EN ? A : B;
            string enFullPath = Path.Combine(Server.MapPath(FILE_PATH), enFile.Filename);

            if (!System.IO.File.Exists(fiFullPath) || !System.IO.File.Exists(enFullPath)) return HttpNotFound("File not found on server.");

            string fiFileContent = System.IO.File.ReadAllText(fiFullPath, Encoding.GetEncoding(1250));
            SubtitleList fiSubList = srtToSubtitleList(fiFileContent);


            string enFileContent = System.IO.File.ReadAllText(enFullPath, Encoding.GetEncoding(1250));
            SubtitleList enSubList = srtToSubtitleList(enFileContent);

            fiSubList.CompareTo(enSubList);
            // send to 'compare'

            SubtitleCompareVM vm = new SubtitleCompareVM();
            vm.leftList = fiSubList;
            vm.rightList = enSubList;

            return View("Compare", vm);

        }

        public static string RandomString(int Size)
        {
            string input = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            StringBuilder builder = new StringBuilder();
            Random random = new Random();
            char ch;
            for (int i = 0; i < Size; i++)
            {
                ch = input[random.Next(0, input.Length)];
                builder.Append(ch);
            }
            return builder.ToString();
        }

        public static string StripHTML(string source)
        {
            source = Regex.Replace(source, "<.*?>", " ");
            source = Regex.Replace(source, @"\s+", " ").Trim();
            return source;
        }
    }
}
