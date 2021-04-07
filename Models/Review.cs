using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ProiectAppWeb.Models
{
    public class Review
    {
        [Key]
        public int ReviewId { get; set; }
        [Required(ErrorMessage = "Comentariul are nevoie de un continut")]
        public String Content { get; set; }
        [Required(ErrorMessage = "Trebuie sa lasati si un rating produsului")]
        public int nrStars { get; set; }
        public int nrLikes { get; set; } // numarul de like uri pe care le are un review
        public int nrDislikes { get; set; } // numarul de dislike uri pe care le poate avea un produs
        public DateTime Data { get; set; }
        public int ProdusId { get; set; }
        virtual public Produs Produsul { get; set; }
        public string UserId { get; set; }

        public virtual ApplicationUser User { get; set; }
    }
}