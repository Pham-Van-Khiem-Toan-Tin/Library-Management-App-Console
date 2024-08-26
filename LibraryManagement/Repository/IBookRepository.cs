using LibraryManagement.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagement.Repository
{
    public interface IBookRepository
    {
        List<Book> FindAllBookLikeTitle(string title);
        bool DeleteBookById(string id);
        bool EditBook(Book book);
        bool InsertBook(Book book);
    }
}
