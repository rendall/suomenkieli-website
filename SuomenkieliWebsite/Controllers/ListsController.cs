using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Suomenkieli;
using SuomenkieliWebsite.Models;

namespace SuomenkieliWebsite.Controllers
{
    public class ListsController : SuomenkieliController
    {

        // GET: Lists
        public ActionResult Index()
        {

            IEnumerable<VocabularyList> vlList = db.Fetch<VocabularyList>("SELECT * FROM VocabularyLists WHERE OwnerID=@0", UserID);
            return View(vlList);
        }

        // GET: Lists/Details/5
        // ID should be the URL for the list
        public ActionResult Details(int id)
        {
            VocabularyListVM vl = GetVocabularyListVW(id);
            if (vl == null) RedirectToActionPermanent("Index");

            return View(vl);
        }

        // GET: Lists/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Lists/Create
        [HttpPost]
        public ActionResult Create(VocabularyList list)
        {
            try
            {

                list.OwnerID = UserID;
                int listID = (int)list.Insert();

                return RedirectToAction("Edit", new { id = listID });
            }
            catch (Exception ex)
            {

                return HttpNotFound(ex.Message);
            }
        }

        // GET: Lists/Edit/5
        public ActionResult Edit(int id)
        {

            VocabularyListVM vl = GetVocabularyListVW(id);
            if (vl == null) RedirectToActionPermanent("Index");

            return View(vl);
        }



        // POST: Lists/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, VocabularyListVM list)
        {
            try
            {
                VocabularyListVM vl = GetVocabularyListVW(id);
                if (vl == null) RedirectToActionPermanent("Index");

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Lists/Delete/5
        public ActionResult Delete(int id)
        {
            VocabularyListVM vl = GetVocabularyListVW(id);
            if (vl == null) RedirectToActionPermanent("Index");




            return View(vl);
        }

        // POST: Lists/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                VocabularyListVM vl = GetVocabularyListVW(id);
                if (vl == null) RedirectToActionPermanent("Index");
                vl.Delete();
            }
            catch
            {

            }
            return RedirectToAction("Index");
        }

        

        private VocabularyListVM GetVocabularyListVW(int id)
        {
            VocabularyList vl = VocabularyList.First("WHERE id=@0 AND OwnerID=@1", id, UserID);
            if (vl == null) return null;

            VocabularyListVM vlm = new VocabularyListVM(vl);


            //List<VocabularyList_Word> vlWords = VocabularyList_Word.Fetch("WHERE VocabularyListID = @0 ORDER BY Rank", id);

            //int[] wordIDs = vlWords.Select(t => t.WordID).ToArray();
            List<FullWord> vlFW = SuomenkieliRepository.GetFullWordListByListID(vlm.ID);


            vlm.WordList = vlFW;

            return vlm;
        }

        private IEnumerable<VocabularyListVM> GetListOfVocabularyListVM(int[] ids)
        {
            IEnumerable<VocabularyList> vLists = VocabularyList.Fetch("WHERE OwnerID=@0 AND ID IN @1", UserID, ids);

            List<VocabularyListVM> vmLists = new List<VocabularyListVM>();

            foreach (VocabularyList vList in vLists)
            {
                VocabularyListVM vmList = new VocabularyListVM(vList);
                vmList.WordList = SuomenkieliRepository.GetFullWordListByListID(vmList.ID);
                vmLists.Add(vmList);

            }

            return vmLists;

        }
    }
}
