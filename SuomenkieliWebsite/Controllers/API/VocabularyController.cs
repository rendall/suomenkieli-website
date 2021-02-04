using PetaPoco;
using Suomenkieli;
using SuomenkieliWebsite.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Mvc;



namespace SuomenkieliWebsite.Controllers.API
{
    public class VocabularyController : SuomenKieliAPIController
    {
        // GET: api/Vocabulary
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/Vocabulary/5
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/Vocabulary
        [ValidateAntiForgeryToken]
        //[System.Web.Http.HttpPost]
        public HttpResponseMessage Post([FromBody] VocabularyWord model)
        {
            if (UserID == null) return ResponseError(HttpStatusCode.Forbidden, "Log in required.");
            Sql sql = new Sql("WHERE OwnerID=@0", UserID);

            int listID = 0;



            if (model.list != null && int.TryParse(model.list, out listID)) sql.Append("AND ID=@0", listID);

            sql.Append("ORDER BY ID DESC");

            VocabularyList vc = VocabularyList.FirstOrDefault(sql);



            if (vc == null) return ResponseError(HttpStatusCode.PreconditionFailed, "No vocabulary list found.");

            try
            {
                SuomenkieliRepository.AddVocabularyWord(model.word, vc.ID, UserID);

            }
            catch (Exception ex)
            {

                return ResponseError(HttpStatusCode.InternalServerError, ex.Message);
            }


            int wordID = SuomenkieliRepository.GetWordID(model.word).GetValueOrDefault(); // This should not be null.

            FullWord fWord = SuomenkieliRepository.GetFullWord(wordID);

            string firstDef = (fWord.Definitions.Count > 0) ? fWord.Definitions.First()._Definition : "";

            ResponseWord rWord = new ResponseWord() { id=fWord.ID, word = fWord._Word, definition = firstDef };

            var resp = Request.CreateResponse<ResponseWord>(HttpStatusCode.OK, rWord);

            return resp;

            //return ResponseOK(rWord);
        }



        // PUT: api/Vocabulary/5
        [ValidateAntiForgeryToken]
        public void Put([FromBody]ListOrder value)
        {
           Debug.Write(value);
            SuomenkieliRepository.OrderVocabularyList(value.list, value.order);
        }

        // DELETE: api/Vocabulary/5
        [ValidateAntiForgeryToken]
        public HttpResponseMessage Delete([FromBody] VocabularyWord model)
        {

            if (UserID == null) return ResponseError(HttpStatusCode.Forbidden, "Log in required.");
            Sql sql = new Sql("WHERE OwnerID=@0", UserID);

            int listID = 0;


            if (model.list != null && int.TryParse(model.list, out listID)) sql.Append("AND ID=@0", listID);

            sql.Append("ORDER BY ID DESC");

            VocabularyList vc = VocabularyList.FirstOrDefault(sql);



            if (vc == null) return ResponseError(HttpStatusCode.PreconditionFailed, "No vocabulary list found.");

            try
            {
                SuomenkieliRepository.RemoveVocabularyWord(model.word, vc.ID, UserID);

            }
            catch (Exception ex)
            {

                return ResponseError(HttpStatusCode.InternalServerError, ex.Message);
            }

            return ResponseOK(model.word);
        }


    }

    public class VocabularyWord
    {
        public string word { get; set; }
        public string list { get; set; }

        public VocabularyWord()
        {

        }
    }

    public class ResponseWord
    {
        public int id { get; set; }
        public string word { get; set; }
        public string definition { get; set; }
    }

    public class ListOrder
    {
        public string list { get; set; }
        public int[] order { get; set; }
    }


}
