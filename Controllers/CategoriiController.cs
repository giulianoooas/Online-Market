using ProiectAppWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ProiectAppWeb.Controllers
{
    public class CategoriiController : Controller
    {
        private ApplicationDbContext app = new ApplicationDbContext();

        [Authorize(Roles = "User, Editor, Admin")]
        public ActionResult Index()
        {
            ViewBag.Categorii = app.Categorii;
            return View();
        }

        /// Aici vom face CRUD , mai exact update, delete si insert
        /// pagina pentru Insert si metoda aferenta din crud
        [Authorize(Roles = "Admin")]
        public ActionResult Add()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult Add(Categorie ct)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    app.Categorii.Add(ct);
                    app.SaveChanges();
                    TempData["mesajCategorie"] = "adaugata";
                    return RedirectToAction("Index");
                }
                else
                {
                    return View(ct);
                }
            }
            catch (Exception e)
            {
                TempData["EroareDeRepetareACategoerie"] = "Din pacate exista deja aceasta categorie.";
                return View(ct);
            }
        }

        /// pagina pentru Update si operatia CRUD
        [Authorize(Roles = "Admin")]
        public ActionResult Schimba(int id)
        {
            Categorie c = app.Categorii.Find(id);
            return View(c);
        }

        [HttpPut]
        [Authorize(Roles = "Admin")]
        public ActionResult Schimba(int id, Categorie c)
        {
            c.CategorieId = id;
            try
            {
                if (ModelState.IsValid)
                {
                    Categorie actual = app.Categorii.Find(id);
                    if (TryUpdateModel(actual))
                    {
                        actual.Titlu = c.Titlu;
                        app.SaveChanges();
                        TempData["mesajCategorie"] = "editata";
                    }
                    return RedirectToAction("Index");
                }
                else
                {
                    return View(c);
                }
            }
            catch (Exception e)
            {
                TempData["EroareDeRepetareACategoerie"] = "Din pacate exista deja aceasta categorie.";
                return View(c);
            }
        }

        /// Acum fac operatia de stergere
        [HttpDelete]
        [Authorize(Roles = "Admin")]
        public ActionResult Sterge(int id)
        {
            try
            {
                Categorie deSters = app.Categorii.Find(id);
                app.Categorii.Remove(deSters);
                app.SaveChanges();
                TempData["mesajCategorie"] = "eliminata";
                return RedirectToAction("Index");

            }
            catch (Exception e)
            {
                return RedirectToAction("Index");
            }
        }

        /// aici voi face afisearea Produselor dintr-o anumita categorie
        [Authorize(Roles = "User, Editor, Admin")]
        public ActionResult ProduseCategorie(int id)
        {
            ViewBag.Produse = from prod in app.Produse
                              where prod.CategorieId.Equals(id)
                              select prod;
            Categorie cat = app.Categorii.Find(id);
            return View(cat);
        }

    }
}