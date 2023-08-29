using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FPTBook_v3.Models
{
    public class OrderDetail
    {

        [Required]
        public int OrderId { get; set; }
        [Required]
        public int? BookId { get; set; }
        [Required]
        public int Quantity { get; set; }
        [Required]
        public double UnitPrice { get; set; }
        public Order Order { get; set; }
        public Book Book { get; set; }
        public bool IsShipped { get; set; }
    }
}
