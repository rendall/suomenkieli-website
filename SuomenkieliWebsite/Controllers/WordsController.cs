//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Reflection;
//using System.Web;
//using System.Web.Mvc;
//using SuomenkieliWebsite.Models;

//namespace SuomenkieliWebsite.Controllers
//{
//    public class WordsController : SuomenkieliController
//    {

//        // GET: Words/Details/5
//        public ActionResult Index()
//        {
//            return View("Explore");
//        }

//        // GET: Words/Details/5
//        public ActionResult Details(int id)
//        {
//            return View();
//        }

//        // GET: Words/Index
//        public ActionResult Explore(string id)
//        {

//            if (string.IsNullOrWhiteSpace(id) && !User.IsInRole("Administrator"))
//            {
//                return View();
//            }
//            ExploreVM ex = new ExploreVM();
//            //if (!id.Contains('*')) id = id + '*';
//            ex.search = id;
//            return Explore(ex);

//        }

//        // POST: Words/Explore
//        [HttpPost]
//        public ActionResult Explore(ExploreVM conceptSet)
//        {
//           // try
//            //{
//                // TODO: Add insert logic here
//                //if (String.IsNullOrWhiteSpace(conceptSet.search)) return View();


//                conceptSet.originalSearch = conceptSet.search;
//                conceptSet.isAdmin = User.IsInRole("Administrator");
//                List<FullWord> results = SuomenkieliRepository.GetWordsByExplore(conceptSet);
                
//                conceptSet.resultList = results;

//                return View(conceptSet);
//           // }
//            //catch (Exception ex)
//            //{
//            //    if (User.IsInRole("Administrator"))
//            //    {
//            //        System.Diagnostics.Debug.Write(ex);
//            //        throw new HttpException(500, ex.Message);
//            //        //
                    

//            //    }
//            //    ViewBag.error = ex.Message;
//            //    return RedirectToAction("Index");
//            //}
//        }


//        // GET: Words/Edit/5
//        public ActionResult Edit(int id)
//        {
//            return View();
//        }

//        // POST: Words/Edit/5
//        [HttpPost]
//        public ActionResult Edit(int id, FormCollection collection)
//        {
//            try
//            {
//                // TODO: Add update logic here

//                return RedirectToAction("Index");
//            }
//            catch
//            {
//                return View();
//            }
//        }

//        // GET: Words/Delete/5
//        public ActionResult Delete(int id)
//        {
//            return View();
//        }

//        // POST: Words/Delete/5
//        [HttpPost]
//        public ActionResult Delete(int id, FormCollection collection)
//        {
//            try
//            {
//                // TODO: Add delete logic here

//                return RedirectToAction("Index");
//            }
//            catch
//            {
//                return View();
//            }
//        }
//    }

//}
