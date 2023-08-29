namespace FPTBook_v3.Models
{
    public class BookDisplayModel
    {
        public IPagedList<Book> Books { get; set; }
        public IEnumerable<Category> Categorys { get; set; }
        public string STerm { get; set; } = "";
        public int GenreId { get; set; } = 0;
        public int PageNumber { get; set; } 
        public int PageSize { get; set; }

        public static implicit operator BookDisplayModel(DTOs.BookDisplayModel v)
        {
            throw new NotImplementedException();
        }
    }
}
