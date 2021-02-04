using NLog;
using SuomenkieliWebsite.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;

namespace SuomenkieliWebsite.Controllers
{



    public class HomeController : SuomenkieliController
    {

        private static Logger _searchLogger;

        private static Logger searchLogger
        {
            get
            {
                if (_searchLogger == null) _searchLogger = LogManager.GetLogger("SearchLogger");
                return _searchLogger;
            }
        }

        public ActionResult Index(string q)
        {
            ExploreVM evm = null;

            if (!String.IsNullOrWhiteSpace(q)) evm = UpdateConceptSet(new ExploreVM() { search = q });
            else evm = (ExploreVM)TempData["conceptSet"];



            return View(evm);
        }

        public ActionResult Explore(string id)
        {


            ExploreVM ex = new ExploreVM();
            //if (!id.Contains('*')) id = id + '*';
            ex.search = id;

            ExploreVM retSet = UpdateConceptSet(ex);

            TempData["conceptSet"] = retSet;

            return RedirectToAction("Index");

        }

        // POST: Words/Explore
        [HttpPost]
        public ActionResult Explore(ExploreVM conceptSet)
        {
            // try
            //{
            // TODO: Add insert logic here
            //if (String.IsNullOrWhiteSpace(conceptSet.search)) return View();


            ExploreVM retSet = UpdateConceptSet(conceptSet);

            TempData["conceptSet"] = retSet;

            return RedirectToAction("Index");
            // }
            //catch (Exception ex)
            //{
            //    if (User.IsInRole("Administrator"))
            //    {
            //        System.Diagnostics.Debug.Write(ex);
            //        throw new HttpException(500, ex.Message);
            //        //


            //    }
            //    ViewBag.error = ex.Message;
            //    return RedirectToAction("Index");
            //}
        }

        private ExploreVM UpdateConceptSet(ExploreVM conceptSet)
        {
            conceptSet.originalSearch = conceptSet.search;

            searchLogger.Info("search: {0} user: {1}", conceptSet.search, User.Identity.Name);

            conceptSet.isAdmin = User.IsInRole("Administrator");
            List<FullWord> results = SuomenkieliRepository.GetWordsByExplore(conceptSet);

            conceptSet.resultList = results;

            return conceptSet;
        }

        public ActionResult About()
        {
            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }




    }
}