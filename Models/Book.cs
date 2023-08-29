using FPTBook_v3.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FPTBook_v3.Models
{
    public class Book
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int book_Id { get; set; }
        [Required]
        public string book_Title { get; set; }
        [Required]
        public DateTime publication_date { get; set; }
        public string? book_ImagURL { get; set; }
        [Required]
        public string book_Description { get; set; }
        [Required]
        public double book_Price { get; set; }
        [Required]
        public int book_Quantity { get; set; }
        public int cate_Id { get; set; }

        [ForeignKey("cate_Id")]
        public virtual Category? category { get; set; }

        public virtual ICollection<OrderDetail>? OrderDetails { get; set; }


        [NotMapped]
        public IFormFile? book_Img { get; set; }
    }
}
