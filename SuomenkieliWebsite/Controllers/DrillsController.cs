using PetaPoco;
using Suomenkieli;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SuomenkieliWebsite.Controllers
{
    public class DrillsController : SuomenkieliController
    {
        // GET: Drills
        public ActionResult Index()
        {
            List<Relationship> rels = Relationship.Fetch("WHERE ID <= 190");
            return View(rels);

        }

        // GET: Drills/Details/5
        public ActionResult Drill(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index", "Drills");
                
            }

            Sql selectSql = new Sql(@"SELECT TOP 10
                    BaseWords.Word AS BaseWord, 
                    Definitions.Definition,
                    Relationships.Relationship,
                    RelatedWords.Word AS RelatedWord 
                    FROM WordRelationships
                    INNER JOIN Words AS BaseWords ON WordRelationships.WordB = BaseWords.ID
                    INNER JOIN Words AS RelatedWords ON WordRelationships.WordA = RelatedWords.ID
                    INNER JOIN Word_Definition ON BaseWords.ID = Word_Definition.WordID
                    INNER JOIN Definitions ON Word_Definition.DefinitionID = Definitions.ID
                    INNER JOIN Relationships ON WordRelationships.RelationshipID = Relationships.ID
                    WHERE RelationshipID = @0
                    ORDER BY NEWID()", id);

            List<DrillRow> drillList = db.Fetch<DrillRow>(selectSql);

            return View(drillList);
        }

        // GET: Drills/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Drills/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Drills/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Drills/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Drills/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Drills/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }

    public class DrillRow
    {
        public string BaseWord { get; set; }
        public string Definition { get; set; }
        public string Relationship { get; set; }
        public string RelatedWord { get; set; }
    }
}
