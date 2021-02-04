using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using SuomenkieliWebsite.Models;
using Suomenkieli;

namespace SuomenkieliWebsite.Controllers
{
    public class SuomenkieliController : Controller
    {
        internal SuomenkieliController()
        {

        }
        internal string UserID
        {


            get
            {
                return User.Identity.GetUserId();
            }
        }

        public static SuomenkieliDB db
        {
            get
            {
                return SuomenkieliRepository.db;
            }
        }
    }
}