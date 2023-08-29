using FPTBook_v3.Data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FPTBook_v3.Models
{
    public class Order
    {
        [Key]
        public int Id { get; set; }
        public string UserId { get; set; }
        [Required, Display(Name = "Create Date")]
        public DateTime CreateDate { get; set; } = DateTime.UtcNow;

        public ICollection<OrderDetail> OrderDetail { get; set; }

        [ForeignKey("UserId")]
        public virtual ApplicationUser? ApplicationUsers { get; set; }
    }
}
