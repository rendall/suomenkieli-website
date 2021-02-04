using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Microsoft.AspNet.Identity;
using SuomenkieliWebsite.Models;
using Suomenkieli;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Net.Http.Headers;

namespace SuomenkieliWebsite.Controllers.API
{
    public class SuomenKieliAPIController:ApiController
    {
        private static SuomenkieliDB _db;

        internal string UserID
        {


            get
            {
                return User.Identity.GetUserId();
            }
        }

        internal static SuomenkieliDB db
        {
            get
            {
                if (_db == null) _db = new SuomenkieliDB();
                return _db;
            }
        }

        internal HttpResponseMessage ResponseError(HttpStatusCode code, string message)
        {
            HttpResponseMessage response = Request.CreateResponse(code);
            response.Content = new StringContent(message, Encoding.Unicode);
            response.Headers.CacheControl = new CacheControlHeaderValue()
            {
                MaxAge = TimeSpan.FromMinutes(20)
            };
            return response;
        }

        internal HttpResponseMessage ResponseOK(string message = null)
        {
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK);
            if (!string.IsNullOrEmpty(message)) response.Content = new StringContent(message, Encoding.Unicode);
            response.Headers.CacheControl = new CacheControlHeaderValue()
            {
                MaxAge = TimeSpan.FromMinutes(20)
            };
            return response;
        }
    }
}