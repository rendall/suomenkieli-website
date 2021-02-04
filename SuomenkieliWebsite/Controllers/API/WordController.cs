using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Suomenkieli;
using PetaPoco;
using SuomenkieliWebsite.Models;

namespace SuomenkieliWebsite.Controllers
{

    public class WordController : ApiController
    {

        private static SuomenkieliDB _db;

        // GET: api/API
        public IEnumerable<string> Get()
        {

            return new string[] { "value1", "value2" };
        }

        // GET: api/API/5
        public string Get(string id)
        {

            if (db.Connection != null) db.Connection.Close();

            String definition = db.FirstOrDefault<String>(@"SELECT Definitions.Definition
                    FROM Definitions INNER JOIN
                         Word_Definition ON Definitions.ID = Word_Definition.DefinitionID INNER JOIN
                         Words ON Word_Definition.WordID = Words.ID
                    WHERE(Words.Word = @0)", id);

            return definition;
        }

        // POST: api/API
        public void Post([FromBody]string value)
        {
        }

        // PUT: api/API/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/API/5
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
