using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace SuomenkieliWebsite.Controllers.API
{
    public class SentencesController : ApiController
    {
        // GET: api/Sentences
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/Sentences/5
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/Sentences
        public void Post([FromBody]string value)
        {
        }

        // PUT: api/Sentences/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/Sentences/5
        public void Delete(int id)
        {
        }
    }
}
