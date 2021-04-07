using Microsoft.AspNet.Identity;
using ProiectAppWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ProiectAppWeb.Controllers
{
    [Authorize(Roles = "User, Admin")]
    public class ReviewController : Controller
    {
        private ApplicationDbContext app = new ApplicationDbContext();

        /// partea de read a fost facuta in controller-ul pentru produse

        /// metoda asta adauga un review nou direct din pagina produsului
        [HttpPost]
        public ActionResult Add(int id, Review nou)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    nou.Data = DateTime.Now;
                    nou.ProdusId = id;
                    nou.nrLikes = 0;
                    nou.nrDislikes = 0;
                    nou.UserId = User.Identity.GetUserId();
                    app.Reviews.Add(nou);
                    app.SaveChanges();
                    TempData["mesajReview"] = "adaugat";
                    return Redirect("/Produse/Afiseaza/" + id);
                }
                else
                {
                    TempData["EroareAdaugareCometariu"] = true;
                    return Redirect("/Produse/Afiseaza/" + id);
                }
            }
            catch (Exception e)
            {
                return Redirect("/Produse/Afiseaza/" + id);
            }
        }

        /// Cu netoda asta voi sterge direct din pagina produsului comentariul
        [HttpDelete]
        public ActionResult Delete(int id)
        {
            Review rew = app.Reviews.Find(id);
            int ProdusId = rew.ProdusId;
            try
            {
                if(rew.UserId == User.Identity.GetUserId() || User.IsInRole("Admin"))
                {
                    app.Reviews.Remove(rew);
                    app.SaveChanges();
                    TempData["mesajReview"] = "sters";
                    return Redirect("/Produse/Afiseaza/" + ProdusId);
                }
                else
                {
                    TempData["message"] = "Nu aveti dreptul sa faceti modificari asupra unui Review care nu va apartine.";
                    return Redirect("/Produse/Afiseaza/" + ProdusId);
                }
            }
            catch (Exception e)
            {
                return Redirect("/Produse/Afiseaza/" + ProdusId);
            }
        }

        /// aici voi face partea de update pentru cometarii
        public ActionResult Edit(int id)
        {
            Review editare = app.Reviews.Find(id);

            Produs produs = app.Produse.Find(editare.ProdusId);

            if(editare.UserId == User.Identity.GetUserId() || User.IsInRole("Admin"))
            {
                ViewBag.Produs = app.Produse.Find(editare.ProdusId);
                ViewBag.lista = GetStars();
                return View(editare);
            }
            else
            {
                TempData["message"] = "Nu aveti dreptul sa faceti modificari asupra unui Review care nu va apartine.";
                return Redirect("/Produse/Afiseaza/" + produs.ProdusId);
            }

            
        }

        [HttpPut]
        public ActionResult Edit(int id, Review editat)
        {
            editat.ReviewId = id;
            ViewBag.Produs = app.Produse.Find(app.Reviews.Find(id).ProdusId);
            ViewBag.lista = GetStars();
            try
            {
                if (ModelState.IsValid)
                {
                    Review vechi = app.Reviews.Find(id);
                    if (vechi.UserId == User.Identity.GetUserId() || User.IsInRole("Admin"))
                    {
                        if (TryUpdateModel(vechi))
                        {
                            vechi.Data = DateTime.Now;
                            vechi.Content = editat.Content;
                            vechi.nrStars = editat.nrStars;
                            app.SaveChanges();
                            TempData["mesajReview"] = "editat";
                        }
                        return Redirect("/Produse/Afiseaza/" + vechi.ProdusId);
                    }
                    else
                    {
                        TempData["message"] = "Nu aveti dreptul sa faceti modificari asupra unui Review care nu va apartine.";                        return Redirect("/Produse/Afiseaza/" + vechi.ProdusId);
                    }
                }
                else
                {
                    return View(editat);
                }
            }
            catch (Exception e)
            {
                return View(editat);
            }
        }

        /// aceste 2 functii le voi folosi pentru ordonare
        public ActionResult Crescator(int id)
        {
            TempData["ORD"] = 1;
            return Redirect("/Produse/Afiseaza/" + id);
        }

        public ActionResult Descrescator(int id)
        {
            TempData["ORD"] = 2;
            return Redirect("/Produse/Afiseaza/" + id);
        }

        /// functia care ascunde comentarile
        public ActionResult Ascunde(int id)
        {
            TempData["Ascundele" + id] = true;
            return Redirect("/Produse/Afiseaza/" + id);
        }

        /// functia care imi afiseaza cometariile
        public ActionResult Afiseaza(int id)
        {
            TempData["Rez" + id] = false;
            return Redirect("/Produse/Afiseaza/" + id);
        }

        /// Functiile pentru like si dislike

        public ActionResult Like(int id)
        {
            Review r = app.Reviews.Find(id);
            r.nrLikes++;
            app.SaveChanges();
            return Redirect("/Produse/Afiseaza/" + r.ProdusId);
        }

        public ActionResult Dislike(int id)
        {
            Review r = app.Reviews.Find(id);
            r.nrDislikes++;
            app.SaveChanges();
            return Redirect("/Produse/Afiseaza/" + r.ProdusId);
        }

        /// Aici vor sta metodele NonAction
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