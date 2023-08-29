
using FPTBook_v3.Data;
using FPTBook_v3.Models;
using FPTBook_v3.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Drawing.Printing;
using System.IO;

using BookDisplayModel = FPTBook_v3.Models.BookDisplayModel;

namespace FPTBook_v3.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _db;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext db)
        {
            _db = db;
            _logger = logger;
        }

        [Route("Home/ShowBook")]
        public async Task<IActionResult> ShowBook(int? page, string sterm = "", int genreId = 0)
        {
            IEnumerable<Book> books = await GetBooks(sterm, genreId);
            IEnumerable<Category> categories = await _db.Categorys
                .Where(x => x.cate_Status == "processed")
                .ToListAsync();

            int pageSize = 8;
            int pageNumber = Math.Max(page ?? 1, 1);

            IPagedList<Book> pagedBooks = books.ToPagedList(pageNumber, pageSize);

            BookDisplayModel bookModel = new BookDisplayModel
            {
                Books = pagedBooks, // Use the paginated books here
                Categorys = categories,
                STerm = sterm,
                GenreId = genreId
            };

            return View(bookModel);

        }


        [Route("/Book/Detail/{id:}")]
        public async Task<IActionResult> BookDetail(int id)
        {
            if (id == null || _db.Books == null)
            {
                return NotFound();
            }
            else
            {
                var book = _db.Books.Include(x => x.category).FirstOrDefault(b => b.book_Id == id);
                if (book == null)
                {
                    return NotFound();
                }
                else
                {
                    return View(book);
                }
            }

        }
        public async Task<IActionResult> Help() => View();

        public async Task<IActionResult> Index(string sterm = "", int genreId = 0)
        {
            IEnumerable<Book> books = await IndexGetBook(sterm, genreId);
            IEnumerable<Category> categorys = await _db.Categorys.Where(x => x.cate_Status == "processed").ToListAsync(); ;
            IPagedList<Book> pagedBooks = books.ToPagedList();

            BookDisplayModel bookModel = new BookDisplayModel
            {
                Books = pagedBooks, // Use the paginated books here
                Categorys = categorys,
                STerm = sterm,
                GenreId = genreId
            };
            return View(bookModel);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public async Task<IEnumerable<Category>> Category()
        {
            return await _db.Categorys.ToListAsync();
        }


        public async Task<IEnumerable<Book>> IndexGetBook(string sTerm = "", int genreId = 0)
        {

            IEnumerable<Book> books = await (from book in _db.Books
                                             join genre in _db.Categorys
                                             on book.cate_Id equals genre.cate_Id
                                             where string.IsNullOrWhiteSpace(sTerm) || (book != null && book.book_Title.ToLower().StartsWith(sTerm))
                                             select book).ToListAsync();

            if (genreId != 0 && sTerm != null)
            {

                books = await (from book in _db.Books
                               join genre in _db.Categorys
                               on book.cate_Id equals genre.cate_Id
                               where genre.cate_Id == genreId && book.book_Title == sTerm
                               select book).ToListAsync();
                /*books = books.Where(a => a.book_Id == genreId).ToList();*/
            }
            else if (genreId != 0 && sTerm == null)
            {
                books = await (from book in _db.Books
                               join genre in _db.Categorys
                               on book.cate_Id equals genre.cate_Id
                               where genre.cate_Id == genreId
                               select book).ToListAsync();
            }
            return books;

        }


        public async Task<IEnumerable<Book>> GetBooks(string sTerm = "", int genreId = 0)
        {

            IEnumerable<Book> books = await (from book in _db.Books
                                             join genre in _db.Categorys
                                             on book.cate_Id equals genre.cate_Id
                                             where string.IsNullOrWhiteSpace(sTerm) || (book != null && book.book_Title.ToLower().StartsWith(sTerm))
                                             select book).ToListAsync();

            if (genreId != 0 && sTerm != null)
            {

                books = await (from book in _db.Books
                               join genre in _db.Categorys
                               on book.cate_Id equals genre.cate_Id
                               where genre.cate_Id == genreId && book.book_Title == sTerm
                               select book).ToListAsync();
                /*books = books.Where(a => a.book_Id == genreId).ToList();*/
            }
            else if (genreId != 0 && sTerm == null)
            {
                books = await (from book in _db.Books
                               join genre in _db.Categorys
                               on book.cate_Id equals genre.cate_Id
                               where genre.cate_Id == genreId
                               select book).ToListAsync();
            }
            return books;

        }


    }
}