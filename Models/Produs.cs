using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ProiectAppWeb.Models
{
    public class Produs
    {
        [Key]
        public int ProdusId { get; set; }
        [Required(ErrorMessage = "Produsul are nevoie de un titlu")]
        public String Titlu { get; set; }
        [Required(ErrorMessage = "Produsul are nevoie de o descriere")]
        [DataType(DataType.MultilineText)]
        public String Descriere { get; set; }
        [Required(ErrorMessage = "Produsul are nevoie de o poza")]
        public String urlPoza { get; set; }
        [Required(ErrorMessage = "Produsul are nevoie de un pret")]
        public float Pret { get; set; }
        [Required(ErrorMessage = "Produsul trebuie sa apartina unei categorii")]
        public int CategorieId { get; set; }
        public string UserId { get; set; }
        


        /// aici vor fi legaturile
        public virtual Categorie Categorie { get; set; }
        public virtual ICollection<Review> Reviews { get; set; }
        public virtual ApplicationUser User { get; set; }
    }
}