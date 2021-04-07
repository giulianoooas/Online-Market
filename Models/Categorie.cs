using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ProiectAppWeb.Models
{
    public class Categorie
    {
        [Key]
        public int CategorieId { get; set; }
        [Required(ErrorMessage = "Categoria are nevoie de un nume")]
        [MaxLength(30, ErrorMessage = "Titlul este prea lung")]
        [Index(IsUnique = true)]
        public String Titlu { get; set; }

        /// aici fac legatura one-to-many
        public virtual ICollection<Produs> Produse { get; set; }
    }
}