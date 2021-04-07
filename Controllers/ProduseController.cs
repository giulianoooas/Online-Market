using Microsoft.AspNet.Identity;
using ProiectAppWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ProiectAppWeb.Controllers
{
    public class ProduseController : Controller
    {
        private ApplicationDbContext app = new ApplicationDbContext();

        [Authorize(Roles = "User, Editor, Admin")]
        public ActionResult Index()
        {
            if (TempData.ContainsKey("nume"))
            {
                string nume = (string)TempData["nume"];
                ViewBag.Produse = from prod in app.Produse.Include("Categorie").Include("User")
                                  where prod.Titlu.Equals(nume)
                                  select prod;
                return View();
            }
            if (TempData.ContainsKey("afisare")) 
            {
                    if (TempData["afisare"] is 1)
                    {
                        ViewBag.Produse = from prod in app.Produse.Include("Categorie").Include("User")
                                          orderby prod.Titlu ascending
                                          select prod;
                        return View();
                    }
                    else
                    {
                        ViewBag.Produse = from prod in app.Produse.Include("Categorie").Include("User")
                                          orderby prod.Titlu descending
                                          select prod;
                        return View();
                    }
               
            }
           
            
            ViewBag.Produse = app.Produse.Include("Categorie").Include("User");
            return View();
            
            
        }


        /// Metoda de adaugare si echivaletul ei in CRUD

        [Authorize(Roles = "Editor, Admin")]
        public ActionResult Adauga()
        {
            ViewBag.cat = GetAllCategories();
            return View();
        }

        
        [HttpPost]
        [Authorize(Roles = "Editor, Admin")]
        public ActionResult Adauga(Produs nou)
        {
            ViewBag.cat = GetAllCategories();
            try
            {
                if (ModelState.IsValid)
                {
                    nou.UserId = User.Identity.GetUserId();
                    app.Produse.Add(nou);
                    app.SaveChanges();
                    TempData["mesajProdus"] = "adaugat";
                    return RedirectToAction("Index");
                }
                else
                {
                    return View(nou);
                }
            }
            catch (Exception e)
            {
                return View(nou);
            }
        }

        /// Acum fac metoda Delete, asta se va intampla direct in index
        
        [HttpPost]
        [Authorize(Roles = "User, Editor, Admin")]
        public ActionResult Cauta(string nume)
        {
            var produs = from prod in app.Produse
                                where prod.Titlu.Equals(nume)
                                select prod;
            if (produs.Count() >= 1)
            {
                if (produs.Count() == 1)
                foreach (var i in produs)
                {
                    return RedirectToAction("/Afiseaza/" + i.ProdusId);
                }
                TempData["nume"] = nume;
                return RedirectToAction("/Index");
            } else
            {
                TempData["atentie"] = "Nu avem produsul!";
                return RedirectToAction("/Index");
            } 
        }

        [HttpDelete]
        [Authorize(Roles = "Editor, Admin")]
        public ActionResult Sterge(int id)
        {
            try
            {
                Produs Sters = app.Produse.Find(id);
                if(Sters.UserId == User.Identity.GetUserId() || User.IsInRole("Admin"))
                {
                    app.Produse.Remove(Sters);
                    app.SaveChanges();
                    TempData["mesajProdus"] = "sters";
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["message"] = "Nu aveti dreptul sa faceti modificari asupra unui Produs care nu va apartine.";
                    return RedirectToAction("Index");
                }
                
            }
            catch (Exception e)
            {
                return RedirectToAction("Index");
            }
        }

        /// Acum fac partea de update
        [Authorize(Roles = "Editor, Admin")]
        public ActionResult Editeaza(int id)
        {
            ViewBag.cat = GetAllCategories();
            Produs pd = app.Produse.Find(id);
            if (pd.UserId == User.Identity.GetUserId() || User.IsInRole("Admin"))
            {
                return View(pd);
            }
            else
            {
                TempData["message"] = "Nu aveti dreptul sa faceti modificari asupra unui Produs care nu va apartine.";
                return RedirectToAction("Index");
            }
        }

        
        [HttpPut]
        [Authorize(Roles = "Editor, Admin")]
        public ActionResult Editeaza(int id, Produs nou)
        {
            ViewBag.cat = GetAllCategories();
            try
            {
                if (ModelState.IsValid)
                {
                    Produs actual = app.Produse.Find(id);
                    if (TryUpdateModel(actual))
                    {
                        actual.CategorieId = nou.CategorieId;
                        actual.Descriere = nou.Descriere;
                        actual.Pret = nou.Pret;
                        actual.Titlu = nou.Titlu;
                        TempData["mesajProdus"] = "editat";
                        app.SaveChanges();
                    }
                    return RedirectToAction("Index");
                }
                else
                {
                    return View(nou);
                }
            }
            catch (Exception e)
            {
                return View(nou);
            }
        }

        /// arat fiecare produs individual
        [Authorize(Roles = "User, Editor, Admin")]
        public ActionResult Afiseaza(int id)
        {
            if (TempData.ContainsKey("Rez" + id))
                TempData["Ascundele" + id] = TempData["Rez" + id]; //// acest temp data de rezerva memoreaza daca mai trebuie sa afisez sau nu comentariile la refresh
            ViewBag.Ascundele = false;
            if (TempData.ContainsKey("Ascundele" + id))
            {
                if (TempData["Ascundele" + id] is true)
                {
                    ViewBag.Ascundele = true;
                    ViewBag.Rew = null;
                    TempData["Rez" + id] = true;
                }

            }

            if (TempData.ContainsKey("ORD"))
            {
                if (TempData["ORD"] is 1)
                {
                    ViewBag.Rew = from rew in app.Reviews.Include("User")
                                  where rew.ProdusId.Equals(id)
                                  orderby rew.nrStars ascending
                                  select rew;
                }
                else
                {
                    ViewBag.Rew = from rew in app.Reviews.Include("User")
                                  where rew.ProdusId.Equals(id)
                                  orderby rew.nrStars descending
                                  select rew;
                }
            }
            else
            {
                ViewBag.Rew = from rew in app.Reviews.Include("User")
                              where rew.ProdusId.Equals(id)
                              select rew;
            }

            Produs prod = app.Produse.Find(id);
            ViewBag.lista = GetStars();
            return View(prod);
        }

        /// Metodele NonAction
        [NonAction] /// returnez o lista Enumerabila peste categorii
        private IEnumerable<SelectListItem> GetAllCategories()
        {// generam o lista goala
            var selectList = new List<SelectListItem>();
            // extragem toate categoriile din baza de date
            var categories = from cat in app.Categorii select cat;
            // iteram prin categorii
            foreach (var category in categories)
            {
                // adaugam in lista elementele necesare pentru dropdown
                selectList.Add(new SelectListItem
                {
                    Value = category.CategorieId.ToString(),
                    Text = category.Titlu.ToString()
                });
            }
            return selectList;
        }

        [Authorize(Roles = "User, Editor, Admin")]
        public ActionResult Cresc()
        {
            TempData["afisare"] = 1;
            return RedirectToAction("Index");
        }

        [Authorize(Roles = "User, Editor, Admin")]
        public ActionResult Desc()
        {
            TempData["afisare"] = 2;
            return RedirectToAction("Index");
        }

        [NonAction]
        private IEnumerable<SelectListItem> GetStars()
        {
            var listaStele = new List<SelectListItem>(); /// lista stelelor de tip cheie valoare unde cheia = valoarea = numarul de stele de la 1 la 5
            for (int i = 1; i <= 5; i++)
            {
                listaStele.Add(
                    new SelectListItem
                    {
                        Value = i.ToString(),
                        Text = i.ToString()
                    }
                );
            }
            return listaStele;
        }
    }
}