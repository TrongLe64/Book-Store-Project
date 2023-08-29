using FPTBook_v3.Constants;
using FPTBook_v3.Models;
using FPTBook_v3.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace FPTBook_v3.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _db;
        private readonly IFileService _fileService;
        public AdminController(ApplicationDbContext db,
            UserManager<ApplicationUser> userManager,
            IFileService fileService)
        {
            _db = db;
            _userManager = userManager;
            _fileService = fileService;
        }
        public IActionResult Index()
        {
            return View();
        }


        private bool UserExists(string id)
        {
            return _db.Users.Any(e => e.Id == id);
        }
        //--------------------------------User Controller------------------------------//
        [Route("Admin/ShowUser")]
        public async Task<IActionResult> ShowUser()
        {   
            var user = await (from users in _db.Users
                              join UserRole in _db.UserRoles
                              on users.Id equals UserRole.UserId
                              join role in _db.Roles
                              on UserRole.RoleId equals role.Id
                              where role.Name == "User"
                              select users).ToListAsync();
            return View(user);
        }

        [Route("Admin/DeleteUser")]
        public async Task<IActionResult> DeleteUser(string id)
        {

            var user = await _db.Users.FirstOrDefaultAsync(m => m.Id == id);
            if (user == null)
            {
                return RedirectToAction("ShowUser");
            }
            else
            {
                _db.Users.Remove(user);
                await _db.SaveChangesAsync();
                return RedirectToAction("ShowUser");
            }
           
        }

        [Route("Admin/EditUser/{id:}")]
        public  IActionResult EditUser(string id)
        {
            ApplicationUser user = _db.Users.Find(id);
            if (user == null)
            {
               return RedirectToAction("Index");
            }
            return View(user);
        }

        [HttpPost]
        [Route("Admin/EditUser/{id:}")]
        public  IActionResult EditUser(string? User_Name, string? PhoneNumber, string? Email, ApplicationUser user )
        {
            if (ModelState.IsValid)
            {

                _db.Entry(user).Reload();
                user.User_Name = User_Name;
                user.Email = Email;
                user.PhoneNumber = PhoneNumber; 
                _db.Users.Update(user);
                _db.SaveChanges();
                return RedirectToAction("ShowUser");
            }
            return View(user);
        }

        //-----------------------------Owner Controller---------------------------------//

        [Route("Admin/ShowOwner")]
        public async Task<IActionResult> ShowOwner()
        {
            var owner = await (from users in _db.Users
                              join UserRole in _db.UserRoles
                              on users.Id equals UserRole.UserId
                              join role in _db.Roles
                              on UserRole.RoleId equals role.Id
                              where role.Name == "Owner"
                              select users).ToListAsync();
            return View(owner);
        }

        [Route("Admin/DeleteOwner")]
        public async Task<IActionResult> DeleteOwner(string id)
        {

            var user = await _db.Users.FirstOrDefaultAsync(m => m.Id == id);
            if (user == null)
            {
                return RedirectToAction("ShowOwner");
            }
            else
            {
                _db.Users.Remove(user);
                await _db.SaveChangesAsync();
                return RedirectToAction("ShowOwner");
            }

        }

        [Route("Admin/EditOwner/{id:}")]
        public IActionResult EditOwner(string id)
        {
            ApplicationUser user = _db.Users.Find(id);
            if (user == null)
            {
                return RedirectToAction("Index");
            }
            return View(user);
        }

        [HttpPost]
        [Route("Admin/EditOwner/{id:}")]
        public IActionResult EditOwner( string? User_Name, string? PhoneNumber, string? Email, ApplicationUser user)
        {
            if (ModelState.IsValid)
            {

                _db.Entry(user).Reload();
                user.User_Name = User_Name;
                user.Email = Email;
                user.PhoneNumber = PhoneNumber;
                _db.Users.Update(user);
                _db.SaveChanges();
                return RedirectToAction("ShowOwner");
            }
            return View(user);
        }


        [Route("Admin/RequestCategory")]
        public IActionResult RequestCategory()
        {
            var category = _db.Categorys.Where(c => c.cate_Status == "processing").ToList();
            return View(category);
        }

        [Route("Admin/RequestCategory/Approval")]
        public IActionResult Approval(int id)
        {
            Category category = _db.Categorys.Find(id);
            if (category == null)
            {
                return RedirectToAction("RequestCategory");
            }
            else
            {
                category.cate_Status = "processed";
                _db.Categorys.Update(category);
                _db.SaveChanges();
                return RedirectToAction("RequestCategory");
            }
            
        }


        [Route("Admin/RequestCategory/Reject")]
        public IActionResult Reject(int id)
        {
            Category category = _db.Categorys.Find(id);
            if (category == null)
            {
                return RedirectToAction("RequestCategory");
            }
            else
            {
                _db.Categorys.Remove(category);
                _db.SaveChanges();
                return RedirectToAction("RequestCategory");
            }
        }

        [Route("Admin/RegisterOwner")]
        public async Task<IActionResult> RegisterOwner()
        {
            return View();
        }

        [HttpPost]
        [Route("Admin/RegisterOwner")]
        public async Task<IActionResult> RegisterOwner(Models.Owner owners)
        {
            if (ModelState.IsValid)
            {
                if (owners.Password != owners.ConfirmPassword)
                {
                    ModelState.AddModelError("ConfirmPassword", "The password and confirmation password do not match.");
                    return View(owners);
                }
                var owner = new ApplicationUser
                {
                    UserName = owners.Email,
                    User_Name = owners.User_Name,
                    Email = owners.Email,
                    PhoneNumber = owners.PhoneNumber,
                    

                };

             /*   if (owners.User_Img != null)
                {
                    var resultt = _fileService.SaveImage(owners.User_Img);
                    if (resultt.Item1 == 1)
                    {
                        var oldImage = owner.User_Img;
                        owner.User_Img = resultt.Item2;

                        
                        if (oldImage == null)
                        {

                        }
                        else
                        {
                            var delete = _fileService.Delete(oldImage);
                        }

                    }
                }*/

                var result = await _userManager.CreateAsync(owner, owners.Password);

                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(owner, Role.Owner.ToString());
                    return RedirectToAction("ShowOwner");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    TempData["Fail"] = "RegisterOwner Fail!";
                    return RedirectToAction("RegisterOwner");
                }
            }
            return RedirectToAction("RegisterOwner");
        }
    }
}
