using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace FPTBook_v3.Data
{
    public class ApplicationUser: IdentityUser
    {
        public string? User_Name { get; set; }
        public string? User_Img { get; set; }

        public virtual ICollection<ShoppingCart>? ShoppingCarts { get; set; }
        public virtual ICollection<Order>? Orders { get; set; }
    }

    public class Owner: ApplicationUser
    {
        [Required]
        [StringLength(100, ErrorMessage =
            "The {0} must be at least {2} and at max {1} characters long.", 
            MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }
}
