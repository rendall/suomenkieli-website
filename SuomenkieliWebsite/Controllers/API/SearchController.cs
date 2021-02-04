using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using PetaPoco;
using Suomenkieli;
using SuomenkieliWebsite.Models;

namespace SuomenkieliWebsite.Controllers.API
{
    public class SearchController : ApiController
    {
        private static SuomenkieliDB _db;

        // GET: api/Search
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/Search/5
        public List<String> Get(string id)
        {
            if (String.IsNullOrWhiteSpace(id)) return null;

            var searchTerm = id + "%";

            Sql searchSql = new Sql(@"SELECT TOP 20 Word
                FROM Words
                WHERE Word LIKE @0
                ORDER BY LEN(Word) ASC, Popularity DESC", searchTerm);

            if (db.Connection != null && db.Connection.State != System.Data.ConnectionState.Closed) return null;

            List<String> search = db.Fetch<String>(searchSql);

            return search;

        }

        // POST: api/Search
        public void Post([FromBody]string value)
        {
        }

        // PUT: api/Search/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/Search/5
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
    }
}
