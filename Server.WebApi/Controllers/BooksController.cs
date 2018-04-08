using System.Collections.Generic;
using System.Web.Http;
using Model;

namespace Siemplify.Server.WebApi.Controllers
{
    public class BooksController : ApiController
    {
        private List<Book> _books;
        public BooksController()
        {
            _books = new List<Book>();
            _books .Add(new Book {Name = "Book-1"});
            _books.Add(new Book { Name = "Book-2" });
            _books.Add(new Book { Name = "Book-3" });
        }
        [HttpPost]
        [Route("api/v1/addbook")]
        public IHttpActionResult AddBook(Book book)
        {
            _books.Add(book);
            return Ok(string.Format("Book {0} was added.", book.Name));
        }

        [HttpGet]
        [Route("api/v1/books")]
        public List<Book> GetBooks()
        {
            return _books;
        }
    }
}
