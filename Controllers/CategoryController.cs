using FPTBook_v3.Models;
using FPTBook_v3.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FPTBook_v3.Models
{

    [Authorize(Roles = "Owner")]


    public class CategoryController : Controller
    {
        private readonly ApplicationDbContext _db;

        public CategoryController(ApplicationDbContext db)
        {
            _db = db;
        }


        [Route("/Owner/Category")]
        public IActionResult Index()
        {
            IEnumerable<Category> ds = _db.Categorys.Where(c => c.cate_Status == "processed").ToList();
            
            return View(ds);
        }

        [Route("/Owner/Category/Create")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [Route("/Owner/Category/Create")]
        public IActionResult Create(Category category)
        {
            
            if (ModelState.IsValid)
            {
                category.cate_Status = "Processing";
                _db.Categorys.Add(category);
                _db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(category);
        }

        [Route("/Owner/Category/Edit/{id:}")]
        public IActionResult Edit(int id)
        {
            Category category = _db.Categorys.Find(id);
            if (category == null)
            {
                return RedirectToAction("Index");
            }
            return View(category);
        }

        [HttpPost]
        [Route("/Owner/Category/Edit/{id:}")]
        public IActionResult Edit(int id, Category category)
        {
            if (ModelState.IsValid)
            {
                category.cate_Id = id;
                category.cate_Status = "processed";
                _db.Categorys.Update(category);
                _db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(category);
        }


        [Route("/Owner/Category/Delete/{id:}")]
        public ActionResult Delete(int id)
        {
            Category category = _db.Categorys.Find(id);
            if (category == null)
            {
                return RedirectToAction("Index");
            }
            else
            {
                _db.Categorys.Remove(category);
                _db.SaveChanges();
                return RedirectToAction("Index");
            }
        }
    }
}
