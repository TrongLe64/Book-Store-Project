using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FPTBook_v3.Models
{
    public class Category
    {
            [Key]
            [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
            [Required]
            public int cate_Id { get; set; }
            [Required]
            [Display(Name = "cate_Name")]
            public string cate_Name { get; set; }
            [Required]
            [Display(Name = "cate_Description")]
            public string cate_Description { get; set; }
            public string? cate_Status { get; set; }

            public virtual ICollection<Book>? Books { get; set; }
     }
}
