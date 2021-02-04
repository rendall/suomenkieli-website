using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using PetaPoco;
using Suomenkieli;
using System.Text.RegularExpressions;
using SuomenkieliWebsite.Models;

namespace SuomenkieliWebsite.Controllers.API
{
    public class SentenceController : ApiController
    {

        const int FI = 1;
        const int EN = 2;

        // GET: api/Sentence
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/Sentence/5
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/Sentence
        public IHttpActionResult Post([FromBody]Dictionary<string, string> value)
        {

            try
            {



                // clean & check

                //  last character should be ".", "!", "?" and should be the same for both.

                char[] allowedPunctuation = new char[] { '.', '!', '?' };

                Dictionary<string, string> sentences = new Dictionary<string, string>();
                char endChar = '*';
                foreach (string langCode in value.Keys)
                {
                    string sentenceVal = value[langCode];
                    sentenceVal = Regex.Replace(sentenceVal, @"\s+", " ").Trim();

                    if (endChar != sentenceVal.Last() && endChar != '*') return Content(HttpStatusCode.BadRequest, "Both sentences must end with the same punctuation.");

                    endChar = sentenceVal.Last();
                    if (!allowedPunctuation.Contains(endChar))
                    {
                        return Content(HttpStatusCode.BadRequest, String.Format("{0} sentence {1} does not end in . ! or ?", langCode, sentenceVal));
                    }

                    sentences[langCode] = sentenceVal;

                }


                // -----
                // Now get / insert

                int fiID;
                int enID;

                Sentence fiSentence = Sentence.SingleOrDefault("WHERE Sentence=@0", sentences["fi"]);
                if (fiSentence == null)
                {
                    fiSentence = new Sentence();
                    fiSentence.LanguageID = FI; //TODO: Do this properly. Perhaps a Stored Procedure.
                    fiSentence._Sentence = sentences["fi"];
                    fiSentence.Source = 2; //TODO: Sources.
                    fiID = (int)fiSentence.Insert();
                    
                }
                else fiID = fiSentence.ID;

                Sentence enSentence = Sentence.SingleOrDefault("WHERE Sentence=@0", sentences["en"]);
                if (enSentence == null)
                {
                    enSentence = new Sentence();
                    enSentence.LanguageID = EN; //TODO: Do this properly. Perhaps a Stored Procedure.
                    enSentence._Sentence = sentences["en"];
                    enSentence.Source = 2; //TODO: Sources.
                    enID = (int)enSentence.Insert();
                }
                else enID = enSentence.ID;

                int AID = Math.Min(fiID, enID);
                int BID = Math.Max(fiID, enID);

                SentenceTranslation st = SentenceTranslation.SingleOrDefault("WHERE SentenceA=@0 AND SentenceB=@1", AID, BID);

                if (st != null) return Content(HttpStatusCode.BadRequest, "Sentence combination already exists.");

                st = new SentenceTranslation();
                st.SentenceA = AID;
                st.SentenceB = BID;

                st.Insert();

                ParseSentence(fiSentence);

                return Ok();

            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
                return Content(HttpStatusCode.BadRequest, e.Message);
            }



        }

        // Takes a sentence, breaks it into its component words & concepts.
        private void ParseSentence(Sentence s)
        {

            string[] words = s._Sentence.ToLower().Split(' ');
            string[] suffixes = new string[] { "mme", "kö" };


            foreach (string w in words)
            {
                string word = new string(w.Where(c => !char.IsPunctuation(c)).ToArray());

                Word W = Word.FirstOrDefault("WHERE Word=@0", word);

                // check if word is in database.  If not, shit gets complicated.

                bool wordNotFound = W == null;
                string wordNoSuffix;

                if (wordNotFound)
                {


                    //bool hasSuffix = suffixes.Any(x => word.EndsWith(x));

                    wordNoSuffix = word;
                    while (suffixes.Any(x => wordNoSuffix.EndsWith(x)))
                    { // strip suffixes from word, add suffix <-> sentence link to database
                      // suffixes cannot appear in any order, although this code treats them
                      // like they could.

                        foreach (string suffix in suffixes)
                        {
                            if (wordNoSuffix.EndsWith(suffix))
                            {
                                wordNoSuffix = wordNoSuffix.Substring(0, wordNoSuffix.LastIndexOf(suffix));

                                // just remove one, then check again.  We don't want to remove -ko from kirkko, for instance.
                            }

                            else continue;

                            W = Word.FirstOrDefault("WHERE Word=@0", wordNoSuffix);
                            if (W != null) break;
                            // if wordNoSuffix exists in the database, break;

                        }

                        if (W != null) break;
                    } // end remove suffix loop

                } // end if not found

                // if word exists then add word <-> sentence entry
                if (W != null)
                {
                    Sentence_Word sw = new Sentence_Word();
                    sw.WordID = W.ID;
                    sw.SentenceID = s.ID;
                    try
                    {
                        sw.Insert();
                    }
                    catch (Exception)
                    {
                        // likely means sentence_word already exists.
                    }
                }
                else 
                {
                    //TODO: add word to unknown word list.
                }

            } // end words loop


        }

        // PUT: api/Sentence/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/Sentence/5
        public void Delete(int id)
        {
        }

        public static SuomenkieliDB db
        {
            get
            {
                if (_db == null) _db = SuomenkieliRepository.db;
                return _db;
            }
        }

        public static SuomenkieliDB _db { get; private set; }
    }
}
