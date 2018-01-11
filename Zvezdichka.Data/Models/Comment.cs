using System;
using System.ComponentModel.DataAnnotations;

namespace Zvezdichka.Data.Models
{
    public class Comment
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Message { get; set; }

        [Required]
        public int ProductId { get; set; }

        public Product Product { get; set; }

        [Required]
        public string UserId { get; set; }

        public ApplicationUser User { get; set; }

        [Required]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MMMM/YYYY HH:mm:ss}")]
        public DateTime DateAdded { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MMMM/YYYY HH:mm:ss}")]
        public DateTime? DateEdited { get; set; }

        [Required]
        public bool IsEdited { get; set; }
    }
}