
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using System.Net;
using FPTBook_v3.Data;
using FPTBook_v3.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace FPTBook_v3.Controllers
{

    [Authorize(Roles = "User")]
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CartController(ApplicationDbContext db, IHttpContextAccessor httpContextAccessor,
            UserManager<ApplicationUser> userManager)
        {
            _db = db;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
        }

        [Route("User/Cart/AddItem")]
        public async Task<IActionResult> AddItem(int bookId, int qty = 1, int redirect = 0)
        {
            var cartCount = await AddItemCart(bookId, qty);

            if (redirect == 0)
                return Ok(cartCount);
            return RedirectToAction("GetUserCart");
        }

        [Route("User/Cart/RemoveItem")]
        public async Task<IActionResult> RemoveItem(int bookId)
        {
            var cartCount = await RemoveCartItem(bookId);
            return RedirectToAction("GetUserCart");
        }

        [Route("User/Cart/GetUserCart")]
        public async Task<IActionResult> GetUserCart()
        {
            var cart = await GetCartItem();
            return View(cart);
        }

        [Route("User/Cart/GetTotalItemInCart")]
        public async Task<IActionResult> GetTotalItemInCart()
        {
            int cartItem = await GetCartItemCount();
            return Ok(cartItem);
        }

        [Route("User/Cart/Checkout")]
        public async Task<IActionResult> Checkout()
        {
            var isCheckedOut = await DoCheckout();
            if (!isCheckedOut)
            {
                TempData["Quantity"] = "The number of products is not enough!";
                return Redirect("~/User/Cart/GetUserCart");
            }
            else
            {
                TempData["Success"] = "Order Success";
                return Redirect("~/User/Cart/GetUserCart");
            }
                
            
        }


        public async Task<int> AddItemCart(int bookId, int qty)
        {
            string userId = GetUserId();
            try
            {
                if (string.IsNullOrEmpty(userId))
                    throw new Exception("user is not logged-in");
                var cart = await GetCart(userId);
                if (cart is null)
                {
                    cart = new ShoppingCart
                    {
                        UserId = userId
                    };
                    _db.ShoppingCarts.Add(cart);
                }
                _db.SaveChanges();

                var cartItem = _db.CartDetails
                                  .FirstOrDefault(a => a.ShoppingCartId == cart.Id && a.BookId == bookId);
                if (cartItem is not null)
                {
                        _db.SaveChanges();
                        cartItem.Quantity += qty;   
                }
                else
                {
                  
                        var book = _db.Books.Find(bookId);
                        cartItem = new CartDetail
                        {
                            BookId = bookId,
                            ShoppingCartId = cart.Id,
                            Quantity = qty,
                            UnitPrice = book.book_Price  // it is a new line after update


                        };
                        _db.CartDetails.Add(cartItem);
                        
                }
                _db.SaveChanges();
            }
            catch (Exception ex)
            {
            }
            var cartItemCount = await GetCartItemCount(userId);
            return cartItemCount;
        }


        public async Task<int> RemoveCartItem(int bookId)
        {
            //using var transaction = _db.Database.BeginTransaction();
            string userId = GetUserId();
            try
            {
                if (string.IsNullOrEmpty(userId))
                    throw new Exception("user is not logged-in");
                var cart = await GetCart(userId);
                if (cart is null)
                    throw new Exception("Invalid cart");
                // cart detail section
                var cartItem = _db.CartDetails
                                  .FirstOrDefault(a => a.ShoppingCartId == cart.Id && a.BookId == bookId);
                if (cartItem is null)
                    throw new Exception("Not items in cart");
                else if (cartItem.Quantity == 1)
                    _db.CartDetails.Remove(cartItem);
                else
                    cartItem.Quantity = cartItem.Quantity - 1;
                _db.SaveChanges();
            }
            catch (Exception ex)
            {

            }
            var cartItemCount = await GetCartItemCount(userId);
            return cartItemCount;
        }

        public async Task<ShoppingCart> GetCartItem()
        {
            var userId = GetUserId();
            if (userId == null)
                throw new Exception("Invalid userid");
            var shoppingCart = await _db.ShoppingCarts
                                  .Include(a => a.CartDetails)
                                  .ThenInclude(a => a.Book)
                                  .ThenInclude(a => a.category)
                                  .Where(a => a.UserId == userId).FirstOrDefaultAsync();
            return shoppingCart;

        }
        public async Task<ShoppingCart> GetCart(string userId)
        {
            var cart = await _db.ShoppingCarts.FirstOrDefaultAsync(x => x.UserId == userId);
            return cart;
        }

        public async Task<int> GetCartItemCount(string userId = "")
        {
            if (!string.IsNullOrEmpty(userId))
            {
                userId = GetUserId();
            }
            var data = await (from cart in _db.ShoppingCarts
                              join cartDetail in _db.CartDetails
                              on cart.Id equals cartDetail.ShoppingCartId
                              select new { cartDetail.Id }
                        ).ToListAsync();
            return data.Count;
        }

        public async Task<bool> DoCheckout()
        {
            try
            {
                var userId = GetUserId();
                if (string.IsNullOrEmpty(userId))
                    throw new Exception("User is not logged-in");
                var cart = await GetCart(userId);
                if (cart is null)
                    throw new Exception("Invalid cart");
                var cartDetail = _db.CartDetails
                                    .Where(a => a.ShoppingCartId == cart.Id).ToList();
                if (cartDetail.Count == 0)
                    throw new Exception("Cart is empty");
                var order = new Order
                {
                    UserId = userId,
                    CreateDate = DateTime.UtcNow,
                };
                _db.Orders.Add(order);
                _db.SaveChanges();
                foreach (var item in cartDetail)
                {

                    var orderDetail = new OrderDetail
                    {
                        BookId = item.BookId,
                        OrderId = order.Id,
                        Quantity = item.Quantity,
                        UnitPrice = item.UnitPrice
                    };
                    _db.OrderDetails.Add(orderDetail);


                    var quantity = _db.Books.FirstOrDefault(a => a.book_Id == item.BookId);
                    
                        if (quantity.book_Quantity < item.Quantity)
                        {
                            return false;
                        }
                        else
                        {
                            quantity.book_Quantity = quantity.book_Quantity - item.Quantity;
                            _db.Update(quantity);
                            _db.SaveChanges();  
                        }
                }
                _db.CartDetails.RemoveRange(cartDetail);
                _db.SaveChanges();

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private string GetUserId()
        {
            var principal = _httpContextAccessor.HttpContext.User;
            string userId = _userManager.GetUserId(principal);
            return userId;
        }
    }
}
