﻿using BE;
using BL;
using Medical.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Medical.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Index(FormCollection collection)
        {
            DoctorsModel model = new DoctorsModel();
            try
            {
                RouteConfig.user=model.ReturnUser(collection["uname"], collection["psw"]);
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
        public ActionResult Logout()
        {
            RouteConfig.user = null;
            return RedirectToAction("Index");
        }
        public ActionResult Catalog()
        {
            MedicineModel model = new MedicineModel();
            var Medicines = model.GetMedicines();
            return View(Medicines);
        }
        // GET: Home/Details/5
        public ActionResult Details(int id)
        {
           MedicineModel model = new MedicineModel();
           List<string> ListChart = model.GetMedicineChart(id);
           return View(ListChart);
        }

        // GET: Home/Create
        //Add new Medicine
        public ActionResult Create()
        {
            var IVM = new ImageViewModel();
            return View(IVM);
        }

        // POST: Home/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {

            //Check NDC
            var ndc = collection["NDC"];
            LicensedMedicineLogic NDCs = new LicensedMedicineLogic();
            if (NDCs.GetMedicineByNdc(ndc).Count() == 0) //there is no such NDC
            {
                ViewBag.message2 = "There is no such a medicine NDC!";
                return View();
            }

            //Drugs Service
            var id = collection["NDC"];
            DrugsLogic DL = new DrugsLogic();
            PrescriptionsLogic BL = new PrescriptionsLogic();
            var NDC_List = BL.GetNDCById(id);
            ViewBag.drugsService = DL.GetDrugsResults(NDC_List.ToArray());
            MedicineModel model = new MedicineModel();


            ViewBag.message1 = model.Add(collection["CommercialName"], collection["GenericName"], collection["Producer"], collection["ActiveIngredients"], collection["DoseCharacteristic"], collection["NDC"]);
            return RedirectToAction("Catalog");
        
                         

        }
        public ActionResult AddImage(int id, HttpPostedFileBase file)
        {
            //Images Service
            MedicineModel model = new MedicineModel();            
            var ImgPath = file.FileName;
            var path = Server.MapPath(Url.Content($"~/images/{ImgPath}"));
            ImageTagsLogic bl = new ImageTagsLogic();
            List<string> tags = bl.GetTags(path); //check images with Imagga

            //Add
            if (tags.Intersect(bl.DrugsTags).Any())
            {
                model.AddImage(file, id);
                return RedirectToAction("Catalog");
            }
            else
            {
                ViewBag.message = "The image that you added isn't a medicine!";
                return View();
            }

        }

        public ActionResult Edit(int id)
        {
            MedicineModel model = new MedicineModel();
            var Medicine = model.GetMedicines().First(m => m.Id == id);
            return View(Medicine);
        }

        // POST: Home/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            MedicineModel model = new MedicineModel();
            try
            {
                model.Update(id, collection["CommercialName"], collection["GenericName"], collection["Producer"], collection["ActiveIngredients"], collection["DoseCharacteristic"], collection["image"], collection["NDC"]);
                return RedirectToAction("Catalog");
            }
            catch
            {
                return View();
            }
        }

        // GET: Home/Delete/5
        public ActionResult Delete(int id)
        {
            MedicineModel model = new MedicineModel();
            model.delete(id);
            return RedirectToAction("Catalog");
        }

    }
}
